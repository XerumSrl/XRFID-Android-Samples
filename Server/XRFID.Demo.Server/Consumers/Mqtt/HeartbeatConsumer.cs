using MassTransit;
using System.Text.Json;
using Xerum.XFramework.Common.Enums;
using Xerum.XFramework.Common.Exceptions;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Events;
using XRFID.Demo.Modules.Mqtt.Payloads;
using XRFID.Demo.Modules.Mqtt.Publishers;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Entities;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.Utilities;

namespace XRFID.Demo.Server.Consumers.Mqtt;

public class HeartbeatConsumer : IRequestConsumer<Heartbeat>
{
    private readonly ReaderRepository readerRepository;
    private readonly MovementRepository movementRepository;
    private readonly UnitOfWork uowk;
    private readonly IZebraMqttCommandPublisher zebraCommandPublisher;
    private readonly IConfiguration configuration;
    private readonly ILogger<HeartbeatConsumer> logger;

    public HeartbeatConsumer(ReaderRepository readerRepository,
                             MovementRepository movementRepository,
                             UnitOfWork uowk,
                             IZebraMqttCommandPublisher zebraCommandPublisher,
                             IConfiguration configuration,
                             ILogger<HeartbeatConsumer> logger)
    {
        this.readerRepository = readerRepository;
        this.movementRepository = movementRepository;
        this.uowk = uowk;
        this.logger = logger;
        this.zebraCommandPublisher = zebraCommandPublisher;
        this.configuration = configuration;
    }

    public async Task Consume(ConsumeContext<Heartbeat> context)
    {
        logger.LogTrace("[Consume<Heartbeat>] message: {@message}", context.Message);

        try
        {
            var reader = (await readerRepository.GetAsync(q => q.Name == context.Message.HostName)).FirstOrDefault();

            if (reader is null)
            {

                GpoConfiguration gpoConfiguration = new();

                //aggiungiamo in stato da approvare

                reader = await readerRepository.CreateAsync(new Reader
                {
                    Name = context.Message.HostName,
                    //Ip = "0.0.0.0"
                    Status = ReaderStatus.Connected,
                    CorrelationId = Guid.Empty,
                    GpoConfiguration = JsonSerializer.Serialize(gpoConfiguration),
                });

                await uowk.SaveAsync();

                //chiedo update di alcuni dati direttamente al reader, invocando un comando su ZebraMqtt.ZebraCommandConsumer
                Task version = zebraCommandPublisher.Publish(new RAWMQTTCommands
                {
                    Topic = "mcmds",
                    HostName = reader.Name,
                    Command = "get_version",
                    CommandId = Guid.NewGuid().ToString(),
                    Payload = new OneOfRAWMQTTCommandsPayload(),
                });

                //chiedo update di alcuni dati direttamente al reader, invocando un comando su ZebraMqtt.ZebraCommandConsumer
                Task network = zebraCommandPublisher.Publish(new RAWMQTTCommands
                {
                    Topic = "mcmds",
                    HostName = reader.Name,
                    Command = "get_network",
                    CommandId = Guid.NewGuid().ToString(),
                    Payload = new OneOfRAWMQTTCommandsPayload(),
                });

                await version;
                await network;

                return;
            }

            ////Heartbeat Time
            //reader.HeartbeatTime = DateTime.Now;
            ////is connected
            //reader.Status = ReaderStatusEnumeration.Connected.Key;
            //await readerManager.UpdateAsync(reader);


            var actMov = (await movementRepository.GetAsync(q => q.IsActive && q.ReaderId == reader.Id)).FirstOrDefault();

            //WorkflowType active movement is equal to reder WorkflowType
            if (actMov is null || reader.ActiveMovementId == Guid.Empty)
            {
                if (actMov is null)
                {
                    var newActiveMov = await movementRepository.CreateAsync(new Movement
                    {
                        ReaderId = reader.Id,
                        Name = $"{reader.Name}_{DateTime.Now}",
                        IsActive = true,
                    });
                    if (newActiveMov is null)
                    {
                        logger.LogWarning("[Consume<Heartbeat>] Creation initial movement error for reader: {Name}", reader.Name);
                        return;
                    }

                    reader.ActiveMovementId = newActiveMov.Id;
                    await readerRepository.UpdateAsync(reader);

                    await uowk.SaveAsync();

                    logger.LogDebug("[Consume<Heartbeat>] movement {Id} created for reader {Name}", newActiveMov.Id, reader.Name);

                }
                else
                {
                    reader.ActiveMovementId = actMov.Id;
                    await readerRepository.UpdateAsync(reader);


                    logger.LogWarning("[Consume<Heartbeat>] update movementId {ActiveMovementId}", reader.ActiveMovementId);
                }
            }
        }
        catch (Exception ex) when (ex is WaitingApprovalException || ex is InvalidLicenseException)
        {
            logger.LogWarning("[Consume<SdkHeartbeat>] " + ex.Message);
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Consume<SdkHeartbeat>] Unexpected exception");
            return;
        }
    }

}

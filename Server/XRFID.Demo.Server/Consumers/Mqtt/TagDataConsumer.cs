using MassTransit;
using Xerum.XFramework.Common.Exceptions;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Common.Dto;
using XRFID.Demo.Common.Enumerations;
using XRFID.Demo.Modules.Mqtt.Contracts;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Contracts;

namespace XRFID.Demo.Server.Consumers.Mqtt;

public class TagDataConsumer : IRequestConsumer<ZebraTagData>
{
    private readonly ReaderRepository readerRepository;
    private readonly ILogger<TagDataConsumer> logger;

    public TagDataConsumer(ReaderRepository readerRepository,
                           ILogger<TagDataConsumer> logger)
    {
        this.readerRepository = readerRepository;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<ZebraTagData> context)
    {
        logger.LogTrace(context.Message.ToString());
        logger.LogDebug("[Consume<ZebraTagData>] {HostName}|Epc: {IdHex} Format: {Format}", context.Message.HostName, context.Message.IdHex, context.Message.Format);

        try
        {
            var reader = (await readerRepository.GetAsync(q => q.Name == context.Message.HostName)).FirstOrDefault();
            logger.LogDebug("[Consume<ZebraTagData>] {HostName}|Getting reader information Id: {Id}", context.Message.HostName, reader.Id);
            logger.LogDebug("[Consume<ZebraTagData>] {HostName}|starting with EPC {IdHex}", context.Message.HostName, context.Message.IdHex);


            logger.LogDebug("[Consume<ZebraTagData>] {HostName}|Beginning switch WorkflowType.Shipment with EPC {IdHex}", context.Message.HostName, context.Message.IdHex);

            await context.Publish(new ShipmentTagData
            {
                CorrelationId = reader.CorrelationId, //coincide con l'active movement. In caso di input da client esterni, va gestito mantenuto il correlation id
                ReaderId = reader.Id,
                //MovementId = reader.CorrelationId,
                MovementId = reader.ActiveMovementId ?? Guid.Empty,
                HostName = context.Message.HostName,

                TagAction = new TagActionDto
                {
                    Action = (context.Message != null) ? TagActionEnumeration.Read.Key : TagActionEnumeration.NotRead.Key,
                    IsTransfered = false,
                    IsValid = false,
                    IgnoreUntil = DateTime.Now.AddDays(1),

                    Tag = new TagDto
                    {
                        Epc = context.Message.IdHex.ToUpper(),
                        //Format = context.Message.Format,
                        TagSeenCount = context.Message.Reads ?? 1,

                        //Phase = context.Message.Phase,
                        //UserMemory = context.Message.UserMemory,
                        //Channel = context.Message.Channel,
                        PC = context.Message.Pc,
                        //Xpc = context.Message.Xpc,
                        //Crc = context.Message.Crc,
                        Tid = context.Message.Tid,
                        Rssi = context.Message.PeakRssi ?? 0,
                        //UserDefined = context.Message.UserDefined,

                        Timestamp = context.Message.Timestamp
                    }
                }

            });

            logger.LogDebug("[Consume<ZebraTagData>] {HostName}|  with EPC {IdHex}", context.Message.HostName, context.Message.IdHex);


            logger.LogDebug("[Consume<ZebraTagData>] {HostName}|Responded to XTagData with EPC {IdHex}", context.Message.HostName, context.Message.IdHex);
        }
        catch (Exception ex) when (ex is WaitingApprovalException || ex is InvalidLicenseException || ex is WorkflowTypeOutOfRangeException)
        {
            logger.LogError("[Consume<ZebraTagData>] {Message}", ex.Message);
            return;
        }
        catch (NotImplementedException niex)
        {
            logger.LogWarning("[Consume<ZebraTagData>] {Message}", niex.Message);
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Consume<ZebraTagData>] Unexpected Exception");
            return;
        }
    }
}
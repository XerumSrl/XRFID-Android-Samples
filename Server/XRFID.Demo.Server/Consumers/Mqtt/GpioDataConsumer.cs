using MassTransit;
using Xerum.XFramework.Common.Exceptions;
using Xerum.XFramework.MassTransit;
using XRFID.Demo.Modules.Mqtt.Contracts;
using XRFID.Demo.Server.Helpers;
using XRFID.Demo.Server.Services;
using XRFID.Demo.Server.StateMachines.Shipment.Contracts;

namespace XRFID.Demo.Server.Consumers.Mqtt;

public class GpioDataConsumer :
    IRequestConsumer<ZebraGpiData>
{
    private readonly ReaderService readerService;
    private readonly ILogger<GpioDataConsumer> logger;

    private readonly bool[] modules;

    public GpioDataConsumer(ReaderService readerService,
                            IConfiguration configuration,
                            ILogger<GpioDataConsumer> logger)
    {
        this.readerService = readerService;
        this.logger = logger;

        modules = ModuleParser.ParseModules(configuration.GetValue("Modules", 0ul), false);
    }

    /// <summary>
    /// Consuma evento GpiEvent da ManagedMqttClientService
    /// Invia SubmitGpIn a 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<ZebraGpiData> context)
    {
        logger.LogDebug("[Consume<ZebraGpiData>] {HostName}|Begin Consume Pin: {Pin} Value: {Value}", context.Message.HostName, context.Message.Pin, context.Message.Value);
        logger.LogDebug("[Consume<ZebraGpiData>] {HostName}|Message: {@Message}", context.Message.HostName, context.Message);

        try
        {
            var reader = await readerService.GetByNameAsync(context.Message.HostName);

            if (reader is null)
            {
                logger.LogWarning("[Consume<ZebraGpiData>] no reader found for hostname: {HostName}", context.Message.HostName);
                return;
            }

            logger.LogDebug("[Consume<ZebraGpiData>] {HostName}|Publish for State Machine| Pin: {Pin} Value: {Value}", context.Message.HostName, context.Message.Pin, context.Message.Value);


            await context.Publish(new ShipmentGpiData
            {
                CorrelationId = reader.CorrelationId,
                ReaderId = reader.Id,
                HostName = reader.Name ?? context.Message.HostName,

                Id = context.Message.Pin,
                Value = context.Message.Value,
            });


            logger.LogDebug("[Consume<ZebraGpiData>] {HostName}|Responded to XTagData on pin {Pin} with value {Value}", context.Message.HostName, context.Message.Pin, context.Message.Value);

        }
        catch (WorkflowTypeOutOfRangeException)
        {
            logger.LogWarning("[Consume<ZebraGpiData>] Unknown workflowtype.");
            return;
        }
        catch (Exception ex) when (ex is WaitingApprovalException || ex is InvalidLicenseException || ex is NotSupportedException)
        {
            logger.LogWarning("[Consume<ZebraGpiData>] " + ex.Message);
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Consume<ZebraGpiData>] Unexpected exception");
            return;
        }

        logger.LogDebug("[Consume<ZebraGpiData>] {HostName}|End Consume Pin: {Pin} Value: {Value}", context.Message.HostName, context.Message.Pin, context.Message.Value);
    }

}

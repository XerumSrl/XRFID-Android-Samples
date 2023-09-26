using MassTransit;
using XRFID.Demo.Server.StateMachines.Shipment.Contracts;
using XRFID.Demo.Server.StateMachines.Shipment.Interfaces;

namespace XRFID.Demo.Server.StateMachines.Shipment.Consumers;

public class ShipmentGpioConsumer :
    IConsumer<ShipmentGpiData>
{
    private readonly ILogger<ShipmentGpioConsumer> logger;

    public ShipmentGpioConsumer(ILogger<ShipmentGpioConsumer> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Consume SubmitGpIn da mevent consumer
    /// Invia response a mevent con GpInStatus
    /// Aggiorna state machine con IGpiEvent
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<ShipmentGpiData> context)
    {
        try
        {
            logger.LogDebug("ShipmentGpioConsumer|{CorrelationId}|ShipmentGpiData begin consumer", context.Message.CorrelationId);
            logger.LogDebug("ShipmentGpioConsumer|{CorrelationId}|ShipmentGpiData Id: {Id} Value: {Value}", context.Message.CorrelationId, context.Message.Id, context.Message.Value);

            //fa publish verso macchina a stati per gestione evento IStartEvent
            if (context.Message.Id == 1 && context.Message.Value == true)
            {
                if (context.Message.CorrelationId == Guid.Empty)
                {
                    logger.LogDebug("ShipmentGpioConsumer|{CorrelationId}|Publish IInitializeEvent on ReaderId {ReaderId}", context.Message.CorrelationId, context.Message.ReaderId);
                    await context.Publish<IInitializeEvent>(new
                    {
                        CorrelationId = context.Message.CorrelationId,
                        ReaderId = context.Message.ReaderId,
                        HostName = context.Message.HostName,

                        GpiId = context.Message.Id,
                        GpiValue = context.Message.Value,
                        Timestamp = DateTime.Now,
                    });
                }
                else
                {
                    logger.LogWarning("ShipmentGpioConsumer|{CorrelationId}|Skip. IInitializeEvent already has a CorrelationId", context.Message.CorrelationId);
                }

            }
            else if (context.Message.CorrelationId != Guid.Empty)
            {
                //fa publish verso macchina a stati per gestione evento IGpiEvent
                await context.Publish<IGpiEvent>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    ReaderId = context.Message.ReaderId,
                    HostName = context.Message.HostName,

                    GpiId = context.Message.Id,
                    GpiValue = context.Message.Value,
                    Timestamp = DateTime.Now,
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError("ShipmentGpioConsumer|{CorrelationId}|{Message}", context.Message.CorrelationId, ex.Message);

        }
        logger.LogDebug("ShipmentGpioConsumer|{CorrelationId}|ShipmentGpiData end consumer", context.Message.CorrelationId);

    }
}

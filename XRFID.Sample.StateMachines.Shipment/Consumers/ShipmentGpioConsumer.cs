using MassTransit;
using Microsoft.Extensions.Logging;
using Xerum.XRFID.Common.Contracts;
using XRFID.Server.Application.Services;
using XRFID.Server.StateMachines.Shipment.Contracts;
using XRFID.Server.StateMachines.Shipment.Interfaces;

namespace XRFID.Sample.StateMachines.Shipment.Consumers;

public class ShipmentGpioConsumer :
    IConsumer<ShipmentGpiData>
{
    private readonly ILogger<ShipmentGpioConsumer> logger;
    private readonly IReaderService readerService;

    public ShipmentGpioConsumer(ILogger<ShipmentGpioConsumer> logger,
        IReaderService readerService)
    {
        this.logger = logger;
        this.readerService = readerService;
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
            if (context.Message.Id == readerService.GetReaderCache(context.Message.ReaderId).GpioConfiguration.GpInPulsante.Id
                && context.Message.Value == readerService.GetReaderCache(context.Message.ReaderId).GpioConfiguration.GpInPulsante.LogicOn)
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
            //*************************************************************

            if (context.RequestId != null)
            {
                //risponde a chi ha chiamato questo consumer (ZebraGpioEventConsumer)
                await context.RespondAsync<XGpiEvent>(new XGpiEvent
                {
                    Timestamp = InVar.Timestamp,

                    CorrelationId = context.Message.CorrelationId,
                    ReaderId = context.Message.ReaderId,
                    HostName = context.Message.HostName,

                    GpioId = context.Message.Id,
                    GpioValue = context.Message.Value,

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

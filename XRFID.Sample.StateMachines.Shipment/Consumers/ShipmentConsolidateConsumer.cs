namespace XRFID.Sample.StateMachines.Shipment.Consumers;
public class ShipmentConsolidateConsumer : IConsumer<ShipmentMovementConsolidateRequest>
{
    private readonly IMovementManager movementManager;
    private readonly ILoadingUnitItemManager loadingUnitItemManager;
    private readonly ILoadingUnitManager loadingUnitManager;
    private readonly ILogger<ShipmentConsolidateConsumer> logger;

    public ShipmentConsolidateConsumer(IMovementManager movementManager,
        ILoadingUnitItemManager loadingUnitItemManager,
        ILoadingUnitManager loadingUnitManager,
        ILogger<ShipmentConsolidateConsumer> logger)
    {
        this.movementManager = movementManager;
        this.loadingUnitItemManager = loadingUnitItemManager;
        this.loadingUnitManager = loadingUnitManager;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<ShipmentMovementConsolidateRequest> context)
    {
        try
        {
            //aggiorno lo status dei movementItems sul db

            var movement = await movementManager.GetActiveMovement(context.Message.ReaderId);
            if (movement is null)
            {
                logger.LogWarning("ShipmentConsolidateConsumer|No active movement for {ReaderId}", context.Message.ReaderId);
                return;
            }

            logger.LogDebug("ShipmentMovementConsolidateRequest|Consolidated movement {MovementId}", context.Message.MovementId);

            var updateMovement = await movementManager.UpdateStatusAsync(context.Message.MovementId);
            logger.LogDebug("ShipmentMovementConsolidateRequest|Consolidated movement {MovementId}", context.Message.MovementId);

        }
        catch (Exception ex)
        {
            logger.LogWarning("ShipmentConsolidateConsumer|{Message}", ex.Message);
        }

    }
}

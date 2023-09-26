using MassTransit;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.StateMachines.Shipment.Contracts;

namespace XRFID.Demo.Server.StateMachines.Shipment.Consumers;
public class ShipmentConsolidateConsumer : IConsumer<ShipmentMovementConsolidateRequest>
{
    private readonly MovementRepository movementRepository;
    private readonly LoadingUnitRepository loadingUnitRepository;
    private readonly LoadingUnitItemRepository loadingUnitItemRepository;
    private readonly ILogger<ShipmentConsolidateConsumer> logger;

    public ShipmentConsolidateConsumer(MovementRepository movementRepository,
                                       LoadingUnitRepository loadingUnitRepository,
                                       LoadingUnitItemRepository loadingUnitItemRepository,
                                       ILogger<ShipmentConsolidateConsumer> logger)
    {
        this.movementRepository = movementRepository;
        this.loadingUnitRepository = loadingUnitRepository;
        this.loadingUnitItemRepository = loadingUnitItemRepository;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<ShipmentMovementConsolidateRequest> context)
    {
        try
        {
            //aggiorno lo status dei movementItems sul db
            var movement = (await movementRepository.GetAsync(q => q.ReaderId == context.Message.ReaderId && q.IsActive)).FirstOrDefault();
            if (movement is null)
            {
                logger.LogWarning("ShipmentConsolidateConsumer|No active movement for {ReaderId}", context.Message.ReaderId);
                return;
            }

            logger.LogDebug("ShipmentMovementConsolidateRequest|Consolidated movement {MovementId}", context.Message.MovementId);

            var updateMovement = await movementRepository.UpdateStatusAsync(context.Message.MovementId);
            logger.LogDebug("ShipmentMovementConsolidateRequest|Consolidated movement {MovementId}", context.Message.MovementId);

        }
        catch (Exception ex)
        {
            logger.LogWarning("ShipmentConsolidateConsumer|{Message}", ex.Message);
        }

    }
}

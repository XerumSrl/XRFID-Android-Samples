namespace XRFID.Demo.Server.StateMachines.Shipment.Contracts;
public record ShipmentMovementConsolidateRequest
{
    public Guid ReaderId { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid MovementId { get; set; }
}

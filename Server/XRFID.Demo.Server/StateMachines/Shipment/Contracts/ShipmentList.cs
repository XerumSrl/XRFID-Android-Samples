namespace XRFID.Demo.Server.StateMachines.Shipment.Contracts;

public record ShipmentList
{
    public Guid ReaderId { get; set; }
    public Guid ListId { get; set; }
    public DateTime Timestamp { get; set; }

}

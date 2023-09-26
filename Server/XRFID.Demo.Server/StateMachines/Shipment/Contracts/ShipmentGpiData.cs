namespace XRFID.Demo.Server.StateMachines.Shipment.Contracts;

/// <summary>
/// SubmitGpIn -> GpInStatus
/// </summary>
public record ShipmentGpiData
{
    public Guid CorrelationId { get; set; }
    public Guid ReaderId { get; set; }
    public string HostName { get; set; }

    public int Id { get; set; }
    public bool Value { get; set; }

}

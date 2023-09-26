namespace XRFID.Demo.Server.StateMachines.Shipment.Interfaces;
public record IGpoBuzzerEvent
{
    public Guid CorrelationId { get; set; }
    public Guid ReaderId { get; set; }
    public string HostName { get; set; }

    public DateTime Timestamp { get; set; }

    public int GpoBuzzerId { get; set; }
    public bool GpoBuzzerValue { get; set; }
}

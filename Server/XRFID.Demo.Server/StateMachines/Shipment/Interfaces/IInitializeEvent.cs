namespace XRFID.Demo.Server.StateMachines.Shipment.Interfaces;

public record IInitializeEvent
{
    public Guid CorrelationId { get; set; }

    public Guid ReaderId { get; set; }
    public string HostName { get; set; }

    public DateTime Timestamp { get; set; }

    public int GpiId { get; set; }
    public bool GpiValue { get; set; }

    //public TimeSpan CompletionTime { get; set; }
}

namespace XRFID.Demo.Server.StateMachines.Shipment.Interfaces;

public record ITagEvent
{
    public Guid CorrelationId { get; set; }
    public Guid ReaderId { get; set; }
    public string HostName { get; set; }

    public string Epc { get; set; }
    public string? Tid { get; set; }
    public string? Pc { get; set; }
    public short? Rssi { get; set; } = 0;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public int TagSeenCount { get; set; } = 0;

    public string Format { get; set; }

    public string? Phase { get; set; }
    public string? UserMemory { get; set; }
    public string? Channel { get; set; }
    public string? Xpc { get; set; }
    public string? Crc { get; set; }

    public string? UserDefined { get; set; }

}

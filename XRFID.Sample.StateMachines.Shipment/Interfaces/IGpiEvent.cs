﻿namespace XRFID.Sample.StateMachines.Shipment.Interfaces;

public record IGpiEvent
{
    public Guid CorrelationId { get; set; }

    public Guid ReaderId { get; set; }
    public string HostName { get; set; }

    public DateTime Timestamp { get; set; }

    public int GpiId { get; set; }
    public bool GpiValue { get; set; }

    //public TimeSpan CompletionTime { get; set; }
}

﻿namespace XRFID.Sample.StateMachines.Shipment.Interfaces;

public interface ICheckEvent
{
    Guid ReaderId { get; set; }

    Guid SessionId { get; }
    DateTime Timestamp { get; }

}

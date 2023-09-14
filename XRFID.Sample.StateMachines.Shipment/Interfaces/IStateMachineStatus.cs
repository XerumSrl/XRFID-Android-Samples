namespace XRFID.Sample.StateMachines.Shipment.Interfaces;

public interface IStateMachineStatus
{
    Guid ReaderId { get; }
    Guid SessionId { get; }
    string ItemValue { get; }

    string State { get; }
}

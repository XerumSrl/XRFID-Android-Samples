using MassTransit;

namespace XRFID.Demo.Server.StateMachines.Shipment.States;

public class ShipmentState :
    SagaStateMachineInstance,
    ISagaVersion
{

    public Guid CorrelationId { get; set; }
    public Guid ReaderId { get; set; }
    public Guid MovementId { get; set; }
    public string HostName { get; set; }
    public string CurrentState { get; set; }
    public int Version { get; set; }

    public DateTime DateModified { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateCompleted { get; set; }
    public byte[] RowVersion { get; set; }

    ///Per la schedule
    public Guid? ReadingTimeoutTokenId { get; set; }
    public Guid? GpoBuzzerTimeoutTokenId { get; set; }

}
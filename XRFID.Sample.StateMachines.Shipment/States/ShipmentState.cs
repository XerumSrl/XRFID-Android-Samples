using MassTransit;

namespace XRFID.Sample.StateMachines.Shipment.States;

public class ShipmentState :
    SagaStateMachineInstance,
    ISagaVersion
{
    //public ShipmentState()
    //{
    //    //CorrelationId = Guid.NewGuid();
    //    Version = 1;
    //    DateModified = DateTime.Now;
    //    DateCreated = DateTime.Now;
    //    DateCompleted = DateTime.Now;
    //    HostName = "Default";
    //}

    public Guid CorrelationId { get; set; }
    public Guid ReaderId { get; set; }
    public Guid MovementId { get; set; }
    public string HostName { get; set; }
    public string CurrentState { get; set; }
    public int Version { get; set; }

    //public string ItemValue { get; set; }
    public DateTime DateModified { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateCompleted { get; set; }
    public byte[] RowVersion { get; set; }

    ///Per la schedule
    public Guid? ReadingTimeoutTokenId { get; set; }
    public Guid? GpoBuzzerTimeoutTokenId { get; set; }

}
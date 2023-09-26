namespace XRFID.Demo.Server.StateMachines.Shipment.Interfaces;

public interface IStatusEvent
{
    Guid SessionId { get; set; }
    String ReaderId { get; set; }
    string ListId { get; set; }
    DateTime Timestamp { get; set; }


    /// <summary>
    /// true -> Open
    /// false -> Completed
    /// </summary>
    bool Status { get; set; }
}

using XRFID.Sample.Common.Dto;

namespace XRFID.Sample.Server.StateMachines.Shipment.Contracts;

public record ShipmentTagData
{
    public Guid CorrelationId { get; set; }
    public Guid ReaderId { get; set; }
    public Guid MovementId { get; set; }
    public string HostName { get; set; }

    public TagActionDto TagAction { get; set; }
}

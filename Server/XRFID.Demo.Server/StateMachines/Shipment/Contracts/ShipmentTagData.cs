using XRFID.Demo.Common.Dto;

namespace XRFID.Demo.Server.StateMachines.Shipment.Contracts;

public record ShipmentTagData
{
    public Guid CorrelationId { get; set; }
    public Guid ReaderId { get; set; }
    public Guid MovementId { get; set; }
    public string HostName { get; set; }

    public TagActionDto TagAction { get; set; }
}

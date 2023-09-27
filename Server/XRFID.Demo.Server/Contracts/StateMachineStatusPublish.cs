using XRFID.Demo.Common.Enumerations;

namespace XRFID.Demo.Server.Contracts;

public record StateMachineStatusPublish
{
    public Guid CorrelationId { get; set; }
    public Guid RederId { get; set; }
    public Guid ActiveMovementId { get; set; }
    public StateMachineStateEnum State { get; set; } = StateMachineStateEnum.Stop;
}

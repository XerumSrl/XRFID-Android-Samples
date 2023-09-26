using XRFID.Demo.Common.Dto;

namespace XRFID.Demo.Server.Contracts;

public record StateMachineUiTagPublish
{
    public Guid ReaderId { get; set; }

    public Guid ActivMoveId { get; set; }

    public TagActionDto? Tag { get; set; }
}

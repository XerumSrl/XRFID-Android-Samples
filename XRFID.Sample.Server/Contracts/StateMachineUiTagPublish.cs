using XRFID.Sample.Common.Dto;

namespace XRFID.Sample.Server.Contracts;

public record StateMachineUiTagPublish
{
    public Guid ReaderId { get; set; }

    public Guid ActivMoveId { get; set; }

    public TagActionDto? Tag { get; set; }
}

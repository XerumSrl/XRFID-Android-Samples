using XRFID.Sample.Common.Dto;

namespace XRFID.Sample.Server.Contracts;

public record StateMachineUiTagPublish
{
    public Guid ReaderId { get; set; }
    public TagActionDto Tag { get; set; } = new TagActionDto();
}

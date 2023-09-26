using Xerum.XFramework.Common.Dto;

namespace XRFID.Demo.Common.Dto;

public class TagActionDto : AuditEntityDto
{
    public TagActionDto()
    {
        this.IsValid = false;
        this.IsTransfered = false;
        this.Id = Guid.NewGuid();
    }

    public TagActionDto(string action, TagDto tag = null)
    {
        this.Action = action;
        this.Tag = tag;
        this.IsValid = false;
        this.IsTransfered = false;
        this.Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public string Action { get; set; }

    public DateTime IgnoreUntil { get; set; }

    public TagDto Tag { get; set; }

    public bool IsValid { get; set; }

    public bool IsTransfered { get; set; }
}

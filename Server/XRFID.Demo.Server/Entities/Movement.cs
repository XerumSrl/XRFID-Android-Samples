using XRFID.Demo.Common.Enumerations;

namespace XRFID.Demo.Server.Entities;

public class Movement : AuditEntity
{
    public int Sequence { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public bool IsValid { get; set; }

    public bool UnexpectedItem { get; set; }

    public bool MissingItem { get; set; }

    public bool OverflowItem { get; set; }

    public bool IsActive { get; set; }

    public bool IsConsolidated { get; set; }

    public Guid ReaderId { get; set; }

    public List<MovementItem> MovementItems { get; set; } = new List<MovementItem>();

    public Guid? OrderId { get; set; }

    public string? OrderReference { get; set; }

    public MovementDirection Direction { get; set; } = MovementDirection.In;
}

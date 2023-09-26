namespace XRFID.Demo.Server.Entities;

public class LoadingUnit : AuditEntity
{
    public int Sequence { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public bool IsActive { get; set; }

    public bool IsValid { get; set; }

    public bool IsConsolidated { get; set; }

    public Guid ReaderId { get; set; }

    public Guid? OrderId { get; set; }

    public string? OrderReference { get; set; }

    public List<LoadingUnitItem> LoadingUnitItems { get; set; } = new List<LoadingUnitItem>();
}

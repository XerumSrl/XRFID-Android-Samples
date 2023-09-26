using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.Entities;

public class MovementItem : AuditEntity
{
    public string Description { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string Epc { get; set; } = string.Empty;

    public short Rssi { get; set; }

    public string? Tid { get; set; }

    public string? PC { get; set; }

    public int ReadsCount { get; set; }

    public bool Checked { get; set; }

    public DateTime FirstRead { get; set; } = DateTime.Now;

    public DateTime LastRead { get; set; } = DateTime.Now;

    public DateTime IgnoreUntil { get; set; } = (DateTime.Now + TimeSpan.FromDays(1)).Date;//ignore until today at midnight

    public ItemStatus Status { get; set; } = ItemStatus.NotFound;

    public bool IsValid { get; set; }

    public bool IsConsolidated { get; set; }

    public Guid MovementId { get; set; }
    public Movement Movement { get; set; }


    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid? LoadingUnitItemId { get; set; }
    public LoadingUnitItem? LoadingUnitItem { get; set; }
}

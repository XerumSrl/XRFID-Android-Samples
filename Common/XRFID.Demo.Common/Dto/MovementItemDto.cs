using Xerum.XFramework.Common.Dto;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Common.Dto;

public class MovementItemDto : RestEntityDto
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

    public bool IsValid { get => Status == ItemStatus.Found; }

    public bool IsConsolidated { get; set; }

    public Guid MovementId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? LoadingUnitItemId { get; set; }
}

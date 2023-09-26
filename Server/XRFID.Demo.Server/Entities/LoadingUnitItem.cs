using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.Entities;

public class LoadingUnitItem : AuditEntity
{
    public string Description { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }

    public string Epc { get; set; } = string.Empty;

    public bool IsConsolidated { get; set; }

    public ItemStatus Status { get; set; }

    public bool IsValid { get => Status == ItemStatus.Found; }

    public Guid LoadingUnitId { get; set; }
    public LoadingUnit LoadingUnit { get; set; }//entityframework likes this syntax for automatic foreignkey contraint setup

    public string LoadingUnitReference { get; set; } = string.Empty;
}

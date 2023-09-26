using Xerum.XFramework.Common.Dto;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Common.Dto;

public class LoadingUnitItemDto : RestEntityDto
{
    public string Description { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }

    public string Epc { get; set; } = string.Empty;

    public bool IsConsolidated { get; set; }

    public ItemStatus Status { get; set; }

    public bool IsValid { get => Status == ItemStatus.Found; }

    public Guid LoadingUnitId { get; set; }

    public string LoadingUnitReference { get; set; } = string.Empty;
}
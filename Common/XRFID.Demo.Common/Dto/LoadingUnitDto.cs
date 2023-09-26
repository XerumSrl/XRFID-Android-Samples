using Xerum.XFramework.Common.Dto;

namespace XRFID.Demo.Common.Dto;

public class LoadingUnitDto : RestEntityDto
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

    public List<LoadingUnitItemDto> LoadingUnitItems { get; set; } = new List<LoadingUnitItemDto>();
}
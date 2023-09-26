using Xerum.XFramework.Common.Dto;
using Xerum.XFramework.Common.Enums;
using XRFID.Demo.Common.Enumerations;

namespace XRFID.Demo.Common.Dto;

public class MovementDto : RestEntityDto
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

    /// <summary>
    /// init, processing, completed
    /// </summary>
    public MovementStatus WorkingStatus
    {
        get
        {
            if (IsActive && !IsConsolidated)
            {
                if (MovementItems.Any())
                {
                    return MovementStatus.Processing;
                }
                else
                {
                    return MovementStatus.Initialize;
                }
            }
            else
            {
                return MovementStatus.Completed;
            }
        }
    }

    public MovementStatus Status
    {
        get
        {
            if (IsValid)
            {
                return MovementStatus.Valid;
            }
            else if (UnexpectedItem)
            {
                return MovementStatus.Unexpected;
            }
            else if (MissingItem)
            {
                return MovementStatus.Missing;
            }
            else if (OverflowItem)
            {
                return MovementStatus.Overflow;
            }
            else
            {
                return MovementStatus.Error;
            }
        }
    }

    public Guid ReaderId { get; set; }

    public List<MovementItemDto> MovementItems { get; set; } = new List<MovementItemDto>();

    public Guid? OrderId { get; set; }

    public string? OrderReference { get; set; }

    public MovementDirection Direction { get; set; } = MovementDirection.In;
}
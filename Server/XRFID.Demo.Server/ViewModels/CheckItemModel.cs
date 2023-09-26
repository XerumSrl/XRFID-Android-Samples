using XRFID.Demo.Common.Enumerations;
using XRFID.Demo.Server.ViewModels.Enums;

namespace XRFID.Demo.Server.ViewModels;

public class CheckItemModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Epc { get; set; } = string.Empty;
    public CheckStatusEnum CheckStatus { get; set; } = CheckStatusEnum.NotFound;
    public string Description { get; set; } = string.Empty;
    public DateTime DateTime { get; set; } = DateTime.Now;
    public MovementDirection Direction { get; set; }
}

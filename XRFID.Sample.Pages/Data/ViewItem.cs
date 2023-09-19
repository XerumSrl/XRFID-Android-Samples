using XRFID.Sample.Pages.Data.Enums;

namespace XRFID.Sample.Pages.Data;

public class ViewItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Epc { get; set; } = string.Empty;
    public CheckStatusEnum CheckStatus { get; set; } = CheckStatusEnum.NotFound;
    public string Description { get; set; } = string.Empty;
    public DateTime DateTime { get; set; } = DateTime.Now;
}

using Xerum.XFramework.Common.Dto;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Common.Dto;

public class LabelDto : RestEntityDto
{
    public int Version { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public PrinterLanguage Language { get; set; }
    public bool IsActive { get; set; }
}
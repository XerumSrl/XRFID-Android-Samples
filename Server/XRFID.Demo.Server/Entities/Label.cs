using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.Entities;

public class Label : AuditEntity
{
    public int Version { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public PrinterLanguage Language { get; set; }
    public bool IsActive { get; set; }
}

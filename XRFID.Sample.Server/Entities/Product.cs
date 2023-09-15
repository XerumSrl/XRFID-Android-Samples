using Xerum.XFramework.Common.Enums;

namespace XRFID.Sample.Server.Entities;

public class Product : AuditEntity
{
    public string Description { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Note { get; set; }

    public string? Attrib1 { get; set; }

    public string? Attrib2 { get; set; }

    public string? Attrib3 { get; set; }

    public ItemStatus Status { get; set; }

    public string Epc { get; set; } = string.Empty;

    public int ContentQuantity { get; set; }

    public string SerialNumber { get; set; } = string.Empty;

    public string? OrderReference { get; set; }
}

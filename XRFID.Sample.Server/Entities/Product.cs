namespace XRFID.Sample.Server.Entities;

public class Product : AuditEntity
{
    public string Description { get; set; } = string.Empty;

    public string? Note { get; set; }

    public string Epc { get; set; } = string.Empty;

    public int ContentQuantity { get; set; }

    public string SerialNumber { get; set; } = string.Empty;

    public Guid SKUId { get; set; }

    public SKU SKU { get; set; } = new SKU();
}

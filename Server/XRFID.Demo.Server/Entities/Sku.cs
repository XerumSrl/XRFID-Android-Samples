namespace XRFID.Demo.Server.Entities;

public class Sku : AuditEntity
{
    public string Description { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new List<Product>();

    public DateTime EffectivityStart { get; set; } = DateTime.Now.Date;
    public DateTime EffectivityEnd { get; set; } = DateTime.Now.Date + TimeSpan.FromDays(180);//180 days validity seems an ok default
}

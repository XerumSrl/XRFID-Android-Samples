using System.ComponentModel.DataAnnotations;

namespace XRFID.Demo.Server.ViewModels;

public class ProductModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? CreationTime { get; set; }

    public string? Note { get; set; }

    public string Epc { get; set; } = string.Empty;

    public int ContentQuantity { get; set; }

    public string SerialNumber { get; set; } = string.Empty;

    public SkuModel? Sku { get; set; }
    public Guid? SkuId { get; set; }
    public string? SkuName { get; set; }
    public string? SkuCode { get; set; }

}
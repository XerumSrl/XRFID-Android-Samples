using System.ComponentModel.DataAnnotations;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.ViewModels;

public class SkuModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? CreationTime { get; set; }

    public DateTime? EffectivityStart { get; set; }
    public DateTime? EffectivityEnd { get; set; }

    public List<ProductModel> Products { get; set; } = new List<ProductModel>();

}
using System.ComponentModel.DataAnnotations;

namespace XRFID.Demo.Server.ViewModels;

public class AddProductModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? Note { get; set; }

    [Required]
    [RegularExpression(@"^([0-9a-fA-F]{32}|[0-9a-fA-F]{24})$", ErrorMessage = "EPC must be a valix HEX rapresentation of 24 or 32 charachers")]
    public string Epc { get; set; } = string.Empty;

    [Range(minimum: 1, maximum: int.MaxValue)]
    public int ContentQuantity { get; set; } = 1;

    [Required]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    public Guid SkuId { get; set; }
}
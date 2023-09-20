using System.ComponentModel.DataAnnotations;

namespace XRFID.Sample.Server.ViewModels;

public class AddSkuModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? ExpirationDate { get; set; }
}
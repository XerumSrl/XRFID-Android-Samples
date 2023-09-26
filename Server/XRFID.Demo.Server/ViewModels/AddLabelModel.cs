using System.ComponentModel.DataAnnotations.Schema;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.ViewModels;

public class AddLabelModel
{
    public string Name { get; set; }

    public int Version { get; set; } = 1;
    public string Description { get; set; }
    public string Content { get; set; }
    public PrinterLanguage Language { get; set; }
    public bool IsActive { get; set; }

}

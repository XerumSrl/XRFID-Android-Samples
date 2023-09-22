using System.ComponentModel.DataAnnotations;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Sample.Server.ViewModels;

public class AddPrinterModel
{
    [Required]
    public string Name { get; set; }
    public string Code { get; set; }
    public string Reference { get; set; }
    [Required]
    public string Ip { get; set; }
    [Required]
    public int Port { get; set; } = 9100;
    public string Description { get; set; }

    public string Model { get; set; }
    public string Version { get; set; }
    public string MacAddress { get; set; }
    public string SerialNumber { get; set; }
    public PrinterManufacturers Manufacturer { get; set; }
    public PrinterLanguage Language { get; set; }
    public PrinterStatus Status { get; set; }
    public WorkflowType WorkflowType { get; set; } = WorkflowType.Printer;
}

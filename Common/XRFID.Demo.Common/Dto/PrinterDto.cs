using Xerum.XFramework.Common.Dto;
using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Common.Dto;

public class PrinterDto : RestEntityDto
{
    public string Uid { get; set; }
    public string Model { get; set; }
    public string Version { get; set; }
    public LicenseStatus LicenseStatus { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    public string MacAddress { get; set; }
    public string SerialNumber { get; set; }
    public PrinterManufacturers Manufacturer { get; set; }
    public PrinterLanguage Language { get; set; }
    public PrinterStatus Status { get; set; }
    public WorkflowType WorkflowType { get; set; } = WorkflowType.Printer;

}
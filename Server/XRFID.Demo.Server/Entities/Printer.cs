using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.Entities;

public class Printer : AuditEntity
{
    public string? Model { get; set; }
    public string? Version { get; set; }
    public LicenseStatus LicenseStatus { get; set; } = LicenseStatus.VALID;
    public string Ip { get; set; }
    public int Port { get; set; }
    public string? MacAddress { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public PrinterManufacturers? Manufacturer { get; set; }
    public PrinterLanguage? Language { get; set; }
    public PrinterStatus Status { get; set; } = PrinterStatus.Connected;
    public WorkflowType WorkflowType { get; set; } = WorkflowType.Printer;

}

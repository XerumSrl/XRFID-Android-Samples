using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.ViewModels;

public class PrinterModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string Ip { get; set; }
    public int Port { get; set; }
    public string Description { get; set; }

    public PrinterManufacturers? Manufacturer { get; set; }
    public PrinterLanguage? Language { get; set; }
    public PrinterStatus Status { get; set; } = PrinterStatus.Connected;
    public WorkflowType WorkflowType { get; set; } = WorkflowType.Printer;
}

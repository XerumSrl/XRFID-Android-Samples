using Xerum.XFramework.Common.Enums;

namespace XRFID.Sample.Server.ViewModels;

public class PrinterModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string Ip { get; set; }
    public int Port { get; set; }
    public string Description { get; set; }
}

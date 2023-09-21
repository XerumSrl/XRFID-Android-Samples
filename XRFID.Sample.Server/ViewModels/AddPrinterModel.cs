using Xerum.XFramework.Common.Enums;

namespace XRFID.Sample.Server.ViewModels;

public class AddPrinterModel
{
    public string Name { get; set; }

    public string Address { get; set; }
    public int Port { get; set; }
    public string Description { get; set; }
}

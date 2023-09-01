using XRFID.Sample.Client.Views.CheckItem;
using XRFID.Sample.Client.Views.FindItem;
using XRFID.Sample.Client.Views.PageSettings;

namespace XRFID.Sample.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("checkitem/rfid", typeof(CheckItemRfidView));
        Routing.RegisterRoute("finditemrfid", typeof(FindItemRfidView));
        Routing.RegisterRoute("inventoryrapidreadsettings", typeof(InventoryRapidReadSettings));
        Routing.RegisterRoute("findsettings", typeof(FindSettings));
    }
}

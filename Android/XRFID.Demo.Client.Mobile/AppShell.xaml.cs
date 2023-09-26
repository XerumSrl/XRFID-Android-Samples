using XRFID.Demo.Client.Mobile.Views.CheckItem;
using XRFID.Demo.Client.Mobile.Views.FindItem;
using XRFID.Demo.Client.Mobile.Views.PageSettings;

namespace XRFID.Demo.Client.Mobile;

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

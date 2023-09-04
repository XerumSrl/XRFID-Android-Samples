using XRFID.Sample.Client.Mobile.Views.CheckItem;
using XRFID.Sample.Client.Mobile.Views.FindItem;
using XRFID.Sample.Client.Mobile.Views.PageSettings;

namespace XRFID.Sample.Client.Mobile;

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

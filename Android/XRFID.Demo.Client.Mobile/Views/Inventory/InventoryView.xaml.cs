using Android.OS;
using Syncfusion.Maui.ListView;
using XRFID.Demo.Client.Mobile.Data.ViewData;
using XRFID.Demo.Client.Mobile.ViewModels;
using SwipeEndedEventArgs = Syncfusion.Maui.ListView.SwipeEndedEventArgs;

namespace XRFID.Demo.Client.Mobile.Views.Inventory;

public partial class InventoryView
{
    public InventoryView(InventoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        SfListView castedSender = (SfListView)sender;
        if (e.Offset == 0)
        {
            return;
        }

        InventoryProductViewData value = e.DataItem as InventoryProductViewData;

        Task result = Shell.Current.GoToAsync("/../finditemrfid", new Dictionary<string, object> { ["Product"] = new FindProductViewData { Epc = value.Epc, Name = value.ProductName } });
        castedSender.ResetSwipeItem(true);
    }

    private void ShowInfoPage(object sender, EventArgs e)
    {
        App.Current.MainPage.DisplayAlert("Page Info", "page information will be written here", "OK");
    }

    private void GoToSettings(object sender, EventArgs e)
    {
        //modify the value of the query to modify setting name, you also need to modify the constant in the ViewModel in OnAppearing
        Shell.Current.GoToAsync("inventoryrapidreadsettings?setting=inventory");
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            bool leave = await DisplayAlert("Application Closing", "Are you sure you want to close the application?", "Yes", "No");

            if (leave)
            {
                Process.KillProcess(Process.MyPid());
            }
        });

        return true;
    }

    private void SfDataGrid_QueryRowHeight(object sender, Syncfusion.Maui.DataGrid.DataGridQueryRowHeightEventArgs e)
    {
        if (e.RowIndex != 0)
        {
            e.Height = e.GetIntrinsicRowHeight(e.RowIndex);
            e.Handled = true;
        }
    }
}


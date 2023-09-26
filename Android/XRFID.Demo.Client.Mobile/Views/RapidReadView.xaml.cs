using Android.OS;
using XRFID.Demo.Client.Mobile.ViewModels;

namespace XRFID.Demo.Client.Mobile.Views;

public partial class RapidReadView
{
    public RapidReadView(RapidReadViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    private void ShowInfoPage(object sender, EventArgs e)
    {
        App.Current.MainPage.DisplayAlert("Page Info", "page information will be written here", "OK");
    }

    private void GoToSettings(object sender, EventArgs e)
    {
        //modify the value of the query to modify setting name, also need to modify the constant in the ViewModel in OnAppearing
        Shell.Current.GoToAsync("inventoryrapidreadsettings?setting=rapidread");
    }

    protected override bool OnBackButtonPressed()
    {
        if (!IsBusy)
        {
            Dispatcher.Dispatch(async () =>
            {
                bool leave = await DisplayAlert("Application Closing", "Are you sure you want to close the application?", "Yes", "No");

                if (leave)
                {
                    Process.KillProcess(Process.MyPid());
                }
            });
        }
        return true;
    }

}
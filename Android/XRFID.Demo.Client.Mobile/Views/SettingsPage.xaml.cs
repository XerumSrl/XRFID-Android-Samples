using Android.OS;
using XRFID.Demo.Client.Mobile.ViewModels;

namespace XRFID.Demo.Client.Mobile.Views;

public partial class SettingsPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
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
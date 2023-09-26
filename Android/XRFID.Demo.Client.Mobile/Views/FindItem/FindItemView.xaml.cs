using Android.Content;
using Android.OS;
using Android.Views.InputMethods;
using XRFID.Demo.Client.Mobile.ViewModels;

namespace XRFID.Demo.Client.Mobile.Views.FindItem;

public partial class FindItemView
{


    public FindItemView(FindItemViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
    private void ShowInfoPage(object sender, EventArgs e)
    {
        App.Current.MainPage.DisplayAlert("Page Info", "page information will be written here", "OK");
    }

    void OnEntryCompleted(object sender, EventArgs e)
    {
        HideKeyboard();
    }

    private void OnClickSelect(object sender, EventArgs e)
    {
        HideKeyboard();
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

    public static void HideKeyboard()
    {
        Context context = Platform.AppContext;
        InputMethodManager inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
        if (inputMethodManager != null)
        {
            Android.App.Activity activity = Platform.CurrentActivity;
            IBinder token = activity.CurrentFocus?.WindowToken;
            inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            activity.Window.DecorView.ClearFocus();
        }
    }
}
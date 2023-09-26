using Android.Content;
using Android.OS;
using Android.Views.InputMethods;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Converters;
using System.Collections.ObjectModel;
using XRFID.Demo.Client.Mobile.Data.ViewData;
using XRFID.Demo.Client.Mobile.ViewModels;

namespace XRFID.Demo.Client.Mobile.Views.CheckItem;

public partial class CheckItemView
{
    private readonly CheckItemViewModel viewModel;

    public CheckItemView(CheckItemViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
        this.viewModel = viewModel;

        EventToCommandBehavior behavior = new EventToCommandBehavior
        {
            EventName = nameof(ListView.ItemSelected),
            EventArgsConverter = new SelectedItemEventArgsConverter()
        };
    }

    private void OnClickSelect(object sender, EventArgs e)
    {
        HideKeyboard();
    }

    private void ShowInfoPage(object sender, EventArgs e)
    {
        App.Current.MainPage.DisplayAlert("Page Info", "page information will be written here", "OK");
    }

    void OnEntryCompleted(object sender, EventArgs e)
    {
        HideKeyboard();
        string barcode = ((Entry)sender).Text;
        try
        {
            Task<ObservableCollection<TagViewData>> task = Task.Run(() => viewModel.ShipmentCreateItemsByScan(barcode));
            task.Wait();

            if (task.Result is not null && task.Result.Any())
            {
                Task result = Shell.Current.GoToAsync("rfid", new Dictionary<string, object> { ["TagsView"] = task.Result, });

                // last current page action after navigation
                if (result.IsCompletedSuccessfully)
                {

                }
                else if (!result.IsCanceled)
                {

                }
            }
        }
        catch (Exception)
        {

        }
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

    private static void HideKeyboard()
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
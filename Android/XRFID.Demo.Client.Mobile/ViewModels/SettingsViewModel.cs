using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using TinyMvvm;

namespace XRFID.Demo.Client.Mobile.ViewModels;

public class SettingsViewModel : TinyViewModel
{
    #region Constants
    private readonly Regex guidRegex = new Regex(@"^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$", RegexOptions.Compiled);
    public string GuidRegex => guidRegex.ToString();

    private readonly Regex uriRegex = new Regex(@"^https?://[0-9a-zA-Z._\-/:]*/$", RegexOptions.Compiled);
    public string UriRegex => uriRegex.ToString();

    private readonly Regex hostnameRegex = new Regex(@"^[a-zA-Z0-9\-_]{1,63}$", RegexOptions.Compiled);
    public string HostnameRegex => hostnameRegex.ToString();

    private readonly Regex apiClientRegex = new Regex(@"^[a-zA-Z0-9\-_.]*$", RegexOptions.Compiled);
    public string ApiClientRegex => apiClientRegex.ToString();

    private readonly Regex apiScopeRegex = new Regex(@"^[a-zA-Z0-9\x20]*$", RegexOptions.Compiled);
    public string ApiScopeRegex => apiScopeRegex.ToString();
    #endregion

    #region BoundProps
    private string apiEndpoint;
    public string ApiEndpoint
    {
        get => apiEndpoint;
        set => SetProperty(ref apiEndpoint, value);
    }

    private string deviceId;
    public string DeviceId
    {
        get => deviceId;
        set => SetProperty(ref deviceId, value);
    }

    private string deviceName;
    public string DeviceName
    {
        get => deviceName;
        set => SetProperty(ref deviceName, value);
    }
    #endregion

    public IRelayCommand SaveCommand { get; }

    public SettingsViewModel()
    {
        SaveCommand = new RelayCommand(SaveSettings);
    }

    private async void SaveSettings()
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        try
        {
            if (!uriRegex.IsMatch(apiEndpoint))
            {
                throw new ArgumentException("API Endpoint is not a valid URL");
            }

            if (!hostnameRegex.IsMatch(deviceName))
            {
                throw new ArgumentException("Device Name does not conform to the hostname ruleset");
            }
        }
        catch (ArgumentException ex)
        {
            SnackbarOptions snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.WhiteSmoke,
                TextColor = Colors.Red,
                ActionButtonTextColor = Colors.Black,
                CornerRadius = new CornerRadius(10)
            };

            string btntext = "OK";
            TimeSpan duration = TimeSpan.FromSeconds(5);

            await Snackbar.Make(ex.Message, null, btntext, duration, snackbarOptions).Show(cancellationTokenSource.Token);
            return;
        }

        Preferences.Default.Set("device_name", DeviceName);
        Preferences.Default.Set("api_endpoint", ApiEndpoint);

        await Toast.Make("success, reboot application for changes to take effect", ToastDuration.Long).Show(cancellationTokenSource.Token);
    }

    public override Task OnAppearing()
    {
        DeviceName = Preferences.Default.Get<string>("device_name", string.Empty);
        if (string.IsNullOrEmpty(DeviceName))
        {
            DeviceName = DeviceInfo.Current.Name;
            Preferences.Default.Set("device_name", DeviceName);
        }

        DeviceId = Preferences.Default.Get<string>("device_id", string.Empty);
        if (string.IsNullOrEmpty(DeviceId))
        {
            DeviceId = Guid.NewGuid().ToString();
            Preferences.Default.Set("device_id", DeviceId);
        }

        ApiEndpoint = Preferences.Default.Get<string>("api_endpoint", string.Empty);
        if (string.IsNullOrEmpty(ApiEndpoint))
        {
            ApiEndpoint = "https://localhost:5001/api/";
            Preferences.Default.Set("api_endpoint", ApiEndpoint);
        }

        return base.OnAppearing();
    }
}

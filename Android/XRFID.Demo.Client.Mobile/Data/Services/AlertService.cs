using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;

namespace XRFID.Demo.Client.Mobile.Data.Services;
public class AlertService : IAlertService
{
    public AlertService()
    {
    }

    public void LongToast(string message)
    {
        Toast.Make(message, ToastDuration.Long).Show();
    }

    public void ShortToast(string message)
    {
        Toast.Make(message, ToastDuration.Short).Show();
    }
}
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using XRFID.Sample.Client.Mobile.Data.Services.Interfaces;

namespace XRFID.Sample.Client.Mobile.Data.Services;
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
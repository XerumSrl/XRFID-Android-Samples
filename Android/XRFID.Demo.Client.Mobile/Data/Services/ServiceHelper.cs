using Android.App;
using Android.Net.Wifi;
using Android.Text.Format;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;

namespace XRFID.Demo.Client.Mobile.Data.Services;
public class ServiceHelper : IServiceHelper
{
    public ServiceHelper()
    {
    }

    public string GetDeviceIp()
    {
        WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Service.WifiService);
        return Formatter.FormatIpAddress(wifiManager.ConnectionInfo.IpAddress);
    }
}

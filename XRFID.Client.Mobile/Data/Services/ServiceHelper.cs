using Android.App;
using Android.Net.Wifi;
using Android.Text.Format;
using XRFID.Sample.Client.Data.Services.Interfaces;

namespace XRFID.Sample.Client.Data.Services;
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

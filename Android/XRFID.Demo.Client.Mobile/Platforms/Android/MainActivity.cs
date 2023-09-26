using Android.App;
using Android.Content.PM;
using Android.OS;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;

namespace XRFID.Demo.Client.Mobile;
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, EMDKManager.IEMDKListener, EMDKManager.IStatusListener
{
    public EMDKManager emdkManager;
    private Symbol.XamarinEMDK.Notification.NotificationManager notificationManager;

    private IEMDKService emdkService;
    private BarcodeManager barcodeManager;

    void EMDKManager.IEMDKListener.OnClosed()
    {
        if (emdkManager != null)
        {
            emdkManager.Release();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // Clean up the objects created by EMDK manager
        if (emdkManager != null)
        {
            emdkManager.Release();
            emdkManager = null;
        }
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

    }

    protected override void OnPostCreate(Bundle savedInstanceState)
    {
        base.OnPostCreate(savedInstanceState);
        EMDKResults results = EMDKManager.GetEMDKManager(this, this);
        //WeakReferenceMessenger.Default.Send(results.StatusCode);

    }

    void EMDKManager.IEMDKListener.OnOpened(EMDKManager emdkManagerInstance)
    {
        //WeakReferenceMessenger.Default.Send("sopra");
        this.emdkManager = emdkManagerInstance;
        emdkService = MauiApplication.Current.Services.GetService<IEMDKService>();

        barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);
        emdkService.SetBarcodeManager(barcodeManager);

        try
        {
            emdkManager.GetInstanceAsync(EMDKManager.FEATURE_TYPE.Profile, this);
            emdkManager.GetInstanceAsync(EMDKManager.FEATURE_TYPE.Version, this);
            emdkManager.GetInstanceAsync(EMDKManager.FEATURE_TYPE.Notification, this);
            emdkManager.GetInstanceAsync(EMDKManager.FEATURE_TYPE.Barcode, this);

        }
        catch (Exception e)
        {
            //RunOnUiThread(() => statusTextView.Text = e.Message);
            Console.WriteLine("Exception: " + e.StackTrace);
        }
        //WeakReferenceMessenger.Default.Send("sotto");

    }

    void EMDKManager.IStatusListener.OnStatus(EMDKManager.StatusData statusData, EMDKBase emdkBase)
    {
        if (statusData.Result == EMDKResults.STATUS_CODE.Success)
        {


        }
    }






}

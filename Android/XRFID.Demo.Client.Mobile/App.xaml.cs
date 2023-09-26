using TinyMvvm;
using XRFID.Demo.Client.Mobile.Behavior;

namespace XRFID.Demo.Client.Mobile;

public partial class App : TinyApplication
{
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider
            .RegisterLicense("your_syncfusion_license"); //You need to edit the content with your syncfusion license

        InitializeComponent();

        MainPage = new AppShell();

        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
        {
#if __ANDROID__
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif __IOS__
#endif
        });
    }

    protected override async Task Initialize()
    {
        await base.Initialize();
    }
}

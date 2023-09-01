using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;
using TinyMvvm;
using XRFID.Sample.Client.Base;
using XRFID.Sample.Client.Data.Services;
using XRFID.Sample.Client.Data.Services.Interfaces;
using XRFID.Sample.Client.Helpers;
using XRFID.Sample.Client.Platforms.Android.Interfaces;
using XRFID.Sample.Client.Platforms.Android.Services;
using XRFID.Sample.Client.ViewModels;
using XRFID.Sample.Client.Views;
using XRFID.Sample.Client.Views.CheckItem;
using XRFID.Sample.Client.Views.FindItem;
using XRFID.Sample.Client.Views.Inventory;
using XRFID.Sample.Client.Views.PageSettings;

namespace XRFID.Sample.Client;
public static class MauiProgram
{
    private static EmdkService emdkControl = new EmdkService();
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseTinyMvvm();

        builder.Services.ConfigureMauiHandlers(handlers =>
         {
#if ANDROID
             handlers.AddHandler(typeof(Shell), typeof(Platforms.Android.CustomShellRenderer));
#endif
         });
        ;
        builder.Services.AddSingleton<IEMDKService, EmdkService>();
        builder.Services.AddSingleton<IBarcodeService, BarcodeService>();

        //Instance api helper for api call
        builder.Services.AddSingleton<RestApiHelper>();

        //for RFID viewmodel
        builder.Services.AddSingleton<IGeneralSettings, GeneralSettings>();
        builder.Services.AddSingleton<IAlertService, AlertService>();
        builder.Services.AddSingleton<IRFIDService, RFIDService>();

        builder.Services.AddTransient<BaseRfidViewModel>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<InventoryView>();
        builder.Services.AddTransient<InventoryViewModel>();

        builder.Services.AddTransient<CheckItemView>();
        builder.Services.AddTransient<CheckItemViewModel>();

        builder.Services.AddTransient<CheckItemRfidView>();
        builder.Services.AddTransient<CheckItemRfidViewModel>();

        builder.Services.AddTransient<RapidReadView>();
        builder.Services.AddTransient<RapidReadViewModel>();

        builder.Services.AddTransient<FindItemView>();
        builder.Services.AddTransient<FindItemViewModel>();

        builder.Services.AddTransient<FindItemRfidView>();
        builder.Services.AddTransient<FindItemRfidViewModel>();

        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<InventoryRapidReadSettings>();

        MauiApp host = builder.Build();

        ServiceHelper serviceHelper = new ServiceHelper();
        host.Services.GetService<IGeneralSettings>().Load(serviceHelper);

        return host;
    }
}

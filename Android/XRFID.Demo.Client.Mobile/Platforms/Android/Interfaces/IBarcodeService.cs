namespace XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;

public interface IBarcodeService
{
    bool IsEnabled { get; }
    void InitBarcodeReader();
    void DeinitBarcodeReader();
    void EnableBarcodeReader();
    void DisableBarcodeReader();
}
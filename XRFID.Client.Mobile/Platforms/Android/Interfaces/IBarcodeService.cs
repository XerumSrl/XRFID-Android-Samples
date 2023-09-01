namespace XRFID.Sample.Client.Platforms.Android.Interfaces;

public interface IBarcodeService
{
    bool IsEnabled { get; }
    void InitBarcodeReader();
    void DeinitBarcodeReader();
    void EnableBarcodeReader();
    void DisableBarcodeReader();
}
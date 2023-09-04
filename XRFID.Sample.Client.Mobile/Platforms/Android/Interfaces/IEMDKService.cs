using Symbol.XamarinEMDK.Barcode;

namespace XRFID.Sample.Client.Mobile.Platforms.Android.Interfaces;

public interface IEMDKService
{
    BarcodeManager GetBarcodeManager();
    void SetBarcodeManager(BarcodeManager barcodeManager);
}
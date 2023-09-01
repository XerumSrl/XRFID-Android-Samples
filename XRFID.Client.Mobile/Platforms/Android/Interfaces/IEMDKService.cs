using Symbol.XamarinEMDK.Barcode;

namespace XRFID.Sample.Client.Platforms.Android.Interfaces;

public interface IEMDKService
{
    BarcodeManager GetBarcodeManager();
    void SetBarcodeManager(BarcodeManager barcodeManager);
}
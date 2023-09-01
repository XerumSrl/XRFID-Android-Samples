using CommunityToolkit.Mvvm.Messaging.Messages;
using Symbol.XamarinEMDK.Barcode;

namespace XRFID.Sample.Client.Messages;
public class BarcodeMessage : ValueChangedMessage<IList<ScanDataCollection.ScanData>>
{

    public BarcodeMessage(IList<ScanDataCollection.ScanData> value) : base(value)
    {

    }
}

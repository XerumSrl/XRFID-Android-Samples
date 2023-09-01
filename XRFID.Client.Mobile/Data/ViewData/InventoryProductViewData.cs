using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace XRFID.Sample.Client.Data.ViewData;
public class InventoryProductViewData : ObservableObject, INotifyPropertyChanged
{
    private string productName = "N/A";
    public string ProductName
    {
        get => productName;
        set => SetProperty(ref productName, value);
    }
    private string epc = string.Empty;
    public string Epc
    {
        get => epc;
        set => SetProperty(ref epc, value);
    }
}

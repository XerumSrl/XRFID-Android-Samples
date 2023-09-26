using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace XRFID.Demo.Client.Mobile.Data.ViewData;
public class FindProductViewData : ObservableObject, INotifyPropertyChanged
{
    private string name = "";
    public string Name { get => name; set => SetProperty(ref name, value); }

    private string code = "";
    public string Code { get => code; set => SetProperty(ref code, value); }

    private string epc = "";
    public string Epc { get => epc; set => SetProperty(ref epc, value); }
}

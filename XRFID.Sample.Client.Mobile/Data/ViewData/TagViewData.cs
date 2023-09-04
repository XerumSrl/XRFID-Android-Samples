using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace XRFID.Sample.Client.Mobile.Data.ViewData;
public class TagViewData : ObservableObject, INotifyPropertyChanged
{
    public string Epc { get; set; } = string.Empty;
    private string status = string.Empty;
    public string Status { get => status; set => SetProperty(ref status, value); }
}

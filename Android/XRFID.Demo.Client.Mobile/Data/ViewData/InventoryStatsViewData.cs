using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace XRFID.Demo.Client.Mobile.Data.ViewData;
public class InventoryStatsViewData : ObservableObject, INotifyPropertyChanged
{
    private int tagRead = 0;
    public int TagRead { get => tagRead; set => SetProperty(ref tagRead, value); }

    private int productFound = 0;
    public int ProductFound { get => productFound; set => SetProperty(ref productFound, value); }

    private int unexpectedTag = 0;
    public int UnexpectedTag { get => unexpectedTag; set => SetProperty(ref unexpectedTag, value); }
}

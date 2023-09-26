using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XRFID.Demo.Client.Mobile.Data.Dto;
public class ItemExtDto : INotifyPropertyChanged
{
    public Guid Id { get; set; }

    public string Epc { get; set; }

    public string SerialNumber { get; set; }

    public string SkuCode { get; set; }

    public string Description { get; set; }

    public Guid? OrderLineId { get; set; }

    public string OrderLineDescription { get; set; }

    public bool Checked { get; set; }

    public bool CHECKED { get { return Checked; } set { Checked = value; OnPropertyChanged(); } }

    public bool UnexpectedTag { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

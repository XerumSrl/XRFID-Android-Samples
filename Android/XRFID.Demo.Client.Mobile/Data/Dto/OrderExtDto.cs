using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XRFID.Demo.Client.Mobile.Data.Dto;
public class OrderExtDto : INotifyPropertyChanged
{
    //customer
    public string CustomerReference { get; set; }

    public string CustomerName { get; set; }

    public string DestinationReference { get; set; }

    public string DestinationName { get; set; }

    //order
    public string OrderReference { get; set; }

    public Guid CustomerId { get; set; }

    public Guid Id { get; set; }

    public string Epc { get; set; }

    public string SerialNumber { get; set; }

    public string SkuCode { get; set; }

    public string Description { get; set; }

    public const string OrderLinesPropertyName = "OrderLines";

    public bool Checked { get; set; }
    public bool CHECKED { get { return Checked; } set { Checked = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

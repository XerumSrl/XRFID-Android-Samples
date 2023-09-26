using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XRFID.Demo.Client.Mobile.Messages;
public class ReaderTag : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Epc { get; set; }
    public short? Rssi { get; set; }
    public string Tid { get; set; }
    public string PC { get; set; }
    public int ReadsCount { get; set; }
    public bool Checked { get; set; }
    public DateTime FirstRead { get; set; }
    public Guid? MovementListId { get; set; }
    public Guid ReaderId { get; set; }
    public Guid? ItemId { get; set; }
    public int Status { get; set; }

    public int RDistance { get; set; }

    public int TagCount { get { return ReadsCount; } set { ReadsCount = value; OnPropertyChanged(); } }

    public short? RSSI { get { return Rssi; } set { Rssi = value; OnPropertyChanged(); } }

    public bool CHECKED { get { return Checked; } set { Checked = value; OnPropertyChanged(); } }

    public int RelativeDistance { get { return RDistance; } set { RDistance = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Timers;
using TinyMvvm;
using XRFID.Demo.Client.Mobile.Data.Dto;
using XRFID.Demo.Client.Mobile.Data.Enums;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;
using XRFID.Demo.Client.Mobile.Messages;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;
using Timer = System.Timers.Timer;

namespace XRFID.Demo.Client.Mobile.Base;

public partial class BaseRfidViewModel : TinyViewModel
{
    public bool IsRfidConnected { get => rfidService != null && rfidService.IsConnected; set => OnPropertyChanged(); }
    public bool IsRfidInputType { get => generalSettings.InputType == InputTypes.Rfid; }

    protected IRFIDService rfidService;
    protected readonly IGeneralSettings generalSettings;

    protected Dictionary<string, int> tagListDict = new Dictionary<string, int>();
    protected DateTime startime;
    protected int totalTagCount = 0;

    protected Timer aTimer;

    protected object tagreadlock = new object();


    private ObservableCollection<ReaderTag> inventoryItems = new ObservableCollection<ReaderTag>();
    public ObservableCollection<ReaderTag> InventoryItems
    {
        get => inventoryItems;
        set => SetProperty(ref inventoryItems, value);
    }

    private ObservableCollection<ItemExtDto> _loadListItems = new ObservableCollection<ItemExtDto>();
    public ObservableCollection<ItemExtDto> LoadListItems
    {
        get => _loadListItems;
        set => SetProperty(ref _loadListItems, value);
    }


    private string uniquetags = "0";
    public string UniqueTags { get => uniquetags; set => SetProperty(ref uniquetags, value); }
    private string totalTags = "0";
    public string TotalTags { get => totalTags; set { totalTags = value; OnPropertyChanged(); } }
    private string totaltime = "0";
    public string TotalTime { get => totaltime; set => SetProperty(ref totaltime, value); }
    private string foundTags = "0";
    public string FoundTags { get => foundTags; set => SetProperty(ref foundTags, value); }
    private string connectionStatus = "Not connected";
    public string ReaderConnection { get => connectionStatus; set => SetProperty(ref connectionStatus, value); }
    private bool inventoryListAvailable = false;
    public bool InventoryListAvailable { get => inventoryListAvailable; set => SetProperty(ref inventoryListAvailable, value); }
    private bool loadListAvailable = false;
    public bool LoadListAvailable { get => loadListAvailable; set => SetProperty(ref loadListAvailable, value); }

    public Command SendTagListCmd { get; set; }

    public BaseRfidViewModel(IRFIDService rfidService, IGeneralSettings generalSettings)
    {
        this.rfidService = rfidService;
        this.generalSettings = generalSettings;


        rfidService.InitRfidReader(this.generalSettings);

        //// UI for hint
        UpdateHints(generalSettings.InputType);


    }

    protected virtual void UpdateUi(string id, int count, short? rssi)
    {
        if (tagListDict.ContainsKey(id))
        {
            tagListDict[id] = tagListDict[id] + count;
            UpdateCount(id, tagListDict[id], rssi);
        }
        else
        {
            tagListDict.Add(id, count);
            UpdateList(id, count, rssi);
        }
        totalTagCount += count;
        UpdateCounts();
    }

    protected virtual void UpdateList(string tag, int count, short? rssi)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InventoryItems.Add(new ReaderTag { Epc = tag, TagCount = count, RSSI = rssi, FirstRead = DateTime.Now });
            if (LoadListItems?.Count > 0)
            {
                try
                {
                    if (LoadListItems.Count(t => t.Epc == tag && t.UnexpectedTag == false) == 1)
                    {
                        LoadListItems.FirstOrDefault(t => t.Epc == tag).CHECKED = true;
                    }
                    else
                    {
                        LoadListItems.Add(new ItemExtDto { Epc = tag, Id = Guid.NewGuid(), UnexpectedTag = true });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error: {ex.Message}");
                }

                LoadListItems.OrderBy(t => t.UnexpectedTag == false);
            }
        });
    }

    protected virtual void UpdateCount(string tag, int count, short? rssi)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ReaderTag found = InventoryItems.FirstOrDefault(x => x.Epc == tag);
            if (found != null)
            {
                found.TagCount = count;
                found.RSSI = rssi;
            }
        });
    }

    protected void UpdateCounts()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (InventoryListAvailable)
            {
                UniqueTags = tagListDict.Count.ToString();
            }
            else
            {
                FoundTags = LoadListItems.Count(i => i.CHECKED == true).ToString();
            }

            TotalTags = totalTagCount.ToString();
            TimeSpan span = DateTime.Now - startime;
            TotalTime = span.ToString("hh\\:mm\\:ss");
        });
    }

    protected virtual void SetTimer()
    {
        // Create a timer with a one second interval.
        aTimer = new Timer(1000);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

    protected virtual void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        //UpdateCounts();
    }

    #region RFID

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual void StopInventory()
    {
        rfidService.StopInventory();
        aTimer?.Stop();
        aTimer?.Dispose();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual void PerformInventory()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            tagListDict.Clear();
            InventoryItems.Clear();
        });
        totalTagCount = 0;
        startime = DateTime.Now;
        SetTimer();
        rfidService.PerformInventory();
    }

    public void ReaderConnectionEvent(bool connection)
    {
        IsRfidConnected = connection;
        UpdateHints(generalSettings.InputType);
        aTimer?.Stop();
        aTimer?.Dispose();
    }

    private void UpdateHints(InputTypes input)
    {
        if (input == InputTypes.Rfid)
        {
            ReaderConnection = IsRfidConnected ? "Connected" : "Not connected";
        }
    }

    public virtual void UpdateIn()
    {
        rfidService.TagRead += TagReadEvent;
        rfidService.TriggerEvent += HHTriggerEvent;
        rfidService.ReaderConnectionEvent += ReaderConnectionEvent;
    }
    public virtual void UpdateOut()
    {
        rfidService.TagRead -= TagReadEvent;
        rfidService.TriggerEvent -= HHTriggerEvent;
        rfidService.ReaderConnectionEvent -= ReaderConnectionEvent;
    }

    protected virtual void HHTriggerEvent(bool pressed)
    {
        if (pressed)
        {
            PerformInventory();
        }
        else
        {
            StopInventory();
        }
    }

    protected virtual void TagReadEvent(IList<ReaderTag> aryTags)
    {
        lock (tagreadlock)
        {
            foreach (ReaderTag tag in aryTags)
            {
                UpdateUi(tag.Epc, tag.TagCount, tag.Rssi);
            }
        }
    }

    #endregion

    #region IDisposable Support
    protected bool disposedValue = false; // To detect redundant calls

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (generalSettings.InputType == InputTypes.Rfid)
                {
                    UpdateOut();
                }

                disposedValue = true;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}

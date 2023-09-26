using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XRFID.Demo.Client.Mobile.Base;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;
using XRFID.Demo.Client.Mobile.Data.ViewData;
using XRFID.Demo.Client.Mobile.Messages;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;

namespace XRFID.Demo.Client.Mobile.ViewModels;

public partial class RapidReadViewModel : BaseRfidViewModel, IDisposable
{
    private short thresholdRssi;

    #region collections
    private ObservableCollection<InventoryProductViewData> productsList = new ObservableCollection<InventoryProductViewData>();
    public ObservableCollection<InventoryProductViewData> ProductsList
    {
        get => productsList;
        set => SetProperty(ref productsList, value);
    }

    private InventoryStatsViewData inventoryStats = new InventoryStatsViewData();
    public InventoryStatsViewData InventoryStats
    {
        get => inventoryStats;
        set => SetProperty(ref inventoryStats, value);
    }
    #endregion

    #region commands
    public IAsyncRelayCommand ReadCommandAsync { get; }
    public IAsyncRelayCommand ResetCommandAsync { get; }

    [RelayCommand]
    public async Task StartStopReadAsync()
    {
        IsReading = !IsReading;
        HHTriggerEvent(IsReading);
    }

    [RelayCommand]
    public async Task ResetAsync()
    {
        ProductsList.Clear();
        InventoryStats = new();
    }
    #endregion

    #region properties
    private bool autoReset = false;
    public bool AutoReset
    {
        get => autoReset;
        set => SetProperty(ref autoReset, value);
    }

    private bool isReading = false;
    public bool IsReading
    {
        get => isReading;
        set
        {
            ResetButtonEnabled = !value;
            if (value)
            {
                ReadButtonText = "Stop";
            }
            else
            {
                ReadButtonText = "Read";
            }
            SetProperty(ref isReading, value);
        }
    }

    private bool resetButtonEnabled = true;
    public bool ResetButtonEnabled
    {
        get => resetButtonEnabled;
        set
        {
            SetProperty(ref resetButtonEnabled, value);
        }
    }

    private string readButtonText = "Read";
    public string ReadButtonText
    {
        get => readButtonText;
        set
        {
            SetProperty(ref readButtonText, value);
        }
    }
    #endregion

    public RapidReadViewModel(IRFIDService rfidService, IGeneralSettings generalSettings) : base(rfidService, generalSettings)
    {
        ReadCommandAsync = new AsyncRelayCommand(StartStopReadAsync);
        ResetCommandAsync = new AsyncRelayCommand(ResetAsync);
    }

    #region view methods
    public override Task OnAppearing()
    {
        UpdateIn();
        thresholdRssi = (short)Preferences.Default.Get<int>("rapidread", -80);
        return base.OnAppearing();
    }
    public override Task OnDisappearing()
    {
        HHTriggerEvent(false);
        Dispose(); //This dispose method is in base, but due to inherithance implementation details it calls the overridden UpdateOut()
        return base.OnDisappearing();
    }
    #endregion

    #region RFID method
    public override void UpdateIn()
    {
        rfidService.TagRead += RapidTagReadEvent;
        rfidService.TriggerEvent += HHTriggerEvent;
        rfidService.ReaderConnectionEvent += ReaderConnectionEvent;

        disposedValue = false;
    }
    public override void UpdateOut()
    {
        rfidService.TagRead -= RapidTagReadEvent;
        rfidService.TriggerEvent -= HHTriggerEvent;
        rfidService.ReaderConnectionEvent -= ReaderConnectionEvent;
    }

    private void RapidTagReadEvent(IList<ReaderTag> aryTags)
    {
        lock (tagreadlock)
        {
            foreach (ReaderTag tag in aryTags)
            {
                RapidUpdateUi(tag.Epc, tag.TagCount, tag.Rssi);
            }
        }
    }

    private void RapidUpdateUi(string id, int count, short? rssi)
    {
        if (rssi < thresholdRssi)
        {
            return;
        }

        try
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
        catch (Exception)
        {
            return;
        }
    }
    #endregion

    #region custom tag methods
    protected override void HHTriggerEvent(bool pressed)
    {
        if (pressed)
        {
            if (AutoReset)
            {
                ProductsList.Clear();
                InventoryStats = new();
            }
            PerformInventory();
        }
        else
        {
            StopInventory();
        }
    }
    protected override void UpdateList(String tag, int count, short? rssi)
    {
        try
        {
            InventoryProductViewData exprod = ProductsList.FirstOrDefault(q => q.Epc == tag);
            if (exprod is not null)
            {
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ProductsList.Add(new InventoryProductViewData()
                {
                    Epc = tag,
                });
            });
            InventoryStats.ProductFound++;
        }
        catch (Exception)
        {
            return;
        }
    }

    protected override void UpdateCount(String tag, int count, short? rssi)
    {

    }
    #endregion
}
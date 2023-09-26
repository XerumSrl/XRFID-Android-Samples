using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XRFID.Demo.Client.Mobile.Base;
using XRFID.Demo.Client.Mobile.Data.Enums;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;
using XRFID.Demo.Client.Mobile.Data.ViewData;
using XRFID.Demo.Client.Mobile.Helpers;
using XRFID.Demo.Client.Mobile.Messages;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;
using XRFID.Demo.Common.Dto;

namespace XRFID.Demo.Client.Mobile.ViewModels;
public partial class InventoryViewModel : BaseRfidViewModel
{
    private Dictionary<string, int> tagDict = new Dictionary<string, int>();
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
    public IAsyncRelayCommand ResetCommandAsync { get; }

    [RelayCommand]
    public async Task ResetAsync()
    {
        StatsReset();
    }
    #endregion

    public InventoryViewModel(IRFIDService rfidService,
                              IGeneralSettings generalSettings,
                              RestApiHelper apiHelper) : base(rfidService, generalSettings)
    {
        this.apiHelper = apiHelper;

        ResetCommandAsync = new AsyncRelayCommand(ResetAsync);
        ProductsList.CollectionChanged += ProductsList_CollectionChanged;
    }

    #region view methods
    public override Task OnAppearing()
    {
        UpdateIn();
        thresholdRssi = (short)Preferences.Default.Get<int>("inventory", -80);
        return base.OnAppearing();
    }

    public override Task OnDisappearing()
    {
        Dispose(); //This dispose method is in base, but due to inherithance implementation details it calls the overridden UpdateOut()

        return base.OnDisappearing();
    }
    #endregion

    #region workflow methods
    public void StatsReset()
    {
        tagDict.Clear();
        ProductsList.Clear();
        InventoryStats.TagRead = 0;
        InventoryStats.ProductFound = 0;
        InventoryStats.UnexpectedTag = 0;
    }

    private async void ProductsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e is null || e.NewItems is null)
        {
            return;
        }
        foreach (InventoryProductViewData item in e?.NewItems)
        {
            try
            {
                ProductDto product = await apiHelper.GetProductByEpcAsync(item.Epc);
                if (product is not null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ProductsList.FirstOrDefault(q => q.Epc == product.Epc).ProductName = product.Name;
                        InventoryStats.ProductFound++;
                    });
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        InventoryStats.UnexpectedTag++;
                    });
                }
            }
            catch (Exception)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    InventoryStats.UnexpectedTag++;
                });
                return;
            }
        }
    }
    #endregion

    #region RFID method
    public override void UpdateIn()
    {
        rfidService.TagRead += NewTagReadEvent;
        rfidService.TriggerEvent += HHTriggerEvent;
        rfidService.ReaderConnectionEvent += ReaderConnectionEvent;

        disposedValue = false;
    }
    public override void UpdateOut()
    {
        rfidService.TagRead -= NewTagReadEvent;
        rfidService.TriggerEvent -= HHTriggerEvent;
        rfidService.ReaderConnectionEvent -= ReaderConnectionEvent;
    }

    private void NewTagReadEvent(IList<ReaderTag> aryTags)
    {
        lock (tagreadlock)
        {
            foreach (ReaderTag tag in aryTags)
            {
                NewtUpdateUi(tag.Epc, tag.TagCount, tag.Rssi);
            }
        }
    }

    private void NewtUpdateUi(string id, int count, short? rssi)
    {
        //check Rssi
        if (rssi < thresholdRssi)
        {
            return;
        }

        if (tagDict.ContainsKey(id))
        {
            tagDict[id] = tagDict[id] + count;
            UpdateCount(id, tagDict[id], rssi);
        }
        else
        {
            tagDict.Add(id, count);
            UpdateList(id, count, rssi);
        }
        totalTagCount += count;
        UpdateCounts();
    }
    #endregion

    #region custom tag methods
    protected override void UpdateList(String tag, int count, short? rssi)
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

            InventoryStats.TagRead++;
        });
    }
    #endregion

    #region IDisposable Support
    private bool newdisposedValue = false;
    private readonly RestApiHelper apiHelper;

    protected void Destroyer(bool disposing)
    {
        if (!newdisposedValue)
        {
            if (disposing)
            {
                if (generalSettings.InputType == InputTypes.Rfid)
                {
                    UpdateOut();
                }

                newdisposedValue = true;
            }
        }
    }
    #endregion
}
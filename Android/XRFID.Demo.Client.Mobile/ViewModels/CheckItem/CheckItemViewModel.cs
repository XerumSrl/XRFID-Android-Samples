using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using TinyMvvm;
using XRFID.Demo.Client.Mobile.Data.Enums;
using XRFID.Demo.Client.Mobile.Data.ViewData;
using XRFID.Demo.Client.Mobile.Helpers;
using XRFID.Demo.Client.Mobile.Messages;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;
using XRFID.Demo.Common.Dto;

namespace XRFID.Demo.Client.Mobile.ViewModels;
public partial class CheckItemViewModel : TinyViewModel, IDisposable
{
    private readonly IBarcodeService barcodeService;
    private readonly RestApiHelper apiHelper;

    #region collections
    private ObservableCollection<OrderViewData> orderViews = new ObservableCollection<OrderViewData>();
    public ObservableCollection<OrderViewData> OrderViews
    {
        get => orderViews;
        set => SetProperty(ref orderViews, value);
    }

    private OrderViewData itemTapped;
    public OrderViewData ItemTapped
    {
        get => itemTapped;
        set
        {
            if (itemTapped != value)
            {
                itemTapped = value;
                OnTapped(value).ConfigureAwait(false);
            }
        }
    }

    private ObservableCollection<TagViewData> tags = new ObservableCollection<TagViewData>();
    #endregion

    #region properties

    private string barcode;
    public string Barcode
    {
        get => barcode;
        set
        {
            if (barcode != value)
            {
                if (value.Length > (barcode.Length) + 1)
                {
                    barcode = value;
                    if (barcode is null || barcode == string.Empty)
                    {
                        return;
                    }

                    SearchByScan(value).ConfigureAwait(false);
                }
                else
                {
                    barcode = value;
                }

            }
        }
    }
    #endregion

    #region commands
    public IAsyncRelayCommand RefreshCommandAsync { get; }
    #endregion

    public CheckItemViewModel(IBarcodeService barcodeService, RestApiHelper apiHelper)
    {
        this.barcodeService = barcodeService;
        this.apiHelper = apiHelper;

        barcodeService.InitBarcodeReader();

        WeakReferenceMessenger.Default.Register<BarcodeMessage>(this, (r, m) =>
        {
            try
            {
                barcode = string.Empty;
                Barcode = m.Value.FirstOrDefault().Data;
            }
            catch (Exception)
            {
                return;
            }
        });

        RefreshCommandAsync = new AsyncRelayCommand(RefreshList);
        //tapCommand = new Command<ItemSelectionChangedEventArgs>(async (obj) => { await OnTapped(obj); });
    }

    #region view methods
    public override async Task OnAppearing()
    {
        barcode = string.Empty;
        Barcode = string.Empty;

        //create list
        FillLoadingTable();

        Task.Run(() => BarcodeEnable());

        await base.OnAppearing();
    }
    public override Task OnDisappearing()
    {
        barcodeService.DisableBarcodeReader();

        return base.OnDisappearing();
    }
    #endregion

    #region Workflow methods

    private async Task RefreshList()
    {
        if (!OrderViews.Any())
        {
            return;
        }
        IsBusy = true;

        try
        {
            orderViews.Clear();
            await FillLoadingTable();
            IsBusy = false;
        }
        catch (Exception)
        {
            IsBusy = false;
            return;
        }
    }

    private void BarcodeEnable()
    {
        barcodeService.EnableBarcodeReader();
    }

    private async Task FillLoadingTable()
    {
        IsBusy = true;
        try
        {
            List<LoadingUnitDto> lus = await apiHelper.GetAllLoadingUnitAsync();
            if (lus is null || !lus.Any())
            {
                IsBusy = false;
                return;
            }

            OrderViews.Clear();
            foreach (LoadingUnitDto item in lus)
            {
                OrderViews.Add(new OrderViewData()
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }
            IsBusy = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            IsBusy = false;
            return;
        }
    }

    public async Task SearchByScan(string barcode)
    {
        try
        {
            var twd = await ShipmentCreateItemsByScan(barcode);

            if (twd is not null && twd.Any())
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Shell.Current.GoToAsync("rfid", new Dictionary<string, object> { ["TagsView"] = twd, });
                });
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    public async Task<ObservableCollection<TagViewData>> ShipmentCreateItemsByScan(string barcode)
    {
        try
        {
            if (IsBusy)
            {
                throw new Exception();
            }
            IsBusy = true;

            if (barcode is null || barcode == string.Empty)
            {
                IsBusy = false;
                throw new Exception();
            }

            ReaderDto thisReader = await apiHelper.GetReaderByNameAsync(Preferences.Default.Get("device_name", string.Empty));
            if (thisReader is null)
            {
                IsBusy = false;
                throw new Exception();
            }

            //Get loading unit + items
            LoadingUnitDto lu = await apiHelper.GetLoadingUnitWithItemsAsync(barcode);
            if (lu is null)
            {
                IsBusy = false;
                throw new Exception();
            }
            else if (!lu.LoadingUnitItems.Any())
            {
                IsBusy = false;
                throw new Exception();
            }

            //Movement + items creation
            MovementDto newMov = new()
            {
                Id = Guid.NewGuid(),
                Name = $"{thisReader.Name}_{DateTime.Now}",
                Code = "",
                Reference = Guid.NewGuid().ToString(),

                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                Description = "",

                Timestamp = DateTime.Now,

                IsValid = true,
                UnexpectedItem = false,
                MissingItem = false,
                OverflowItem = false,
                IsActive = true,
                IsConsolidated = false,

                ReaderId = thisReader.Id,

                MovementItems = new List<MovementItemDto>(),
            };

            foreach (LoadingUnitItemDto lui in lu.LoadingUnitItems)
            {
                //create a movement item from loading unit item
                MovementItemDto newItem = new MovementItemDto()
                {
                    Id = Guid.NewGuid(),
                    Name = lui.Name,
                    Code = lui.Code is not null ? lui.Code : "",
                    Description = lui.Description,
                    Reference = Guid.NewGuid().ToString(),
                    Epc = lui.Epc,
                    MovementId = newMov.Id,
                    Status = lui.Status,

                    FirstRead = DateTime.Now,
                    LastRead = DateTime.Now,
                    IgnoreUntil = DateTime.Now.AddDays(1),

                    LoadingUnitItemId = lui.Id,
                };
                newMov.MovementItems.Add(newItem);
            };
            MovementDto createdMovement = await apiHelper.CreateMovementAsync(newMov);

            ObservableCollection<TagViewData> tags = new ObservableCollection<TagViewData>();
            foreach (MovementItemDto item in createdMovement.MovementItems)
            {
                tags.Add(new TagViewData
                {
                    Epc = item.Epc,
                    Status = TagViewStatus.Default.ToString(),
                });
            }

            IsBusy = false;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Shell.Current.GoToAsync("rfid", new Dictionary<string, object> { ["TagsView"] = tags, });
            });
            return tags;
        }
        catch (Exception)
        {
            IsBusy = false;
            return new ObservableCollection<TagViewData>();
        }
    }

    private async Task OnTapped(OrderViewData value)
    {
        try
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;

            OrderViewData item = value;

            MovementDto result = await CreateMovement(item);

            ObservableCollection<TagViewData> tags = new ObservableCollection<TagViewData>();
            foreach (MovementItemDto mitem in result.MovementItems)
            {
                tags.Add(new TagViewData
                {
                    Epc = mitem.Epc,
                    //Status = item.Status.ToString(),
                    Status = TagViewStatus.Default.ToString(),
                });
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Shell.Current.GoToAsync("rfid", new Dictionary<string, object> { ["TagsView"] = tags, });
            });

            IsBusy = false;

        }
        catch (Exception ex)
        {
            IsBusy = false;
            await App.Current.MainPage.DisplayAlert("Alert", ex + "" + ex.Message, "OK");
            return;
        }
    }

    private async Task<MovementDto> CreateMovement(OrderViewData order)
    {
        ReaderDto thisReader = await apiHelper.GetReaderByNameAsync(Preferences.Default.Get<string>("device_name", string.Empty));

        if (thisReader is null)
        {
            IsBusy = false;
            throw new Exception("Reader is null");
        }

        //Get all items in selected box
        List<LoadingUnitItemDto> luis = await apiHelper.GetLoadingUnitItemsByLuIdAsync(order.Id);
        if (luis is null || !luis.Any())
        {
            throw new Exception($"No items found for LoadingUnit {order.Name}");
        }

        //Movement + items creation
        MovementDto newMov = new()
        {
            Id = Guid.NewGuid(),
            Name = $"{thisReader.Name}_{DateTime.Now}",
            Code = "",
            Reference = Guid.NewGuid().ToString(),

            CreationTime = DateTime.Now,
            LastModificationTime = DateTime.Now,
            Description = "",

            Timestamp = DateTime.Now,

            IsValid = true,
            UnexpectedItem = false,
            MissingItem = false,
            OverflowItem = false,
            IsActive = true,
            IsConsolidated = false,

            ReaderId = thisReader.Id,

            MovementItems = new List<MovementItemDto>(),
        };

        foreach (LoadingUnitItemDto lui in luis)
        {
            //create a movement item from loading unit item
            MovementItemDto newItem = new MovementItemDto()
            {
                Id = Guid.NewGuid(),
                Name = lui.Name,
                Code = lui.Code is not null ? lui.Code : "",
                Description = lui.Description,
                Reference = Guid.NewGuid().ToString(),
                Epc = lui.Epc,
                MovementId = newMov.Id,
                Status = lui.Status,

                FirstRead = DateTime.Now,
                LastRead = DateTime.Now,
                IgnoreUntil = DateTime.Now.AddDays(1),

                LoadingUnitItemId = lui.Id,
            };
            newMov.MovementItems.Add(newItem);
        };
        MovementDto createdMovement = await apiHelper.CreateMovementAsync(newMov);

        return createdMovement;
    }
    #endregion

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                barcodeService.DeinitBarcodeReader();
                WeakReferenceMessenger.Default.Unregister<BarcodeMessage>(this);

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

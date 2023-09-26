using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TinyMvvm;
using XRFID.Demo.Client.Mobile.Data.Services.Interfaces;
using XRFID.Demo.Client.Mobile.Data.ViewData;
using XRFID.Demo.Client.Mobile.Helpers;
using XRFID.Demo.Common.Dto;

namespace XRFID.Demo.Client.Mobile.ViewModels;

public partial class FindItemViewModel : TinyViewModel
{
    private readonly RestApiHelper apiHelper;
    private readonly IAlertService alertService;

    #region collections
    private ObservableCollection<FindProductViewData> productList = new ObservableCollection<FindProductViewData>();
    public ObservableCollection<FindProductViewData> ProductList
    {
        get => productList;
        set => SetProperty(ref productList, value);
    }

    private FindProductViewData itemTapped;
    public FindProductViewData ItemTapped
    {
        get => itemTapped;
        set
        {
            if (itemTapped != value)
            {
                itemTapped = value;
                OnTapped(value);
            }
        }
    }
    #endregion

    #region properties
    private string searchEntry;
    public string SearchEntry
    {
        get => searchEntry;
        set => SetProperty(ref searchEntry, value);
    }

    private bool searchEntryEnabled = true;
    public bool SearchEntryEnabled
    {
        get => searchEntryEnabled;
        set => SetProperty(ref searchEntryEnabled, value);
    }
    #endregion

    #region commands
    public IAsyncRelayCommand SearchCommandAsync { get; }
    #endregion

    public FindItemViewModel(RestApiHelper apiHelper, IAlertService alertService)
    {
        this.apiHelper = apiHelper;
        this.alertService = alertService;
        SearchCommandAsync = new AsyncRelayCommand(SearchItemByNameAsync);
    }

    #region view methods
    public override Task OnDisappearing()
    {
        return base.OnDisappearing();
    }
    #endregion

    #region WorkFlow Methods
    public async Task SearchItemByNameAsync()
    {
        try
        {
            if (IsBusy || string.IsNullOrWhiteSpace(SearchEntry))
            {
                return;
            }
            IsBusy = true;
            SearchEntryEnabled = false;

            List<ProductDto> products = await apiHelper.GetProductsSearchAsync(SearchEntry);
            if (products is null || !products.Any())
            {
                SearchEntryEnabled = true;
                IsBusy = false;
                return;
            }

            ProductList.Clear();

            foreach (ProductDto prod in products)
            {
                ProductList.Add(new FindProductViewData
                {
                    Name = prod.Name,
                    Epc = prod.Epc,
                    Code = prod.Code,
                });
            }

            SearchEntryEnabled = true;
            IsBusy = false;
        }
        catch (Exception)
        {
            SearchEntryEnabled = true;
            IsBusy = false;
            return;
        }
    }

    private void OnTapped(FindProductViewData value)
    {
        FindProductViewData item = value;
        if (item is null)
        {
            return;
        }
        Task result = Shell.Current.GoToAsync("/../finditemrfid", new Dictionary<string, object> { ["Product"] = item, });
    }
    #endregion
}
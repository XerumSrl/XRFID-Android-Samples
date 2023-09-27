using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TinyMvvm;
using XRFID.Demo.Client.Mobile.Helpers;
using XRFID.Demo.Client.Mobile.Messages;
using XRFID.Demo.Client.Mobile.Platforms.Android.Interfaces;
using XRFID.Demo.Common.Dto;

namespace XRFID.Demo.Client.Mobile.ViewModels;

public partial class ScanPrintViewModel : TinyViewModel
{
    private readonly RestApiHelper restApiHelper;
    private readonly IBarcodeService barcodeService;

    public ScanPrintViewModel(RestApiHelper restApiHelper, IBarcodeService barcodeService)
    {
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

        SendCommand = new RelayCommand(Send);
        this.restApiHelper = restApiHelper;
        this.barcodeService = barcodeService;
        barcodeService.InitBarcodeReader();
    }

    private async void Send()
    {
        if (selectedLabel is null || selectedPrinter is null)
        {
            await ShowErrorSnackbar("missing Label or Printer, please select both", 10);
            return;
        }

        ProductDto product = await restApiHelper.GetProductByCodeAsync(barcode);

        if (product == null)
        {
            await ShowErrorSnackbar("could not find product", 10);
            return;
        }

        Dictionary<string, string> variables = new Dictionary<string, string>();
        variables.Add("EPC", product.Epc);
        variables.Add("barcode", barcode);

        PrintLabelDto sendLabel = new PrintLabelDto
        {
            LabelId = selectedLabel.Id,
            LabelName = selectedLabel.Name,
            PrinterId = selectedPrinter.Id,
            PrinterName = selectedPrinter.Name,
            LabelQuantity = labelQuantity,
            Variables = variables
        };

        bool res = await restApiHelper.SendPrintAsync(sendLabel);

        if (!res)
        {
            await ShowErrorSnackbar("print failed", 10);
            return;
        }
    }

    private async Task ShowErrorSnackbar(string error, int showTime)
    {
        SnackbarOptions snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.WhiteSmoke,
            TextColor = Colors.Red,
            ActionButtonTextColor = Colors.Black,
            CornerRadius = new CornerRadius(10)
        };

        string btntext = "OK";
        TimeSpan duration = TimeSpan.FromSeconds(showTime);

        await Snackbar.Make(error, null, btntext, duration, snackbarOptions).Show();
    }

    private List<PrinterDto> printerList;
    public List<PrinterDto> PrinterList { get => printerList; set => SetProperty(ref printerList, value); }

    private List<LabelDto> labelList;
    public List<LabelDto> LabelList
    {
        get => labelList;
        set => SetProperty(ref labelList, value);
    }

    private PrinterDto selectedPrinter;
    public PrinterDto SelectedPrinter
    {
        get => selectedPrinter;
        set => SetProperty(ref selectedPrinter, value);
    }

    private LabelDto selectedLabel;
    public LabelDto SelectedLabel
    {
        get => selectedLabel;
        set => SetProperty(ref selectedLabel, value);
    }

    private int labelQuantity = 1;
    public int LabelQuantity
    {
        get => labelQuantity;
        set => SetProperty(ref labelQuantity, value);
    }

    private string barcode;
    public string Barcode
    {
        get => barcode;
        set => SetProperty(ref barcode, value);
    }

    public IRelayCommand SendCommand { get; }

    public async override Task OnAppearing()
    {
        LabelList = await restApiHelper.GetLabelsAsync();
        PrinterList = await restApiHelper.GetPrintersAsync();
        Task.Run(() => { barcodeService.EnableBarcodeReader(); });



        await base.OnAppearing();
    }
    public async override Task OnDisappearing()
    {
        barcodeService.DisableBarcodeReader();
        await base.OnDisappearing();
    }
}

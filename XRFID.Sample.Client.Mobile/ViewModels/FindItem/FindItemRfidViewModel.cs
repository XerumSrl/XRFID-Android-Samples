using XRFID.Sample.Client.Mobile.Base;
using XRFID.Sample.Client.Mobile.Data.Services.Interfaces;
using XRFID.Sample.Client.Mobile.Data.ViewData;
using XRFID.Sample.Client.Mobile.Platforms.Android.Interfaces;

namespace XRFID.Sample.Client.Mobile.ViewModels;

[QueryProperty(nameof(Product), nameof(Product))]
public partial class FindItemRfidViewModel : BaseRfidViewModel
{
    #region properties
    private const int TIMER_DELAY = 5000;

    private short rssiLow = -80;
    private short rssiHigh = -30;
    private short rssiMagicNumber;//Calculate constant to sum to make rssi always positive   
    private float rssiScaleFactor;//calculte scale factor from normalized rssi values to Percentage values, this is basically the *100/maxvalue part of the proportion

    private const short PERC_LOW = 33;
    private const short PERC_HIGH = 66;

    private readonly SolidColorBrush PercLow = new SolidColorBrush(new Color(231, 29, 54)); //red //cannot be const due to c# limitations
    private readonly SolidColorBrush PercMid = new SolidColorBrush(new Color(253, 197, 0)); //yellow
    private readonly SolidColorBrush PercHigh = new SolidColorBrush(new Color(70, 200, 53)); //green?

    private Timer resetTimer;

    private FindProductViewData product = new FindProductViewData();
    public FindProductViewData Product
    {
        get => product;
        set => SetProperty(ref product, value);
    }

    private Brush color;
    public Brush Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }

    private double percent;
    public double Percent
    {
        get => percent;
        set => SetProperty(ref percent, value);
    }

    private bool isReading = false;
    #endregion

    public FindItemRfidViewModel(IRFIDService rfidService, IGeneralSettings generalSettings) : base(rfidService, generalSettings)
    {
        resetTimer = new Timer(ResetPercentage, new AutoResetEvent(false), Timeout.Infinite, Timeout.Infinite);
        Color = PercLow;

        rssiMagicNumber = (short)(rssiLow * -1);
        rssiScaleFactor = 100f / (float)(rssiHigh + rssiMagicNumber);
    }

    #region view methods
    public override Task OnAppearing()
    {
        Percent = 0;
        isReading = false;
        rssiHigh = (short)Preferences.Default.Get<int>("findsensitivity", -30);
        rssiMagicNumber = (short)(rssiLow * -1);
        rssiScaleFactor = 100f / (float)(rssiHigh + rssiMagicNumber);
        UpdateIn();
        return base.OnAppearing();
    }
    public override Task OnDisappearing()
    {
        Dispose(); //This dispose method is in base, but due to inherithance implementation details it calls the overridden UpdateOut()
        return base.OnDisappearing();
    }
    #endregion

    #region WorkFlow Methods
    public void ContinousRead(bool pressed)
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

    private void ResetPercentage(object e)
    {
        Percent = 0;
    }

    private void PercentageCalculation(short rssi)
    {
        if (rssi > rssiHigh)
        {
            rssi = rssiHigh;
        }
        else if (rssi < rssiLow)
        {
            rssi = rssiLow;
        }

        // rssi to positive
        rssi += rssiMagicNumber;

        //Calculate scaled percentage with " oneHundredPerc:100 = rssi:X "
        Percent = Math.Round(rssi * rssiScaleFactor);

        switch (Percent)
        {
            case <= PERC_LOW:
                Color = PercLow;
                break;
            case > PERC_HIGH:
                Color = PercHigh;
                break;
            default:
                Color = PercMid;
                break;
        }

    }
    #endregion

    #region RFID method
    public override void UpdateIn()
    {
        rfidService.TagRead += TagReadEvent;
        rfidService.TriggerEvent += HHTriggerEvent;
        rfidService.ReaderConnectionEvent += ReaderConnectionEvent;

        disposedValue = false;
    }
    public override void UpdateOut()
    {
        rfidService.TagRead -= TagReadEvent;
        rfidService.TriggerEvent -= HHTriggerEvent;
        rfidService.ReaderConnectionEvent -= ReaderConnectionEvent;
    }

    protected override void HHTriggerEvent(bool pressed)
    {
        if (pressed)
        {
            isReading = true;
            Task.Run(ContinousInventory);
        }
        else
        {
            isReading = false;
            StopInventory();
        }
    }

    protected override void UpdateUi(string id, int count, short? rssi)
    {
        if (Product.Epc.Equals(id))
        {
            PercentageCalculation(rssi ?? rssiLow);
            resetTimer.Change(TIMER_DELAY, Timeout.Infinite);
        }
    }

    private void ContinousInventory()
    {
        do
        {
            PerformInventory();
            Thread.Sleep(100);
            StopInventory();

        } while (isReading);
    }
    #endregion
}
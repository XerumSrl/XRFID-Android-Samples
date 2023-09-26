namespace XRFID.Demo.Client.Mobile.Views.PageSettings;

[QueryProperty(nameof(Setting), "setting")]
public partial class InventoryRapidReadSettings : ContentPage
{
    #region RssiConstants
    #region Constants
    private const int MAX_SENSITIVITY = -80;
    private const int MIN_SENSITIVITY = -40;
    #endregion

    #region TransitiveConstants 
    //automatically calculated do not touch
    private const int NORM_MAX_SENS = MAX_SENSITIVITY * -1;
    private const int NORM_MIN_SENS = MIN_SENSITIVITY * -1;
    private const double SCALE_FACTOR = (NORM_MAX_SENS - NORM_MIN_SENS) / 100d;
    #endregion 
    #endregion

    public string Setting { get; set; } = string.Empty;

    public InventoryRapidReadSettings()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private int PercentToRssi(double value)
    {
        double current;
        current = value * SCALE_FACTOR;//crush 0-100 to 0-deltamaxmin
        current += NORM_MIN_SENS;//move up to (min*-1) to 80
        current *= -1;//multiply *-1 as the rssi is in this range, but negative, high rssi=strog signal enche the multiplication instead of a transpose

        return (int)Math.Round(current);
    }

    private double RssiToPercent(int rssi)
    {
        double current;
        current = rssi * -1;
        current -= NORM_MIN_SENS;
        current /= SCALE_FACTOR;

        if (current > 100)
        {
            current = 100;
        }

        return current;
    }

    protected override void OnAppearing()
    {
        sens.Value = RssiToPercent(Preferences.Default.Get(Setting, MAX_SENSITIVITY));
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        Preferences.Default.Set(Setting, PercentToRssi(sens.Value));
        base.OnDisappearing();
    }
}
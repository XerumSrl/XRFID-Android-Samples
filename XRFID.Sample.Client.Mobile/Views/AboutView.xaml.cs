namespace XRFID.Sample.Client.Mobile.Views;

public partial class AboutView : ContentPage
{

    public AboutView()
    {
        InitializeComponent();

        string copyrightYear = "© Xerum " + DateTime.Now.Year.ToString();

        CopyrightLabel.Text = copyrightYear;
    }
}
namespace XRFID.Sample.Client.Views;

public partial class AboutView : ContentPage
{

    public AboutView()
    {
        InitializeComponent();

        string copyrightYear = "� Xerum " + DateTime.Now.Year.ToString();

        CopyrightLabel.Text = copyrightYear;
    }
}
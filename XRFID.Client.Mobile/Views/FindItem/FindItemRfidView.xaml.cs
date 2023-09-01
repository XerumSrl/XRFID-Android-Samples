using XRFID.Sample.Client.ViewModels;

namespace XRFID.Sample.Client.Views.FindItem;

public partial class FindItemRfidView
{
    private readonly FindItemRfidViewModel viewModel;

    public FindItemRfidView(FindItemRfidViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
        this.viewModel = viewModel;
    }
    private void SettingsNavigation(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("findsettings?setting=findsensitivity");
    }
}
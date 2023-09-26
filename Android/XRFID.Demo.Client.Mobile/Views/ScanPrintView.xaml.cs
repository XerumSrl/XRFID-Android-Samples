using XRFID.Demo.Client.Mobile.ViewModels;

namespace XRFID.Demo.Client.Mobile.Views;

public partial class ScanPrintView
{
    public ScanPrintView(ScanPrintViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
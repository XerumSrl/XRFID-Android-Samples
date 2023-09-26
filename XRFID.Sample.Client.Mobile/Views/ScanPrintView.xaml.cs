using XRFID.Sample.Client.Mobile.ViewModels;

namespace XRFID.Sample.Client.Mobile.Views;

public partial class ScanPrintView
{
    public ScanPrintView(ScanPrintViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
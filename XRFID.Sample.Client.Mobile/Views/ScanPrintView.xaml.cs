using XRFID.Sample.Client.Mobile.ViewModels;

namespace XRFID.Sample.Client.Mobile.Views;

public partial class ScanPrintView
{
    private readonly ScanPrintViewModel viewModel;
    public ScanPrintView()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
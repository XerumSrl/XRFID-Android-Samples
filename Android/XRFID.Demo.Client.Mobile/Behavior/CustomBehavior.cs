using Syncfusion.Maui.DataGrid;
using System.Globalization;
using System.Windows.Input;

namespace XRFID.Demo.Client.Mobile.Behavior;

// Behavior
public class CustomBehavior : Behavior<SfDataGrid>
{
    public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(CustomBehavior), null);
    public static readonly BindableProperty InputConverterProperty = BindableProperty.Create("Converter", typeof(IValueConverter), typeof(CustomBehavior), null);

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public IValueConverter Converter
    {
        get { return (IValueConverter)GetValue(InputConverterProperty); }
        set { SetValue(InputConverterProperty, value); }
    }

    public SfDataGrid AssociatedObject { get; private set; }

    protected override void OnAttachedTo(SfDataGrid bindable)
    {
        base.OnAttachedTo(bindable);
        AssociatedObject = bindable;
        bindable.BindingContextChanged += OnBindingContextChanged;
        bindable.SelectionChanged += Bindable_SelectionChanged;
    }

    private void Bindable_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
    {
        if (Command == null)
        {
            return;
        }

        object gridSelectionChangedEventArgs = Converter.Convert(e, typeof(object), null, null);
        if (Command.CanExecute(gridSelectionChangedEventArgs))
        {
            Command.Execute(gridSelectionChangedEventArgs);
        }
    }

    protected override void OnDetachingFrom(SfDataGrid bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.BindingContextChanged -= OnBindingContextChanged;
        bindable.SelectionChanged -= Bindable_SelectionChanged;
        AssociatedObject = null;
    }

    private void OnBindingContextChanged(object sender, EventArgs e)
    {
        OnBindingContextChanged();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        BindingContext = AssociatedObject.BindingContext;
    }
}

public class CustomConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

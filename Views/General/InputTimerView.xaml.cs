using System.Windows.Input;

namespace PI_AQP.Views;

public partial class InputTimerView : ContentView
{
    public static readonly BindableProperty IsEditingProperty = BindableProperty.Create(
        nameof(IsEditing),
        typeof(bool),
        typeof(InputTimerView),
        false,
        propertyChanged: ChangeLayout);
    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(TimeSpan),
        typeof(InputTimerView),
        defaultValue: TimeSpan.FromMinutes(0));
    public TimeSpan Value
    {
        get => (TimeSpan)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    private TimeSpan _flagValue;


    public static readonly BindableProperty MinValueProperty = BindableProperty.Create(
        nameof(MinValue),
        typeof(TimeSpan),
        typeof(InputTimerView),
        TimeSpan.MinValue);
    public TimeSpan MinValue
    {
        get => (TimeSpan)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
        nameof(MaxValue),
        typeof(TimeSpan),
        typeof(InputTimerView),
        TimeSpan.MaxValue);
    public TimeSpan MaxValue
    {
        get => (TimeSpan)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public static readonly BindableProperty NameProperty = BindableProperty.Create(
        nameof(Name),
        typeof(object),
        typeof(InputTimerView),
        defaultValue: "");
    public object Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty CommandSaveProperty = BindableProperty.Create(
        nameof(CommandSave),
        typeof(ICommand),
        typeof(InputTimerView),
        default(ICommand));
    public ICommand CommandSave
    {
        get => (ICommand)GetValue(CommandSaveProperty);
        set => SetValue(CommandSaveProperty, value);
    }
    public static readonly BindableProperty CommandSaveParametersProperty = BindableProperty.Create(
        nameof(CommandSaveParameters),
        typeof(ICommand),
        typeof(InputTimerView),
        default(ICommand));
    public ICommand CommandSaveParameters
    {
        get => (ICommand)GetValue(CommandSaveParametersProperty);
        set => SetValue(CommandSaveParametersProperty, value);
    }
    public InputTimerView()
    {
        InitializeComponent();
        ControlTemplate = GetTemplate(this);
    }
    private static void ChangeLayout(BindableObject bindable, object oldValue, object newValue)
    {
        InputTimerView meuItemTemplate = (InputTimerView)bindable;

        meuItemTemplate.ControlTemplate = GetTemplate(meuItemTemplate);
    }

    public static ControlTemplate GetTemplate(InputTimerView input)
    {
        return (input.IsEditing ? (ControlTemplate)input.Resources["inputNumberEdit"] : (ControlTemplate)input.Resources["inputNumber"]);
    }
    void OnEnableEditing(object sender, TappedEventArgs args)
    {
        IsEditing = true;
        _flagValue = Value;
    }
    void OnCommandSave(object sender, TappedEventArgs args)
    {
        if (CommandSave != null)
        {
            //CommandSave.Execute(this);
            IsEditing = false;
            _flagValue = Value;
        }
    }
    private void DisableEditing(object sender, EventArgs e)
    {
        IsEditing = false;

        Value = _flagValue;
    }
}
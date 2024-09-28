using System.Windows.Input;

namespace PI_AQP.Views;

public partial class InputNumberView : ContentView
{
    public static readonly BindableProperty IsEditingProperty = BindableProperty.Create(
        nameof(IsEditing),
        typeof(bool),
        typeof(InputNumberView),
        false,
        propertyChanged: ChangeLayout);
    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(int),
        typeof(InputNumberView),
        defaultValue: 0);
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    private int _flagValue = 0;


    public static readonly BindableProperty MinValueProperty = BindableProperty.Create(
        nameof(MinValue),
        typeof(int),
        typeof(InputNumberView),
        Int32.MinValue);
    public int MinValue
    {
        get => (int)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
        nameof(MaxValue),
        typeof(int),
        typeof(InputNumberView),
        Int32.MaxValue);
    public int MaxValue
    {
        get => (int)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public static readonly BindableProperty NameProperty = BindableProperty.Create(
        nameof(Name),
        typeof(object),
        typeof(InputNumberView),
        defaultValue: "");
    public object Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty UnitMeasureProperty = BindableProperty.Create(
        nameof(UnitMeasure),
        typeof(string),
        typeof(InputNumberView),
        defaultValue: "");
    public string UnitMeasure
    {
        get => (string)GetValue(UnitMeasureProperty);
        set => SetValue(UnitMeasureProperty, value);
    }

    public static readonly BindableProperty CommandSaveProperty = BindableProperty.Create(
        nameof(CommandSave),
        typeof(ICommand),
        typeof(InputNumberView),
        default(ICommand));
    public ICommand CommandSave
    {
        get => (ICommand)GetValue(CommandSaveProperty);
        set => SetValue(CommandSaveProperty, value);
    }
    public static readonly BindableProperty CommandSaveParametersProperty = BindableProperty.Create(
        nameof(CommandSaveParameters),
        typeof(ICommand),
        typeof(InputNumberView),
        default(ICommand));
    public ICommand CommandSaveParameters
    {
        get => (ICommand)GetValue(CommandSaveParametersProperty);
        set => SetValue(CommandSaveParametersProperty, value);
    }
    public InputNumberView()
    {
        InitializeComponent();
        ControlTemplate = GetTemplate(this);
    }
    private static void ChangeLayout(BindableObject bindable, object oldValue, object newValue)
    {
        InputNumberView meuItemTemplate = (InputNumberView)bindable;

        meuItemTemplate.ControlTemplate = GetTemplate(meuItemTemplate);
    }

    public static ControlTemplate GetTemplate(InputNumberView input)
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

    private void Increment(object sender, EventArgs e)
    {
        if (Value >= MaxValue)
            return;

        Value++;
    }
    private void Decrement(object sender, EventArgs e)
    {
        if (Value <= MinValue)
            return;

        Value--;
    }
}
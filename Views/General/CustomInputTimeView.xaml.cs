namespace PI_AQP.Views;

public partial class CustomInputTimeView : ContentView
{
    public static readonly BindableProperty IsEditingProperty = BindableProperty.Create(
        nameof(IsEditing),
        typeof(bool),
        typeof(CustomInputTimeView),
        false,
        propertyChanged: ChangeLayout);
    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    public static readonly BindableProperty TimeProperty =
    BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(CustomInputTimeView), default(TimeSpan),
                            propertyChanged: (bindable, oldValue, newValue) =>
                            {
                                var view = (CustomInputTimeView)bindable;
                                view.Time = (TimeSpan)newValue;
                            });

    public TimeSpan Time
    {
        get
        {
            return (TimeSpan)GetValue(TimeProperty);
        }
        set
        {


            SetValue(TimeProperty, value);
        }
    }

    public CustomInputTimeView()
    {
        InitializeComponent();
        BindingContext = this;
        ControlTemplate = GetTemplate(this);
    }
    private static void ChangeLayout(BindableObject bindable, object oldValue, object newValue)
    {
        CustomInputTimeView meuItemTemplate = (CustomInputTimeView)bindable;

        meuItemTemplate.ControlTemplate = GetTemplate(meuItemTemplate);
    }
    public static ControlTemplate GetTemplate(CustomInputTimeView input)
    {
        return (input.IsEditing ? (ControlTemplate)input.Resources["inputTime"] : (ControlTemplate)input.Resources["inputTimeDisabled"]);
    }
}
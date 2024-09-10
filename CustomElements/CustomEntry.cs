namespace PI_AQP.CustomElements
{
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            nameof(Value),
            typeof(int),
            typeof(CustomEntry),
            defaultValue: 0);
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public CustomEntry()
        {
        }
    }
}

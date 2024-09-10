using PI_AQP.Models;

namespace PI_AQP.Templates
{
    public class ListRotinasTemplate : DataTemplateSelector
    {
        public DataTemplate On { get; set; } = default!;
        public DataTemplate Off { get; set; } = default!;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((RotinasCardModel)item).isOn ? On : Off;
        }

    }
}

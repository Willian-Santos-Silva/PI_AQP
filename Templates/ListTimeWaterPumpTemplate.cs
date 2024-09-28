using PI_AQP.Models;

namespace PI_AQP.Templates
{
    public class ListTimeWaterPumpTemplate : DataTemplateSelector
    {
        public DataTemplate CanDelete { get; set; } = default!;
        public DataTemplate CanEditing { get; set; } = default!;
        public DataTemplate Default { get; set; } = default!;
        public DataTemplate Invalid { get; set; } = default!;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var timing = (TimingPumpModel)item;
            switch (timing.VisualState)
            {
                default:
                case TimingPumpModel.DEFAULT:
                    return Default;
                case TimingPumpModel.CAN_DELETE:
                    return CanDelete;
                case TimingPumpModel.CAN_EDITING:
                    return CanEditing;
                case TimingPumpModel.INVALID:
                    return Invalid;
            }
        }
    }
}

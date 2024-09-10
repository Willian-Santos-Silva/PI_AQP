using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static Java.Util.Jar.Attributes;

namespace PI_AQP.Models
{
    public class WeekDaysModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public const string NOT_SELECTED = "NOT_SELECTED";
        public const string SELECTED = "SELECTED";
        public const string DISABLED = "DISABLED";


        public string Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public DayOfWeek DayOfWeek { get; set; }



        private string _State = NOT_SELECTED;
        public string State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                }
            }
        }

        private bool _IsActive = true;
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsChecked = false;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                NotifyPropertyChanged();
            }
        }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (IsActive == false)
            {
                State = DISABLED;
            }
            else
            {
                if (IsChecked == false)
                    State = NOT_SELECTED;
                else
                    State = SELECTED;
            }

            Debug.WriteLine(string.Join(",", DayOfWeek, State, IsActive, IsChecked));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
        }
    }


    public class ButtonWeekdayTemplate : DataTemplateSelector
    {

        public DataTemplate NOT_SELECTED { get; set; } = default!;
        public DataTemplate SELECTED { get; set; } = default!;
        public DataTemplate DISABLED { get; set; } = default!;


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var timing = (WeekDaysModel)item;
            switch (timing.State)
            {
                default:
                case WeekDaysModel.NOT_SELECTED:
                    return NOT_SELECTED;
                case WeekDaysModel.SELECTED:
                    return SELECTED;
                case WeekDaysModel.DISABLED:
                    return DISABLED;
            }
        }
    }

}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PI_AQP.Models
{
    public class TimingPumpModel : INotifyPropertyChanged
    {
        public const string CAN_EDITING = "CAN_EDITING";
        public const string CAN_DELETE = "CAN_DELETE";
        public const string DEFAULT = "DEFAULT";
        public const string INVALID = "INVALID";

        private string _visualState = DEFAULT;
        public string VisualState
        {
            get => _visualState;
            set
            {
                if (_visualState != value)
                {
                    _visualState = value;
                    OnPropertyChanged();
                }
            }
        }


        public string IdRotinas { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string StartTimeString { get { return StartTime.ToString(); } }
        public string EndTimeString { get { return EndTime.ToString(); } }
        public double TempoIrrigacao { get { return EndTime.TotalMinutes - StartTime.TotalMinutes; } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

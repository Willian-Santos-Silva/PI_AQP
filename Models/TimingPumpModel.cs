namespace PI_AQP.Models
{
    public class TimingPumpModel
    {
        public const string CAN_EDITING = "CAN_EDITING";
        public const string CAN_DELETE = "CAN_DELETE";
        public const string DEFAULT = "DEFAULT";
        public string VisualState { get; set; } = DEFAULT;
        public string IdRotinas { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string StartTimeString { get { return StartTime.ToString(); } }
        public string EndTimeString { get { return EndTime.ToString(); } }
        public double TempoIrrigacao { get { return EndTime.TotalMinutes - StartTime.TotalMinutes; } }
    }
}

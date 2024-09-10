namespace Aquaponia.Domain.Entities
{
    public class SystemInformationModel
    {
        public double temperatura { get; set; }
        public double turbidade { get; set; }
        public double ph { get; set; }
        public bool status_heater { get; set; }
        public bool status_water_pump { get; set; }
        public TimeSpan remaining_time_irrigation { get; set; }
    }
}

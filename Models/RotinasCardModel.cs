using Aquaponia.DTO.Entities;
using System.Collections.ObjectModel;

namespace PI_AQP.Models
{
    public class RotinasCardModel
    {
        public string Id { get; set; }
        public TimeSpan EndNextHour { get; set; }
        public TimeSpan StartNextHour { get; set; }
        public string nextHour { get; set; } = String.Empty;
        public ObservableCollection<WeekDaysModel> daysOfTheWeek { get; set; } = default!;
        public bool isOn { get; set; } = false;
        public RotinasDTO rotinaDTO { get; set; } = default!;
    }
}

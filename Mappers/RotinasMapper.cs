using Aquaponia.DTO.Entities;
using Microsoft.Maui.Platform;
using PI_AQP.Models;
using System.Collections.ObjectModel;

namespace PI_AQP.Mapper
{
    public static class RotinasMapper
    {
        public static void ToModel(this ObservableCollection<WeekDaysModel> weekDays, ref RotinasDTO rotina)
        {
            for (int i = 0; i < weekDays.Count; i++)
            {
                WeekDaysModel weekday = weekDays.First(w => i == (int)w.DayOfWeek);
                rotina.weekdays[i] = weekday.IsChecked;
                rotina.id = weekday.Id;
            }
        }
        public static void ToModel(this ObservableCollection<TimingPumpModel> horarios, ref RotinasDTO rotina)
        {
            rotina.horarios = new List<Periodo>();
            foreach (TimingPumpModel horario in horarios)
            {
                Periodo periodo = new Periodo();
                periodo.start = Convert.ToInt16(horario.StartTime.TotalMinutes);
                periodo.end = Convert.ToInt16(horario.EndTime.TotalMinutes);
                rotina.horarios.Add(periodo);
                rotina.id = horario.IdRotinas;
            }
        }
        public static List<TimingPumpModel> ToViewModelHorarios(this RotinasDTO rotinas)
        {
            List<TimingPumpModel> horarios = new List<TimingPumpModel>();

            foreach (Periodo periodo in rotinas.horarios)
            {
                TimingPumpModel horario = new TimingPumpModel();
                horario.StartTime = TimeSpan.FromMinutes(periodo.start);
                horario.EndTime = TimeSpan.FromMinutes(periodo.end);
                horario.IdRotinas = rotinas.id;
                horario.VisualState = TimingPumpModel.CAN_DELETE;
                horarios.Add(horario);
            }
            horarios.Last().VisualState = TimingPumpModel.CAN_EDITING;
            if (horarios.Count == 1)
            {
                horarios.First().VisualState = TimingPumpModel.DEFAULT;
            }
            return horarios;
        }
        public static ObservableCollection<WeekDaysModel> ToViewModelWeekDays(this RotinasDTO model, bool[] dw)
        {
            return new ObservableCollection<WeekDaysModel>()
            {
                new WeekDaysModel { Id = model.id, Name = "D", DayOfWeek = DayOfWeek.Sunday, IsChecked = model.weekdays[(int)DayOfWeek.Sunday], IsActive = !dw[(int)DayOfWeek.Sunday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Monday, IsChecked = model.weekdays[(int)DayOfWeek.Monday], IsActive = !dw[(int)DayOfWeek.Monday] },
                new WeekDaysModel { Id = model.id, Name = "T", DayOfWeek = DayOfWeek.Tuesday, IsChecked = model.weekdays[(int)DayOfWeek.Tuesday], IsActive = !dw[(int)DayOfWeek.Tuesday]  },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Wednesday, IsChecked = model.weekdays[(int)DayOfWeek.Wednesday], IsActive = !dw[(int)DayOfWeek.Wednesday] },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Thursday, IsChecked = model.weekdays[(int)DayOfWeek.Thursday], IsActive = !dw[(int)DayOfWeek.Thursday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Friday, IsChecked = model.weekdays[(int)DayOfWeek.Friday], IsActive = !dw[(int)DayOfWeek.Friday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Saturday, IsChecked = model.weekdays[(int)DayOfWeek.Saturday], IsActive = !dw[(int)DayOfWeek.Saturday] }
            };
        }
        public static ObservableCollection<WeekDaysModel> ToViewModelWeekDays(this RotinasDTO model)
        {
            return new ObservableCollection<WeekDaysModel>()
            {
                new WeekDaysModel { Id = model.id, Name = "D", DayOfWeek = DayOfWeek.Sunday, IsChecked = model.weekdays[(int)DayOfWeek.Sunday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Monday, IsChecked = model.weekdays[(int)DayOfWeek.Monday] },
                new WeekDaysModel { Id = model.id, Name = "T", DayOfWeek = DayOfWeek.Tuesday, IsChecked = model.weekdays[(int)DayOfWeek.Tuesday] },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Wednesday, IsChecked = model.weekdays[(int)DayOfWeek.Wednesday] },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Thursday, IsChecked = model.weekdays[(int)DayOfWeek.Thursday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Friday, IsChecked = model.weekdays[(int)DayOfWeek.Friday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Saturday, IsChecked = model.weekdays[(int)DayOfWeek.Saturday] }
            };
        }


        public static List<RotinasCardModel> ToViewModelRotinasCardModel(this List<RotinasDTO> rotinas)
        {
            List<RotinasDTO> rotinas_ordering = rotinas.Classificar();

            List<RotinasCardModel> rotinasCardModels = new List<RotinasCardModel>();
            foreach (RotinasDTO rotina in rotinas_ordering)
            {
                RotinasCardModel horario = new RotinasCardModel();
                horario.Id = rotina.id;
                horario.daysOfTheWeek = rotina.ToViewModelWeekDays();

                rotina.horarios.OrderBy(p => p.start);

                Periodo? nextHour = rotina.horarios.FirstOrDefault(periodo => periodo.EstaDentroDoPeriodo());
                if (nextHour == null)
                {
                    nextHour = rotina.horarios
                                .FirstOrDefault(p => p.isNextPeriod());

                    if (nextHour == null)
                    {
                        nextHour = rotina.horarios.First();
                    }
                }

                horario.StartNextHour = TimeSpan.FromMinutes(nextHour.start);
                horario.EndNextHour = TimeSpan.FromMinutes(nextHour.end);

                horario.nextHour = $"{horario.StartNextHour.ToFormattedString("HH:mm")} à {horario.EndNextHour.ToFormattedString("HH:mm")}";
                horario.rotinaDTO = rotina;
                rotinasCardModels.Add(horario);
            }

            rotinasCardModels.First().isOn = true;
            return rotinasCardModels;
        }

        private static List<RotinasDTO> Classificar(this List<RotinasDTO> rotinas)
        {
            List<RotinasDTO> rotinasOrdering = new();


            int i = 0;
            while (i < rotinas.Count())
            {
                if (rotinas[i].weekdays[(int)DateTime.Today.DayOfWeek])
                {
                    rotinasOrdering.Add(rotinas[i]);
                    rotinas.RemoveAt(i);
                    break;
                }
                i++;
            }


            for (int w = (int)DateTime.Today.DayOfWeek; w < 7; w++)
            {
                i = 0;
                while (i < rotinas.Count())
                {
                    if (rotinas[i].weekdays[w])
                    {
                        rotinasOrdering.Add(rotinas[i]);
                        rotinas.RemoveAt(i);
                        break;
                    }
                    i++;
                }
            }


            for (int w = 0; w < (int)DateTime.Today.DayOfWeek; w++)
            {
                i = 0;
                while (i < rotinas.Count())
                {
                    if (rotinas[i].weekdays[w])
                    {
                        rotinasOrdering.Add(rotinas[i]);
                        rotinas.RemoveAt(i);
                        break;
                    }
                    i++;
                }
            }


            return rotinasOrdering;
        }
    }
}

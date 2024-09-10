using Aquaponia.DTO.Entities;
using Microsoft.Maui.Platform;
using PI_AQP.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PI_AQP.Mapper
{
    public static class RotinasMapper
    {
        public static void ToModel(this ObservableCollection<WeekDaysModel> weekDays, ref RotinasDTO rotina)
        {
            for (int i = 0; i < weekDays.Count; i++)
            {
                WeekDaysModel weekday = weekDays.First(w => i == (int)w.DayOfWeek);
                rotina.WeekDays[i] = weekday.IsChecked;
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
                new WeekDaysModel { Id = model.id, Name = "D", DayOfWeek = DayOfWeek.Sunday, IsChecked = model.WeekDays[(int)DayOfWeek.Sunday], IsActive = !dw[(int)DayOfWeek.Sunday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Monday, IsChecked = model.WeekDays[(int)DayOfWeek.Monday], IsActive = !dw[(int)DayOfWeek.Monday] },
                new WeekDaysModel { Id = model.id, Name = "T", DayOfWeek = DayOfWeek.Tuesday, IsChecked = model.WeekDays[(int)DayOfWeek.Tuesday], IsActive = !dw[(int)DayOfWeek.Tuesday]  },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Wednesday, IsChecked = model.WeekDays[(int)DayOfWeek.Wednesday], IsActive = !dw[(int)DayOfWeek.Wednesday] },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Thursday, IsChecked = model.WeekDays[(int)DayOfWeek.Thursday], IsActive = !dw[(int)DayOfWeek.Thursday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Friday, IsChecked = model.WeekDays[(int)DayOfWeek.Friday], IsActive = !dw[(int)DayOfWeek.Friday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Saturday, IsChecked = model.WeekDays[(int)DayOfWeek.Saturday], IsActive = !dw[(int)DayOfWeek.Saturday] }
            };
        }
        public static ObservableCollection<WeekDaysModel> ToViewModelWeekDays(this RotinasDTO model)
        {
            return new ObservableCollection<WeekDaysModel>()
            {
                new WeekDaysModel { Id = model.id, Name = "D", DayOfWeek = DayOfWeek.Sunday, IsChecked = model.WeekDays[(int)DayOfWeek.Sunday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Monday, IsChecked = model.WeekDays[(int)DayOfWeek.Monday] },
                new WeekDaysModel { Id = model.id, Name = "T", DayOfWeek = DayOfWeek.Tuesday, IsChecked = model.WeekDays[(int)DayOfWeek.Tuesday] },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Wednesday, IsChecked = model.WeekDays[(int)DayOfWeek.Wednesday] },
                new WeekDaysModel { Id = model.id, Name = "Q", DayOfWeek = DayOfWeek.Thursday, IsChecked = model.WeekDays[(int)DayOfWeek.Thursday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Friday, IsChecked = model.WeekDays[(int)DayOfWeek.Friday] },
                new WeekDaysModel { Id = model.id, Name = "S", DayOfWeek = DayOfWeek.Saturday, IsChecked = model.WeekDays[(int)DayOfWeek.Saturday] }
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

                //Debug.WriteLine($"{new DateTimeOffset(DateTime.Today).ToUniversalTime().AddMinutes(rotina.horarios.First().start)} - {new DateTimeOffset(DateTime.Today).ToUniversalTime().AddMinutes(rotina.horarios.First().end)}");
                //Debug.WriteLine($"{DateTimeOffset.Now.ToUniversalTime()}");
                

                //Periodo? nextHour = rotina.horarios.FirstOrDefault(periodo => periodo.EstaDentroDoPeriodo());
                //if(nextHour == null)
                //{
                //    nextHour = rotina.horarios
                //                .FirstOrDefault(p => p.isNextPeriod());

                //    if (nextHour == null)
                //    {
                //        nextHour = new Periodo();
                //    }
                //}
                //horario.StartNextHour = TimeSpan.FromMinutes(nextHour.start);
                //horario.EndNextHour = TimeSpan.FromMinutes(nextHour.end);

                //long epochTicks = new DateTime(1970, 1, 1).Ticks;
                //long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);

                //Debug.WriteLine($"{horario.StartNextHour} - {horario.EndNextHour}");

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
                if (rotinas[i].WeekDays[(int)DateTime.Today.DayOfWeek])
                {
                    rotinasOrdering.Add(rotinas[i]);
                    rotinas.RemoveAt(i);
                    break;
                }
                i++;
            }
            i = 0;

            while (i < rotinas.Count())
            {
                for (int w = (int)DateTime.Today.DayOfWeek; w < 7; w++)
                {
                    if (rotinas[i].WeekDays[w])
                    {
                        rotinasOrdering.Add(rotinas[i]);
                        rotinas.RemoveAt(i);
                        break;
                    }
                }
                i++;
            }

            i = 0;
            while (i < rotinas.Count())
            {
                for (int w = 0; w < (int)DateTime.Today.DayOfWeek; w++)
                {
                    if (rotinas[i].WeekDays[w])
                    {
                        rotinasOrdering.Add(rotinas[i]);
                        rotinas.RemoveAt(i);
                        break;
                    }
                }
                i++;
            }
            return rotinasOrdering;
        }
    }
}

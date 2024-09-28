using Aquaponia.Domain.Entities;
using Aquaponia.DTO.Entities;
using PI_AQP.CustomBehavior;
using PI_AQP.Mapper;
using PI_AQP.Models;
using PI_AQP.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace PI_AQP.ViewModels.Routines
{
    public class CreateIrrigationRoutinesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public RotinasDTO _rotina { get; set; }
        public ICommand WeekdayOption_Clicked { get; private set; }
        public ICommand BtSaving_Clicked { get; private set; }
        public ICommand BtRemover_Clicked { get; private set; }
        public ICommand BtAdicionar_Clicked { get; private set; }
        public ObservableCollection<TimingPumpModel> listRotinas { get; set; } = default!;
        public ObservableCollection<WeekDaysModel> listWeekdays { get; set; } = default!;
        private int _intervalo = 10;
        public int Intervalo
        {
            get { return _intervalo; }
            set
            {
                _intervalo = value;
            }
        }
        private NumericBehavior _intervaloValidacao;
        public NumericBehavior IntervaloValidacao
        {
            get { return _intervaloValidacao; }
            set
            {
                _intervaloValidacao = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_intervaloValidacao)));
            }
        }
        public bool[] _dw;

        public bool _weekdaysIsValid = !true;
        public bool WeekdaysIsValid { 
            get { return _weekdaysIsValid; }
            set { _weekdaysIsValid = value;  OnPropertyChanged();  }
        }

        public CreateIrrigationRoutinesViewModel(bool[] dw)
        {
            _rotina = new RotinasDTO();
            _dw = dw;
            listWeekdays = _rotina.ToViewModelWeekDays(_dw);
            listRotinas = new ObservableCollection<TimingPumpModel>() { new TimingPumpModel { StartTime = TimeSpan.FromMinutes(0), EndTime = TimeSpan.FromMinutes(Intervalo) } };

            WeekdayOption_Clicked = new Command<WeekDaysModel>(SelectWeekdayOption);
            BtAdicionar_Clicked = new Command(AddTime);
            BtRemover_Clicked = new Command<TimingPumpModel>(Remove);
            BtSaving_Clicked = new Command(Saving);

            IntervaloValidacao = new NumericBehavior() {
                MinValue = 1,
                MaxValue = 1440
            };
        }

        public void SelectWeekdayOption(WeekDaysModel sender)
        {
            sender.IsChecked = !sender.IsChecked;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(listWeekdays)));
        }

        public void Remove(TimingPumpModel sender)
        {
            if (listRotinas.Count() <= 1)
                return;

            listRotinas.Remove(sender);
            if (listRotinas.Count() == 1)
            {
                listRotinas.First().VisualState = TimingPumpModel.DEFAULT;
                return;
            }
            listRotinas.Last().VisualState = TimingPumpModel.CAN_EDITING;
        }

        public void AddTime()
        {
            if (listRotinas.Count <= 0)
            {
                return;
            } 

            var lastTime = listRotinas.Last();

            if (!listIsValid()) return;


            lastTime.VisualState = TimingPumpModel.CAN_DELETE;
            listRotinas.Add(new TimingPumpModel
            {
                VisualState = TimingPumpModel.CAN_EDITING,
                StartTime = lastTime.EndTime.Add(TimeSpan.FromMinutes(_intervalo)),
                EndTime = TimeSpan.FromMinutes(Math.Min(new TimeSpan(23, 59, 0).TotalMinutes, lastTime.EndTime.Add(TimeSpan.FromMinutes(_intervalo + lastTime.TempoIrrigacao)).TotalMinutes))
            });

            IntervaloValidacao.MaxValue = (int)(new TimeSpan(23, 59, 0).TotalMinutes - listRotinas.Last().EndTime.TotalMinutes);
        }

        public async void Saving()
        {
            if (!listIsValid() || !listWeekdaysIsValid()) return;

            RotinasDTO rotina = new RotinasDTO();
            listRotinas.ToModel(ref rotina);
            listWeekdays.ToModel(ref rotina);

            BluetoothCaracteristica _rotinasCreateCaracteristica;
            DevicesBluetoothDTO _device;

            const string SERVICE_ROUTINES_UUID = "bb1db1b1-6696-4bfa-a140-8d5836e980c8";
            const string CHARACTERISTIC_CREATE_ROUTINES_UUID = "61b86e63-66bc-4b19-b178-5b5e92a67b9d";
            rotina.id = Guid.NewGuid().ToString();

            _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
            _rotinasCreateCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_ROUTINES_UUID, CHARACTERISTIC_CREATE_ROUTINES_UUID);
            await _rotinasCreateCaracteristica.StartService();
            await _rotinasCreateCaracteristica.SendMessage(JsonSerializer.Serialize(rotina));

            await Shell.Current.GoToAsync("///MainPage");
        }

        bool listIsValid()
        {
            var lastTime = listRotinas.Last();
            if (lastTime.EndTime >= new TimeSpan(23, 59, 0) || lastTime.StartTime >= lastTime.EndTime)
            {
                lastTime.VisualState = TimingPumpModel.INVALID;
                listRotinas[listRotinas.Count() - 1] = lastTime;
                return false;
            }
            if (listRotinas.Count() > 1)
            {
                if (lastTime.StartTime <= listRotinas[listRotinas.Count() - 2].EndTime || lastTime.EndTime <= listRotinas[listRotinas.Count() - 2].EndTime)
                {
                    lastTime.VisualState = TimingPumpModel.INVALID;
                    listRotinas[listRotinas.Count() - 1] = lastTime;
                    return false;
                }
            }

            return true;
        }
        bool listWeekdaysIsValid()
        {
            WeekdaysIsValid = !(listWeekdays.Where(l => l.IsActive == true && l.State == WeekDaysModel.SELECTED).Count() > 0);

            return !WeekdaysIsValid;
        }
    }
}


namespace PI_AQP.CustomBehavior
{
    public class NumericBehavior : Behavior<Entry>
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int value))
            {
                if (value < MinValue)
                    ((Entry)sender).Text = MinValue.ToString();
                else if (value > MaxValue)
                    ((Entry)sender).Text = MaxValue.ToString();
            }
            else
            {
                ((Entry)sender).Text = string.Empty;
            }
        }

    }

    public class TimePickerBehavior : Behavior<TimePicker>
    {
        public TimeSpan MinTime { get; set; }
        public TimeSpan MaxTime { get; set; }

        protected override void OnAttachedTo(TimePicker bindable)
        {
            bindable.PropertyChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(TimePicker bindable)
        {
            bindable.PropertyChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        void OnEntryTextChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != TimePicker.TimeProperty.PropertyName)
                return;

            var timePicker = sender as TimePicker;
            var selectedTime = timePicker.Time;

            if (selectedTime < MinTime)
            {
                timePicker.Time = MinTime;
            }
            else if (selectedTime > MaxTime)
            {
                timePicker.Time = MaxTime;
            }
        }
    }
}
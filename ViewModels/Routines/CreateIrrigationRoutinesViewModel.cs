using Aquaponia.Domain.Entities;
using Aquaponia.DTO.Entities;
using PI_AQP.Mapper;
using PI_AQP.Models;
using PI_AQP.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;

namespace PI_AQP.ViewModels.Routines
{
    public class CreateIrrigationRoutinesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RotinasDTO _rotina { get; set; }
        public ICommand WeekdayOption_Clicked { get; private set; }
        public ICommand BtSaving_Clicked { get; private set; }
        public ICommand BtRemover_Clicked { get; private set; }
        public ICommand BtAdicionar_Clicked { get; private set; }
        public ObservableCollection<TimingPumpModel> listRotinas { get; set; } = default!;
        public ObservableCollection<WeekDaysModel> listWeekdays { get; set; } = default!;
        public int Intervalo { get; set; } = 10;
        public bool[] _dw;
        public CreateIrrigationRoutinesViewModel(bool[] dw)
        {
            _rotina = new RotinasDTO();
            _dw = dw;
            listWeekdays = _rotina.ToViewModelWeekDays(_dw);
            listRotinas = new ObservableCollection<TimingPumpModel>() { new TimingPumpModel { StartTime = TimeSpan.FromMinutes(0), EndTime = TimeSpan.FromMinutes(Intervalo) } };

            WeekdayOption_Clicked = new Command<WeekDaysModel>(SelectWeekdayOption);
            BtAdicionar_Clicked = new Command<int>(AddTime);
            BtRemover_Clicked = new Command<TimingPumpModel>(Remove);
            BtSaving_Clicked = new Command(Saving);
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

        public void AddTime(int intervalo)
        {
            if (listRotinas.Count <= 0)
            {
                return;
            }

            var lastTime = listRotinas.Last();
            lastTime.VisualState = TimingPumpModel.CAN_DELETE;

            listRotinas.Add(new TimingPumpModel
            {
                VisualState = TimingPumpModel.CAN_EDITING,
                StartTime = lastTime.EndTime.Add(TimeSpan.FromMinutes(intervalo)),
                EndTime = lastTime.EndTime.Add(TimeSpan.FromMinutes(intervalo + lastTime.TempoIrrigacao))
            });
        }

        public async void Saving()
        {
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

    }
}

using Aquaponia.Domain.Entities;
using Aquaponia.DTO.Entities;
using PI_AQP.Mapper;
using PI_AQP.Models;
using PI_AQP.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;

namespace PI_AQP.ViewModels.Routines;

public class UpdateIrrigationRoutinesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public RotinasDTO _rotina { get; set; }
    public ICommand WeekdayOption_Clicked { get; private set; }
    public ICommand BtSaving_Clicked { get; private set; }
    public ICommand BtRemover_Clicked { get; private set; }
    public ICommand BtAdicionar_Clicked { get; private set; }
    public ObservableCollection<TimingPumpModel> listRotinas { get; set; }
    public ObservableCollection<WeekDaysModel> listWeekdays { get; set; }
    public int Intervalo { get; set; } = 10;



    public BluetoothCaracteristica _rotinasListCaracteristica;
    public DevicesBluetoothDTO _device;

    const string SERVICE_ROUTINES_UUID = "bb1db1b1-6696-4bfa-a140-8d5836e980c8";
    const string CHARACTERISTIC_UPDATE_ROUTINES_UUID = "4b9c96d4-cf69-45ca-a157-9ac8d0b7b155";

    public UpdateIrrigationRoutinesViewModel(RotinasDTO rotina, bool []dw)
    {
        _rotina = rotina;
        listWeekdays = _rotina.ToViewModelWeekDays(dw);
        listRotinas = new ObservableCollection<TimingPumpModel>(_rotina.ToViewModelHorarios());

        WeekdayOption_Clicked = new Command<WeekDaysModel>(SelectWeekdayOption);
        BtAdicionar_Clicked = new Command(AddTime);
        BtRemover_Clicked = new Command<TimingPumpModel>(Remove);
        BtSaving_Clicked = new Command(Saving);


        _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
        _rotinasListCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_ROUTINES_UUID, CHARACTERISTIC_UPDATE_ROUTINES_UUID);

        Task.Run(async () =>
        {
            try
            {

                await _rotinasListCaracteristica.StartService();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        });
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
        lastTime.VisualState = TimingPumpModel.CAN_DELETE;

        listRotinas.Add(new TimingPumpModel
        {
            VisualState = TimingPumpModel.CAN_EDITING,
            StartTime = lastTime.EndTime.Add(TimeSpan.FromMinutes(Intervalo)),
            EndTime = lastTime.EndTime.Add(TimeSpan.FromMinutes(Intervalo + lastTime.TempoIrrigacao))
        });
    }

    public async void Saving()
    {
        RotinasDTO rotina = new RotinasDTO();
        listRotinas.ToModel(ref rotina);
        listWeekdays.ToModel(ref rotina);


        await _rotinasListCaracteristica.SendMessage(JsonSerializer.Serialize(rotina));

        //_rotinasService.Update(_rotina.ToViewModelHorarios());
        await Shell.Current.GoToAsync("///MainPage");
    }

}
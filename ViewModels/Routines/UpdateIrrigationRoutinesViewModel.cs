using Aquaponia.Domain.Entities;
using Aquaponia.DTO.Entities;
using PI_AQP.CustomBehavior;
using PI_AQP.Mapper;
using PI_AQP.Models;
using PI_AQP.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace PI_AQP.ViewModels.Routines;

public class UpdateIrrigationRoutinesViewModel : INotifyPropertyChanged
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
    public ObservableCollection<TimingPumpModel> listRotinas { get; set; }
    public ObservableCollection<WeekDaysModel> listWeekdays { get; set; }

    private int _intervalo = 10;
    public int Intervalo
    {
        get { return _intervalo; }
        set {  _intervalo = value; }
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

    public bool _weekdaysIsValid = !true;
    public bool WeekdaysIsValid
    {
        get { return _weekdaysIsValid; }
        set { _weekdaysIsValid = value; OnPropertyChanged(); }
    }

    public BluetoothCaracteristica _rotinasListCaracteristica;
    public DevicesBluetoothDTO _device;

    const string SERVICE_ROUTINES_UUID = "bb1db1b1-6696-4bfa-a140-8d5836e980c8";
    const string CHARACTERISTIC_UPDATE_ROUTINES_UUID = "4b9c96d4-cf69-45ca-a157-9ac8d0b7b155";

    public UpdateIrrigationRoutinesViewModel(RotinasDTO rotina, bool[] dw)
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

        IntervaloValidacao = new NumericBehavior()
        {
            MinValue = 1,
            MaxValue = 1440
        };
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


        await _rotinasListCaracteristica.SendMessage(JsonSerializer.Serialize(rotina));

        //_rotinasService.Update(_rotina.ToViewModelHorarios());
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
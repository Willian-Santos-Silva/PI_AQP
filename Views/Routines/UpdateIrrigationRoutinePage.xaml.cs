using Aquaponia.DTO.Entities;
using PI_AQP.ViewModels.Routines;

namespace PI_AQP.Views;

[QueryProperty(nameof(rotina), "_rotina"), QueryProperty(nameof(dw), "dw")]
public partial class UpdateIrrigationRoutinePage : ContentPage
{
    private UpdateIrrigationRoutinesViewModel _irrigationRoutinesViewModel;
    private bool[] _dw = new bool[7];
    public bool[] dw
    {
        get => _dw;
        set
        {
            _dw = value;

            _irrigationRoutinesViewModel = new UpdateIrrigationRoutinesViewModel(rotina, _dw);
            BindingContext = _irrigationRoutinesViewModel;
            OnPropertyChanged();
        }
    }
    private RotinasDTO _rotina { get; set; }
    public RotinasDTO rotina
    {
        get => _rotina;
        set
        {
            _rotina = value;

            BindingContext = _irrigationRoutinesViewModel;
            OnPropertyChanged();
        }
    }

    public UpdateIrrigationRoutinePage()
    {
        InitializeComponent();
    }
}
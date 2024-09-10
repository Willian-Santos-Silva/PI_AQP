using Aquaponia.DTO.Entities;
using PI_AQP.ViewModels.Routines;

namespace PI_AQP.Views;

[QueryProperty(nameof(rotina), "_rotina"), QueryProperty(nameof(_dw), "dwUtilizados")]
public partial class UpdateIrrigationRoutinePage : ContentPage
{
    private UpdateIrrigationRoutinesViewModel _irrigationRoutinesViewModel;
    private bool [] _dw = new bool[7];
    private RotinasDTO _rotina { get; set; }
    public RotinasDTO rotina
    {
        get => _rotina;
        set
        {
            _rotina = value;
            OnPropertyChanged();

            _irrigationRoutinesViewModel = new UpdateIrrigationRoutinesViewModel(rotina, _dw);
            BindingContext = _irrigationRoutinesViewModel;
        }
    }

    public UpdateIrrigationRoutinePage()
    {
        InitializeComponent();
    }
}
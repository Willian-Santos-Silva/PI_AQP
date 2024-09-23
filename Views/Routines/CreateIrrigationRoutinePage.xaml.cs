using PI_AQP.ViewModels.Routines;
using System.Diagnostics;

namespace PI_AQP.Views;


[QueryProperty(nameof(dw), "dw")]
public partial class CreateIrrigationRoutinePage : ContentPage
{
    private CreateIrrigationRoutinesViewModel _irrigationRoutinesViewModel = default!;
    private bool[] _dw = new bool[7];
    public bool[] dw {
        get => _dw;
        set
        {
            _dw = value;

            _irrigationRoutinesViewModel = new CreateIrrigationRoutinesViewModel(_dw);
            BindingContext = _irrigationRoutinesViewModel;
            OnPropertyChanged();
        }
    }

    public CreateIrrigationRoutinePage()
    {
        InitializeComponent();
    }
}
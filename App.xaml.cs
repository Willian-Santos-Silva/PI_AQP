using Aquaponia.Domain.Interfaces;
using PI_AQP.Views;
using PI_AQP.Views.Historico;

namespace PI_AQP
{
    public partial class App : Application
    {
        readonly IDevicesConnectionService _devicesServices;
        public App(IDevicesConnectionService devicesServices)
        {
            //MainPage = new LoadingPage(_devicesServices);

            try
            {
                _devicesServices = devicesServices;

                Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
                Routing.RegisterRoute(nameof(ConfiguracoesPage), typeof(ConfiguracoesPage));
                Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
                Routing.RegisterRoute(nameof(CreateIrrigationRoutinePage), typeof(CreateIrrigationRoutinePage));
                Routing.RegisterRoute(nameof(UpdateIrrigationRoutinePage), typeof(UpdateIrrigationRoutinePage));
                Routing.RegisterRoute(nameof(HistoryTemperaturePage), typeof(HistoryTemperaturePage));
                Routing.RegisterRoute(nameof(HistoryPhPage), typeof(HistoryPhPage));

                UserAppTheme = AppTheme.Light;
                InitializeComponent();
                MainPage = new LoadingPage(_devicesServices);
            }
            catch (Exception ex)
            {
                // Log ou lide com o erro
                Console.WriteLine($"Erro ao inicializar o Shell: {ex.Message}");
            }
        }
    }
}

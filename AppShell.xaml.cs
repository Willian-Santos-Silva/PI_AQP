using PI_AQP.Views;

namespace PI_AQP
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            try
            {
                Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
                Routing.RegisterRoute(nameof(ConfiguracoesPage), typeof(ConfiguracoesPage));
                Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
                Routing.RegisterRoute(nameof(CreateIrrigationRoutinePage), typeof(CreateIrrigationRoutinePage));
                Routing.RegisterRoute(nameof(UpdateIrrigationRoutinePage), typeof(UpdateIrrigationRoutinePage));
                InitializeComponent();
            }
            catch (Exception ex)
            {
                // Log ou lide com o erro
                Console.WriteLine($"Erro ao inicializar o Shell: {ex.Message}");
            }
        }
    }
}

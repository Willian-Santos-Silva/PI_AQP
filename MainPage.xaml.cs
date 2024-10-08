using Aquaponia.Domain.Entities;
using CommunityToolkit.Maui.Views;
using PI_AQP.Models;
using PI_AQP.Services;
using PI_AQP.ViewModels;
using PI_AQP.Views;
using PI_AQP.Views.General;
using PI_AQP.Views.Historico;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;


namespace PI_AQP
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        CultureInfo culture = new CultureInfo("pt-br");
        CancellationTokenSource _cts { get; set; } = default!;

        public DevicesBluetoothDTO _device;
        private ListRotinasViewModel RotinasViewModel;

        public BluetoothCaracteristica _systemInfoCaracteristica;
        public BluetoothCaracteristica _pumpCaracteristica;


        private ObservableCollection<RotinasCardModel> _listRotinas = [];
        public ObservableCollection<RotinasCardModel> listRotinas
        {
            get => _listRotinas;
            set
            {
                if (_listRotinas == value) return;
                _listRotinas = value;
                AddNewRoutine.IsVisible = _listRotinas.Count() < 7;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(listRotinas)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
        const string CHARACTERISTIC_INFO_UUID = "eeaaf2ad-5264-47a1-a49f-5b274ab1a0fe";
        const string CHARACTERISTIC_SYSTEM_INFO_UUID = "26f8dc2f-213a-4048-a898-07eaf4f2089e";
        const string CHARACTERISTIC_PUMP_UUID = "da4a95ee-6998-40a6-ad8c-c87087e15ca6";
        Task t;
        TaskCompletionSource<bool> tComplete = new TaskCompletionSource<bool>();

        public MainPage()
        {
            _cts = new();

            RotinasViewModel = new ListRotinasViewModel();

            _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
            _systemInfoCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_INFO_UUID);
            _systemInfoCaracteristica.CallbackOnUpdate(SystemInformationEndpoint);

            _pumpCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_PUMP_UUID);
            _pumpCaracteristica.CallbackOnUpdate(Start);

            BindingContext = RotinasViewModel;


            InitializeComponent();

            listRotinas = RotinasViewModel.listRotinas;
            rotinasListView.ItemsSource = listRotinas;
        }

        private async void RestartPage()
        {
            var currentShell = Shell.Current;
            var shell = new AppShell(); // Crie uma nova instância do Shell
            Application.Current.MainPage = shell;
            if(shell != null)
                await shell.Navigation.PushAsync(new MainPage());
        }


        private async Task StartServices()
        {
            await RotinasViewModel.StartService();
            await _systemInfoCaracteristica.StartService();
            await _pumpCaracteristica.StartService();

            await _systemInfoCaracteristica.OnStartUpdate();
            await _pumpCaracteristica.OnStartUpdate();
        }

        protected override async void OnAppearing()
        {
            var loading = new PopupLoadingSpinner();
            try
            {
                this.ShowPopup(loading);

                await StartServices();
                await RotinasViewModel.Reload();

                await loading.CloseAsync();
            }
            catch (Exception e)
            {
                await loading.CloseAsync();
                bool a = await DisplayAlert("Erro", e.Message, "Recarregar", "Fechar");
                if (a)
                    RestartPage();
            }
        }

        private async void Temperature_Clicked(object sender, TappedEventArgs args)
        {
            await Shell.Current.GoToAsync(nameof(HistoryTemperaturePage));
        }
        private async void Ph_Clicked(object sender, TappedEventArgs args)
        {
            await Shell.Current.GoToAsync(nameof(HistoryPhPage));
        }

        private async void BtAdicionar_Clicked(object sender, EventArgs e)
        {
            bool[] dw = new bool[7];
            foreach (var r in listRotinas)
            {
                for (int i = 0; i < 7; i++)
                {
                    dw[i] |= r.rotinaDTO.weekdays[i];
                }
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                { "dw", dw }
            };

            await Shell.Current.GoToAsync(nameof(CreateIrrigationRoutinePage), parameters);
        }
        public void SystemInformationEndpoint(CharacteristicBluetoothDTO ble)
        {
            try
            {
                string response = Encoding.UTF8.GetString(ble.Value);
                SystemInformationDTO dto = new SystemInformationDTO();
                using var document = JsonDocument.Parse(response);
                var root = document.RootElement;

                dto.temperatura = root.GetProperty("temperatura").GetDouble();
                dto.ph = root.GetProperty("ph").GetDouble();
                dto.turbidade = root.GetProperty("ntu").GetDouble();
                dto.status_heater = root.GetProperty("status_heater").GetBoolean();
                dto.status_water_pump = root.GetProperty("status_water_pump").GetBoolean();

                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    labelTemperature.Text = $"{dto.temperatura.ToString("N1", culture)}";
                    labelPh.Text = $"{dto.ph.ToString("N1", culture)}";
                    labelTurbitity.Text = $"{dto.turbidade.ToString("N1", culture)}";
                });

            }
            catch (Exception e)
            {
                MainThread.InvokeOnMainThreadAsync (() => { DisplayAlert("Erro", e.Message, "Ok"); });
            }
        }


        public void Start(CharacteristicBluetoothDTO ble)
        {
            Debug.WriteLine("start");
            try
            {
                ResetTimer();

                string data = Encoding.ASCII.GetString(ble.Value);
                var doc = JsonDocument.Parse(data);

                bool status_pump = doc.RootElement.GetProperty("status_pump").GetBoolean();
                int duracao = doc.RootElement.GetProperty("duracao").GetInt32();
                int tempo_restante = doc.RootElement.GetProperty("tempo_restante").GetInt32();

                StartTimer(duracao, tempo_restante);
            }
            catch (Exception e)
            {
                MainThread.InvokeOnMainThreadAsync(() => { DisplayAlert("Erro", e.Message, "Ok"); });
            }
        }

        public async void StartTimer(int timing, int tempo_restante)
        {
            if (timing <= 0 || tempo_restante <= 0)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Timing.Text = "--:--:--";
                    ActionTiming.Text = "OXIGENANDO";
                    GraphicTimingPump.Invalidate();
                });
                return;
            }
            try
            {
                TimeSpan duration = TimeSpan.FromSeconds(timing);

                var timer = TimeSpan.FromSeconds(tempo_restante);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {

                    Timing.Text = timer.ToString(@"hh\:mm\:ss");
                    ActionTiming.Text = "IRRIGANDO";

                    TimingPump.PercentageValue = tempo_restante > 0.0f ? tempo_restante / (float)duration.TotalSeconds : 0.0f;

                    GraphicTimingPump.Invalidate();
                });
            }
            catch (Exception e)
            {
                bool a = await DisplayAlert("Erro", e.Message, "Recarregar", "Fechar");
                if (a)
                    RestartPage();
            }
        }
        public void ResetTimer()
        {
            _cts.Cancel();
            _cts = new();
        }

    }
}


namespace PI_AQP.CustomElements
{
    public class TimerDrawable : GraphicsView, IDrawable
    {
        public static readonly BindableProperty DurationValueProperty = BindableProperty.Create(
            nameof(DurationValue),
            typeof(TimeSpan),
            typeof(TimerDrawable));
        public TimeSpan DurationValue
        {
            get => (TimeSpan)GetValue(DurationValueProperty);
            set => SetValue(DurationValueProperty, value);
        }

        public readonly BindableProperty PercentageValueProperty = BindableProperty.Create(
            nameof(PercentageValue),
            typeof(float),
            typeof(TimerDrawable),
            0.0F,
            propertyChanged: ChangePercentage);
        public float PercentageValue
        {
            get => (float)GetValue(PercentageValueProperty);
            set => SetValue(PercentageValueProperty, value);
        }
        ICanvas _canvas;
        Microsoft.Maui.Graphics.RectF _dirtyRect;
        public void Draw(ICanvas canvas, Microsoft.Maui.Graphics.RectF dirtyRect)
        {
            _canvas = canvas;
            _dirtyRect = dirtyRect;
            DrawTiming();
        }
        private static void ChangePercentage(BindableObject DrawTiming, object oldValue, object newValue)
        {
            var timer = (TimerDrawable)DrawTiming;
            //timer.Draw(_canvas, _dirtyRect);
        }
        void DrawTiming()
        {
            float stroke = 8;
            float diametroX = _dirtyRect.Width;
            float diametro = _dirtyRect.Height - stroke;

            float centerX = 0;
            float centerY = stroke / 2;

            _canvas.StrokeSize = stroke;
            _canvas.StrokeColor = Color.FromHex("#FFFFFF");
            _canvas.StrokeLineCap = LineCap.Square;
            _canvas.DrawArc(centerX, centerY, diametro, diametro, 0, 360, false, false);


            _canvas.StrokeSize = stroke;
            _canvas.StrokeColor = Color.FromHex("#F12A85");
            _canvas.StrokeLineCap = LineCap.Round;
            _canvas.DrawArc(centerX, centerY, diametro, diametro, 90, 90 - PercentageValue * 360.0f, true, false);
        }
    }
}
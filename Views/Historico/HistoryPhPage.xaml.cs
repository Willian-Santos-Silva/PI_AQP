

using Aquaponia.Domain.Entities;
using Microcharts;
using PI_AQP.Services;
using SkiaSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace PI_AQP.Views.Historico;


public class HistoryPhDTO
{
    public long timestamp { get; set; } = 0;
    public float ph { get; set; } = -127;
    public DateTime getData()
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }
}

public partial class HistoryPhPage : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
    const string CHARACTERISTIC_GET_HIST_PH_UUID = "c496fa32-b11a-460b-8dd3-1aeeb7c7f0ae";

    Task t;
    TaskCompletionSource<bool> tComplete;

    private DevicesBluetoothDTO _device;
    private BluetoothCaracteristica _historyCaracteristica;


    List<HistoryPhDTO> historyList = default!;
    List<ChartEntry> entries { get; set; }

    private LineChart _HistoryChart;
    public LineChart HistoryChart
    {
        get { return _HistoryChart; }
        set
        {
            _HistoryChart = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HistoryChart)));
        }
    }
    private int page = 0;
    private int step = 24;
    public HistoryPhPage()
    {
        _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
        _historyCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_GET_HIST_PH_UUID);
        _historyCaracteristica.CallbackOnUpdate(GetHistoryEndpoint);


        entries = new List<ChartEntry>();
        historyList = new List<HistoryPhDTO>();

        InitializeComponent();
        BindingContext = this;


        Task.Run(async () =>
        {
            try
            {
                await _historyCaracteristica.StartService();
                await _historyCaracteristica.OnStartUpdate();
                await _historyCaracteristica.Request();
            }
            catch (Exception e)
            {
                await DisplayAlert("Erro", e.Message, "Ok");
            }
        });

    }
    public void LoadMore()
    {
        if (historyList.Count > step * page)
        {
            var newpage = historyList.Skip(step * page).Take(step); // This method means that the data of the step * page item is skipped and the data of another step is obtained.
            foreach (var item in newpage)
            {
                entries.Add(new(item.ph)
                {
                    Label = item.getData().ToString("dd/MM/yyyy HH:mm"),
                    Color = SKColor.Parse("#fff"),
                    ValueLabel = item.ph.ToString("N1"),
                    ValueLabelColor = SKColor.Parse("#fff"),
                });
            }
            page++;
        }
    }
    private void GetHistoryEndpoint(CharacteristicBluetoothDTO ble)
    {
        try
        {
            string response = Encoding.UTF8.GetString(ble.Value);
            HistoryTemperatureDTO dto = new HistoryTemperatureDTO();
            using var document = JsonDocument.Parse(response);
            var root = document.RootElement;

            JsonElement j;
            if (root.TryGetProperty("history", out j))
                historyList = JsonSerializer.Deserialize<List<HistoryPhDTO>>(j) ?? new List<HistoryPhDTO>();

            int min = root.GetProperty("min_ph").GetInt16();
            int max = root.GetProperty("max_ph").GetInt16();

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                HistoryToEntries();
                HistoryToTable();

                Min.Text = min.ToString("N1");
                Max.Text = max.ToString("N1");
            });

        }
        catch (Exception e)
        {
            MainThread.InvokeOnMainThreadAsync(() => { DisplayAlert("Erro", e.Message, "Ok"); });
        }
    }

    public void HistoryToEntries()
    {
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            historyList.OrderBy(h => h.timestamp);

            foreach (var history in historyList.TakeLast(10))
            {
                entries.Add(new(history.ph)
                {
                    Label = history.getData().ToString("dd/MM/yyyy HH:mm"),
                    Color = SKColor.Parse("#fff"),
                    ValueLabel = history.ph.ToString("N1"),
                    ValueLabelColor = SKColor.Parse("#fff"),
                });
            }

            HistoryChart = new LineChart
            {
                Entries = entries,
                ValueLabelOption = ValueLabelOption.TopOfElement,
                ValueLabelTextSize = 24,
                ValueLabelOrientation = Orientation.Horizontal,

                BackgroundColor = SKColor.FromHsl(0, 0, 0, 0),
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                YAxisTextPaint = new SKPaint() { Color = SKColor.Parse("#fff"), TextSize = 24 },
                YAxisLinesPaint = new SKPaint() { Color = SKColor.Parse("#153D8C") },
                LabelOrientation = Orientation.Vertical,
                LabelColor = SKColor.Parse("#fff"),
                LabelTextSize = 24,
                IsAnimated = false,
                PointMode = PointMode.None
            };
        });
    }

    public void HistoryToTable()
    {
        historyList.OrderByDescending(h => h.timestamp);

        for (int i = 0; i < historyList.Count(); i++)
        {
            tableHistory.Add(new Label
            {
                Text = historyList[i].getData().ToString("dd/MM/yyyy HH:mm"),
                FontFamily = "Inter",
                Padding = new Thickness(16, 16),

                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false),
            }, 0, i + 1);

            tableHistory.Add(new Label
            {
                Text = historyList[i].ph.ToString("N1"),
                HorizontalTextAlignment = TextAlignment.End,
                FontFamily = "Inter",
                Padding = new Thickness(16, 16),

                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false),
            }, 1, i + 1);
        }
    }
}
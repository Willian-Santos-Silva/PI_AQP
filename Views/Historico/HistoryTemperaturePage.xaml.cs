
using Aquaponia.Domain.Entities;
using Microcharts;
using PI_AQP.Services;
using SkiaSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace PI_AQP.Views.Historico;


public class HistoryTemperatureDTO
{
    public long timestamp { get; set; } = 0;
    public float temperatura { get; set; } = -127;
    public DateTime getData()
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToLocalTime();
    }
}

public partial class HistoryTemperaturePage : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
    const string CHARACTERISTIC_GET_HIST_TEMP_UUID = "23ffac66-d0f9-4dbb-bb10-2b92d3664760";

    public DevicesBluetoothDTO _device;
    public BluetoothCaracteristica _historyCaracteristica;


    List<HistoryTemperatureDTO> historyList = default!;
    List<ChartEntry> entries = default!;

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
    public HistoryTemperaturePage()
    {
        _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
        _historyCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_GET_HIST_TEMP_UUID);
        _historyCaracteristica.CallbackOnUpdate(GetHistoryEndpoint);

        entries = new List<ChartEntry>();
        historyList = new List<HistoryTemperatureDTO>();


        entries.Add(new(0)
        {
            Label = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
            Color = SKColor.Parse("#fff"),
            ValueLabel = "",
            ValueLabelColor = SKColor.Parse("#fff"),
        });


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
            IsAnimated = false
        };

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

    public void GetHistoryEndpoint(CharacteristicBluetoothDTO ble)
    {
        try
        {
            string response = Encoding.UTF8.GetString(ble.Value);
            HistoryTemperatureDTO dto = new HistoryTemperatureDTO();
            using var document = JsonDocument.Parse(response);
            var root = document.RootElement;

            JsonElement j;
            if (root.TryGetProperty("history", out j))
                historyList = JsonSerializer.Deserialize<List<HistoryTemperatureDTO>>(j) ?? new List<HistoryTemperatureDTO>();


            int min = root.GetProperty("min_temperatura").GetInt16();
            int max = root.GetProperty("max_temperatura").GetInt16();

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                HistoryToEntries();
                HistoryToTable();

                Min.Text = min.ToString("N1") + "°C";
                Max.Text = max.ToString("N1") + "°C";
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


            foreach (var history in historyList)
            {
                entries.Add(new(history.temperatura)
                {
                    Label = history.getData().ToString("dd/MM/yyyy HH:mm"),
                    Color = SKColor.Parse("#fff"),
                    ValueLabel = history.temperatura.ToString("N1") + "°C",
                    ValueLabelColor = SKColor.Parse("#fff"),
                });
            }

            HistoryChart.Entries = entries;
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
                Text = historyList[i].temperatura.ToString("N1") + "°C",
                HorizontalTextAlignment = TextAlignment.End,
                FontFamily = "Inter",
                Padding = new Thickness(16, 16),

                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false),
            }, 1, i + 1);
        }
    }
}
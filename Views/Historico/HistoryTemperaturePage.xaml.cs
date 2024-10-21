
using Aquaponia.Domain.Entities;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microcharts;
using PI_AQP.Services;
using SkiaSharp;
using System.Collections.ObjectModel;
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


    ObservableCollection<HistoryTemperatureDTO> historyList = default!;

    public ISeries[] Series { get; set; }
    public RectangularSection[] Sections { get; set; } = new[] {
        new RectangularSection
        {
            Yi = 7.8,
            Yj = 7.8,
            Stroke = new SolidColorPaint
            {
                Color = SKColor.Parse("#FCD526"),
                StrokeThickness = 2,
            }
        },

        new RectangularSection
        {
            Yi = 7.4,
            Yj = 7.4,
            Stroke = new SolidColorPaint
            {
                Color = SKColor.Parse("#CCAB1F"),
                StrokeThickness = 2,
            }
        }
    };
    public Axis[] XAxes { get; set; } = new[] {
        new Axis {
            Labeler = value => $"{DateTimeOffset.FromUnixTimeSeconds((long)value).DateTime.ToString("dd/MM/yyyy HH:mm")}",
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#fff")),
            TextSize = 8,
        },
    };
    public Axis[] YAxes { get; set; } = new[] {
        new Axis {
            Labeler = value => $"{value} °C",
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#fff")),
            TextSize = 8,
            SeparatorsPaint = null
        }
    };

    public HistoryTemperaturePage()
    {
        _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
        _historyCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_GET_HIST_TEMP_UUID);
        _historyCaracteristica.CallbackOnUpdate(GetHistoryEndpoint);

        historyList = new ObservableCollection<HistoryTemperatureDTO>();

        ChartSetup();
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

    void ChartSetup()
    {
        Series = new ISeries[]
        {
            new LineSeries<HistoryTemperatureDTO>
            {
                Values = historyList,
                Mapping = (sample, index) => new(sample.timestamp, sample.temperatura),
                Name = "Temperatura",
                Stroke = new SolidColorPaint(SKColor.Parse("#fff")) { StrokeThickness = 2 },
                Fill = null,
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#567df5")){ StrokeThickness = 2 },
                GeometrySize = 6,

            },
        };

        XAxes[0].MaxLimit = historyList.Max(x => x.timestamp);
        XAxes[0].MinLimit = historyList.Max(x => x.timestamp) - (24 * 60 * 60);
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
                historyList = JsonSerializer.Deserialize<ObservableCollection<HistoryTemperatureDTO>>(j) ?? new ObservableCollection<HistoryTemperatureDTO>();


            int min = root.GetProperty("min_temperatura").GetInt16();
            int max = root.GetProperty("max_temperatura").GetInt16();

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                ChartSetup();
                HistoryToTable();

                Min.Text = min.ToString("N1") + "°C";
                Max.Text = max.ToString("N1") + "°C";

                Sections[0].Yj = max;
                Sections[0].Yi = max;

                Sections[1].Yj = min;
                Sections[1].Yi = min;
            });

        }
        catch (Exception e)
        {
            MainThread.InvokeOnMainThreadAsync(() => { DisplayAlert("Erro", e.Message, "Ok"); });
        }
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


using Aquaponia.Domain.Entities;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using PI_AQP.Services;
using SkiaSharp;
using System.Text;
using System.Text.Json;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;

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

public partial class HistoryPhPage : ContentPage
{
    const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
    const string CHARACTERISTIC_GET_HIST_PH_UUID = "c496fa32-b11a-460b-8dd3-1aeeb7c7f0ae";

    const string CHARACTERISTIC_INFO_UUID = "eeaaf2ad-5264-47a1-a49f-5b274ab1a0fe";

    private DevicesBluetoothDTO _device;
    private BluetoothCaracteristica _historyCaracteristica;
    private BluetoothCaracteristica _systemInfoCaracteristica;


    ObservableCollection<HistoryPhDTO> historyList = default!;

    public ObservableCollection<ISeries> Series { get; set; }
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
            MaxLimit = 10,
            MinLimit = 0,
            AnimationsSpeed = TimeSpan.FromMilliseconds(0),
            SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100)),
        },
    };
    public Axis[] YAxes { get; set; } = new[] {
        new Axis {
            Labeler = value => $"{value}",
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#fff")),
            TextSize = 8,
            SeparatorsPaint = null,
            MaxLimit = 24,
            MinLimit = 0,
        }
    };


    public HistoryPhPage()
    {
        _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
        _historyCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_GET_HIST_PH_UUID);
        _historyCaracteristica.CallbackOnUpdate(GetHistoryEndpoint);

        _systemInfoCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_INFO_UUID);
        _systemInfoCaracteristica.CallbackOnUpdate(SystemInformationEndpoint);

        historyList = new ObservableCollection<HistoryPhDTO>();

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
        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<HistoryPhDTO>
            {
                Values = historyList,
                Mapping = (sample, index) => new(sample.timestamp, sample.ph),
                Name = "PH",
                Stroke = new SolidColorPaint(SKColor.Parse("#fff")) { StrokeThickness = 2 },
                Fill = null,
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#567df5")){ StrokeThickness = 2 },
                GeometrySize = 6,

            },
        };

        if (historyList.Count() > 0)
        {
            XAxes[0].MaxLimit = historyList.Max(x => x.timestamp);
            XAxes[0].MinLimit = historyList.Max(x => x.timestamp) - (24 * 60 * 60);
        }
    }

    private void GetHistoryEndpoint(CharacteristicBluetoothDTO ble)
    {
        try
        {
            string response = Encoding.UTF8.GetString(ble.Value);
            HistoryPhDTO dto = new HistoryPhDTO();
            using var document = JsonDocument.Parse(response);
            var root = document.RootElement;

            JsonElement j;
            if (root.TryGetProperty("history", out j))
                historyList = JsonSerializer.Deserialize<ObservableCollection<HistoryPhDTO>>(j) ?? new ObservableCollection<HistoryPhDTO>();

            int min = root.GetProperty("min_ph").GetInt16();
            int max = root.GetProperty("max_ph").GetInt16();

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Series[0].Values = historyList;

                if (historyList.Count() > 0)
                {
                    XAxes[0].MaxLimit = historyList.Max(x => x.timestamp);
                    XAxes[0].MinLimit = historyList.Max(x => x.timestamp) - (24 * 60 * 60);


                    YAxes[0].MaxLimit = 24;
                    YAxes[0].MinLimit = max;
                }

                HistoryToTable();

                Min.Text = min.ToString("N1");
                Max.Text = max.ToString("N1");

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

    public void SystemInformationEndpoint(CharacteristicBluetoothDTO ble)
    {
        try
        {
            string response = Encoding.UTF8.GetString(ble.Value);
            SystemInformationDTO dto = new SystemInformationDTO();
            using var document = JsonDocument.Parse(response);
            var root = document.RootElement;

            dto.ph = root.GetProperty("ph").GetDouble();

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                historyList.Add(new HistoryPhDTO() { ph = (float)dto.ph, timestamp = DateTimeOffset.Now.Add(TimeZoneInfo.Local.BaseUtcOffset).ToUnixTimeSeconds() });
                if (historyList.Count > 300) historyList.RemoveAt(0);
                Series[0].Values = historyList;

                if (historyList.Count() > 0)
                {
                    XAxes[0].MaxLimit = historyList.Max(x => x.timestamp);
                    XAxes[0].MinLimit = historyList.Min(x => x.timestamp);
                }

            });
        }
        catch (Exception e)
        {
            MainThread.InvokeOnMainThreadAsync(() => { DisplayAlert("Erro", e.Message, "Ok"); });
        }
    }

    public async void RealTimeMode(object sender, EventArgs args)
    {
        historyList.Clear();
        await _historyCaracteristica.OnPauseUpdate();

        await _systemInfoCaracteristica.StartService();
        await _systemInfoCaracteristica.OnStartUpdate();

        ToggleRealTimeButton();
        ToggleHistoryButton();

        HistoryChart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.None;
    }

    public async void HistoryMode(object sender, EventArgs args)
    {
        historyList.Clear();

        await _systemInfoCaracteristica.OnPauseUpdate();
        await _historyCaracteristica.OnStartUpdate();


        ToggleRealTimeButton();
        ToggleHistoryButton();

        HistoryChart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.Both;


        await _historyCaracteristica.Request();
    }

    protected void ToggleRealTimeButton()
    {
        RealTimeModeButton.IsEnabled = !RealTimeModeButton.IsEnabled;
        if (!RealTimeModeButton.IsEnabled)
        {
            RealTimeModeButton.TextColor = Color.FromHex("#1E58CC");
            RealTimeModeButton.BackgroundColor = Colors.White;
        }
        else
        {
            RealTimeModeButton.TextColor = Colors.White;
            RealTimeModeButton.BackgroundColor = Color.FromHex("#1E58CC");
        }
    }
    protected void ToggleHistoryButton()
    {
        HistoryModeButton.IsEnabled = !HistoryModeButton.IsEnabled;
        if (!HistoryModeButton.IsEnabled)
        {
            HistoryModeButton.TextColor = Color.FromHex("#1E58CC");
            HistoryModeButton.BackgroundColor = Colors.White;
        }
        else
        {
            HistoryModeButton.TextColor = Colors.White;
            HistoryModeButton.BackgroundColor = Color.FromHex("#1E58CC");
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
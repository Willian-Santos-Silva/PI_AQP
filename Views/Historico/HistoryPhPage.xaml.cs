

using Android.Health.Connect.DataTypes.Units;
using Aquaponia.Domain.Entities;
using Aquaponia.DTO.Entities;
using Microcharts;
using PI_AQP.Services;
using PI_AQP.ViewModels;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static AndroidX.Concurrent.Futures.CallbackToFutureAdapter;

namespace PI_AQP.Views.Historico;


public class HistoryPhDTO
{
    public long timestamp { get; set; } = 0;
    public float ph { get; set; } = -127;
    public DateTime getData()
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToLocalTime();
    }
}

public partial class HistoryPhPage : ContentPage
{
    const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
    const string CHARACTERISTIC_GET_HIST_PH_UUID = "c496fa32-b11a-460b-8dd3-1aeeb7c7f0ae";

    Task t;
    TaskCompletionSource<bool> tComplete;

    public DevicesBluetoothDTO _device;
    public BluetoothCaracteristica _historyCaracteristica;


    List<HistoryPhDTO> historyList = default!;
    List<ChartEntry> entries = default!;
    public HistoryPhPage()
    {
        _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
        _historyCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_GET_HIST_PH_UUID);
        _historyCaracteristica.CallbackOnUpdate(GetHistoryEndpoint);

        InitializeComponent();
        entries = new List<ChartEntry>();
        chartView.Chart = new LineChart
        {
            Entries = entries,
            ValueLabelOption = ValueLabelOption.TopOfElement,
            ValueLabelTextSize = 24,
            ValueLabelOrientation = Orientation.Horizontal,
            
            BackgroundColor = SKColor.FromHsl(0,0,0,0),
            ShowYAxisLines = true,
            ShowYAxisText = true,
            YAxisPosition = Position.Left,
            YAxisTextPaint =  new SKPaint() { Color = SKColor.Parse("#fff"), TextSize = 24 },
            YAxisLinesPaint =  new SKPaint() { Color = SKColor.Parse("#153D8C") },
            LabelOrientation = Orientation.Vertical,
            LabelColor = SKColor.Parse("#fff"),
            LabelTextSize = 24,
            IsAnimated = false
        };

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
                Debug.WriteLine(e.Message, "rotinas list");
            }
        });

    }
    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //}

    public void GetHistoryEndpoint(CharacteristicBluetoothDTO ble)
    {
        try
        {
            string response = Encoding.UTF8.GetString(ble.Value);
            HistoryTemperatureDTO dto = new HistoryTemperatureDTO();
            using var document = JsonDocument.Parse(response);
            var root = document.RootElement;

            historyList = JsonSerializer.Deserialize<List<HistoryPhDTO>>(root.GetProperty("history").GetString() ?? "[]") ?? new List<HistoryPhDTO>();

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                HistoryToEntries();
                HistoryToTable();

                Min.Text = root.GetProperty("min_ph").GetDouble().ToString("N1");
                Max.Text = root.GetProperty("max_ph").GetDouble().ToString("N1");
            });

        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    public void HistoryToEntries()
    {
        historyList.OrderBy(h => h.timestamp);

        foreach (var history in historyList)
        {
            entries.Add(new(history.ph)
            {
                Label = history.getData().ToString("dd/MM/yyyy HH:mm"),
                Color = SKColor.Parse("#fff"),
                ValueLabel = history.ph.ToString("N1") + "°C",
                ValueLabelColor = SKColor.Parse("#fff"),
            });
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
                Text = historyList[i].ph.ToString("N1") + "°C",
                HorizontalTextAlignment = TextAlignment.End,
                FontFamily = "Inter",
                Padding = new Thickness(16, 16),

                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false),
            }, 1, i + 1);
        }
    }
}
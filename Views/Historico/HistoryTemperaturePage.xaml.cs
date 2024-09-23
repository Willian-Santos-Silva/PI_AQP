

using Microcharts;
using SkiaSharp;

namespace PI_AQP.Views.Historico;

public partial class HistoryTemperaturePage : ContentPage
{
    ChartEntry[] entries = new[]
        {
            new ChartEntry(212)
            {
                Label = "UWP",
                ValueLabel = "112",
                Color = SKColor.Parse("#2c3e50")
            },
            new ChartEntry(248)
            {
                Label = "Android",
                ValueLabel = "648",
                Color = SKColor.Parse("#77d065")
            },
            new ChartEntry(128)
            {
                Label = "iOS",
                ValueLabel = "428",
                Color = SKColor.Parse("#b455b6")
            },
            new ChartEntry(514)
            {
                Label = "Forms",
                ValueLabel = "214",
                Color = SKColor.Parse("#3498db")
            }
        };
    public HistoryTemperaturePage()
    {
        InitializeComponent();

        chartView.Chart = new LineChart
        {
            Entries = entries,
            LineMode = LineMode.Straight,
            LineSize = 8,
            PointMode = PointMode.Square,
            PointSize = 18,
        }; ;
    }
}
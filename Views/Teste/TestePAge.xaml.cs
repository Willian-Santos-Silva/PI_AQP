using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microcharts.Maui;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Maui;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;

namespace PI_AQP.Views.Teste;

public class HistoryPhDTO
{
    public long timestamp { get; set; } = 0;
    public float ph { get; set; } = -127;
    public DateTime getData()
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }
}
public partial class TestePAge : ContentPage
{
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
    public Axis[] YAxes {get; set; } = new[] { 
        new Axis {
            Labeler = value => $"{value} °C",
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#fff")),
            TextSize = 8,
            SeparatorsPaint = null
        } 
    };

    public ObservableCollection<HistoryPhDTO> GenerateHistoryPhDTOCollection()
    {
        var collection = new ObservableCollection<HistoryPhDTO>();
        var random = new Random();

        for (int i = 0; i < 168; i++)
        {
            var timestamp = (long)(DateTime.UtcNow.AddHours(-350 + i).Subtract(DateTime.UnixEpoch)).TotalSeconds;
            var ph = 7 + (float)(random.NextDouble() * 2 - 1); // Generating random pH values between 6 and 8

            collection.Add(new HistoryPhDTO { timestamp = timestamp, ph = ph });
        }

        return collection;
    }
    public TestePAge()
    {
        var list = GenerateHistoryPhDTOCollection();
        Series = new ISeries[]
        {
                new LineSeries<HistoryPhDTO>
                {
                    Values = list,
                    Mapping = (sample, index) => new(sample.timestamp, sample.ph),
                    Name = "Temperatura",
                    Stroke = new SolidColorPaint(SKColor.Parse("#fff")) { StrokeThickness = 2 },
                    Fill = null,
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#567df5")){ StrokeThickness = 2 },
                    GeometrySize = 6,
                    
                },
        };

        XAxes[0].MaxLimit = list.Max(x => x.timestamp);
        XAxes[0].MinLimit = list.Max(x => x.timestamp) - (24 * 60 * 60);

        InitializeComponent();
        BindingContext = this;
        
        list = new ObservableCollection<HistoryPhDTO> ();
        list.Add(new HistoryPhDTO { ph = 101, timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() });
        list.Add(new HistoryPhDTO { ph = 10, timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() });
        XAxes[0].MaxLimit = list.Max(x => x.timestamp);
        XAxes[0].MinLimit = list.Max(x => x.timestamp) - (24 * 60 * 60);
        //list.Add(new HistoryPhDTO { ph = 101, timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() });

        Series[0].Values = list;

    }

}
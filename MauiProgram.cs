using Aquaponia.Domain.Interfaces;
using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using PI_AQP.Services;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace PI_AQP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp(true)
                .UseMicrocharts()
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Inter-ExtraBold.ttf", "InterExtraBold");
                    fonts.AddFont("Inter-Bold.ttf", "InterBold");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("Inter-Light.ttf", "InterLight");
                    fonts.AddFont("Inter-Regular.ttf", "Inter");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "GoogleMaterialIcons");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            //.UseSkiaSharp();

            StylingEntry();

            builder.Services.AddSingleton<IDevicesConnectionService, BluetoothClientService>();
            builder.Services.AddSingleton<ILocationService, PI_AQP.Platforms.Android.Services.LocationService>();
            builder.Services.AddSingleton<IBluetoothService, PI_AQP.Platforms.Android.Services.BluetoothService>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
        private static void StylingEntry()
        {
            EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
                handler.PlatformView.SetPadding(24, 16, 24, 15);

#elif IOS
                        handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                        handler.PlatformView.Layer.BorderWidth = 0;
                        handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });


            DatePickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
                handler.PlatformView.SetPadding(24, 16, 24, 15);
#elif IOS
                        handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                        handler.PlatformView.Layer.BorderWidth = 0;
                        handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });
            TimePickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Colors.Transparent.ToPlatform());
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
                handler.PlatformView.SetPadding(24, 16, 24, 15);
#elif IOS
                        handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                        handler.PlatformView.Layer.BorderWidth = 0;
                        handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });
        }
    }
}

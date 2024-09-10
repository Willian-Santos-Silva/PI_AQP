using Aquaponia.Domain.Entities;
using Aquaponia.Domain.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;

namespace PI_AQP.Views;

public partial class DiscoveryDevicesPage : ContentPage
{
    private ObservableCollection<DevicesBluetoothDTO> ListDevices = new();
    private readonly IDevicesConnectionService _devicesService;
    private readonly CancellationTokenSource cts = new();

    public DiscoveryDevicesPage(IDevicesConnectionService devicesService)
    {
        _devicesService = devicesService;

        BindingContext = this;
        InitializeComponent();

        connectionsList.ItemsSource = ListDevices;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Task.Run(StartScan, cts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cts.Cancel();
    }

    public async void RedirectToConnect(object sender, TappedEventArgs args)
    {
        StackLayout e = (StackLayout)sender;
        if (e == null)
            return;

        DevicesBluetoothDTO device = (DevicesBluetoothDTO)e.BindingContext;
        if (device == null) return;

        if (await _devicesService.TryConnectAsync(device.id))
        {
            await Toast.Make("Dispositivo conectado com sucesso!", ToastDuration.Short).Show();
            Preferences.Set("deviceID_BLE", device.id.ToString());

            Application.Current.MainPage = new AppShell();
            //await Navigation.PushAsync(new DiscoveryNetworksPage(_devicesService, device));
            return;
        }
        else
        {
            await Toast.Make("Não foi possível se conectar ao dispositivo!", ToastDuration.Short).Show();

        }
    }


    public async Task StartScan()
    {
        while (!cts.IsCancellationRequested)
        {
            var devices = await _devicesService.StartScanningAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListDevices.Clear();
                foreach (var device in devices)
                {
                    ListDevices.Add(device);
                }
            });

            await Task.Delay(500);
        }
    }
}
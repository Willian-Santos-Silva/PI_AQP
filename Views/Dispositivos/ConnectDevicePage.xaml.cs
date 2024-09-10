using Aquaponia.Domain.Interfaces;
using Aquaponia.DTO.Entities;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Diagnostics;

namespace PI_AQP.Views;

public partial class ConnectDevicePage : ContentPage
{
    public string Name { get; set; } = String.Empty;

    public string Password { get; set; } = String.Empty;
    public ConnectionWifiDTO _device;

    private readonly IWifiConnectionsServices _wifiServices = default!;
    public ConnectDevicePage(IWifiConnectionsServices wifiServices, ConnectionWifiDTO device)
    {
        _wifiServices = wifiServices;
        _device = device;

        InitializeComponent();
        BindingContext = _device;
    }
    public ConnectDevicePage(IWifiConnectionsServices wifiServices)
    {
        _wifiServices = wifiServices;
        _device = new();
        _device.ssid = "Aquapony";
        _device.password = "12345678";
        InitializeComponent();
        BindingContext = _device;
    }
    public async void BtnConnecting_Clicked(object sender, EventArgs args)
    {
        bool isConnect = false;

        try
        {
            //#if ANDROID
            isConnect = await _wifiServices.TryConnecting(_device.ssid, _device.password);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{ex.Message}", "CONEXAO WIFI");
            await Toast.Make("Erro ao tentar se conectar!", ToastDuration.Short).Show();
        }

        if (isConnect)
        {
#pragma warning disable CS8602
            //Application.Current.MainPage = new AppShell();
            return;
        }
        //#endif
        await Toast.Make("Não foi possível se conectar!", ToastDuration.Short).Show();
    }



    public List<IDevice> GetBluetoothDevicesAsync()
    {
        var adapter = CrossBluetoothLE.Current.Adapter;
        var devices = adapter.GetSystemConnectedOrPairedDevices();
        return devices.ToList();
    }
}
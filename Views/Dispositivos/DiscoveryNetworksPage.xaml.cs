using Aquaponia.Domain.Entities;
using Aquaponia.Domain.Interfaces;
using Aquaponia.DTO.Entities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;



namespace PI_AQP.Views;

public partial class DiscoveryNetworksPage : ContentPage
{
    ObservableCollection<ConnectionWifiDTO> ListHotpost = new();
    DevicesBluetoothDTO _device;
    private readonly IDevicesConnectionService _devicesServices = default!;
    private readonly CancellationTokenSource cts = new();

    const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
    const string CHARACTERISTIC_WIFI_UUID = "6fd27a35-0b8a-40cb-ad23-3f3f6c0d8626";
    const string CHARACTERISTIC_INFO_UUID = "eeaaf2ad-5264-47a1-a49f-5b274ab1a0fe";
    const string CHARACTERISTIC_SYSTEM_INFO_UUID = "26f8dc2f-213a-4048-a898-07eaf4f2089e";
    const string CHARACTERISTIC_ROUTINES_UUID = "4b9c96d4-cf69-45ca-a157-9ac8d0b7b155";
    const string CHARACTERISTIC_CONFIGURATION_UUID = "b371220d-3559-410d-8a47-78b06df6eb3a";

    public DiscoveryNetworksPage(IDevicesConnectionService devicesServices, DevicesBluetoothDTO device)
    {
        _devicesServices = devicesServices;
        _device = device;

        //devicesServices.OnUpdate(_device.id, SERVICE_UUID, CHARACTERISTIC_SYSTEM_INFO_UUID, onDebug);
        //devicesServices.OnUpdate(_device.id, SERVICE_UUID, CHARACTERISTIC_WIFI_UUID, onScan);

        BindingContext = this;
        InitializeComponent();

        ListHotpost = new ObservableCollection<ConnectionWifiDTO>();
        connectionsList.ItemsSource = ListHotpost;

        //Task.Run(StartScan, cts.Token);
    }
    protected override void OnAppearing()
    {
        //cts.TryReset();
        //Task.Run(StartScan, cts.Token);
    }
    protected override void OnDisappearing()
    {
        cts.Cancel();
    }

    public void RedirectToConnect(object sender, TappedEventArgs args)
    {
        StackLayout e = (StackLayout)sender;
        if (e == null)
            return;

        ConnectionWifiDTO hotpost = (ConnectionWifiDTO)e.BindingContext;
        if (hotpost == null) return;

        Navigation.PushAsync(new ConnectDeviceInNetworkPage(_devicesServices, _device, hotpost));
    }

    public void IgnorarRede_Clicked(object sender, EventArgs args)
    {
#pragma warning disable CS8602
        //Application.Current.MainPage = new AppShell(_wifiServices);
        //Application.Current.MainPage = new AppShell(_wifiServices);
        var d = _devicesServices.GetDevice(_device.id);
    }

    public void onScan(CharacteristicBluetoothDTO ble)
    {
        string response = Encoding.ASCII.GetString(ble.Value);
        List<ConnectionWifiDTO> l = JsonSerializer.Deserialize<List<ConnectionWifiDTO>>(response) ?? new();

        ListHotpost.Clear();
        foreach (var d in l)
        {
            ListHotpost.Add(d);
        }

    }
    public void onDebug(CharacteristicBluetoothDTO ble)
    {
        string response = Encoding.ASCII.GetString(ble.Value);
        Debug.WriteLine(response);
    }
}
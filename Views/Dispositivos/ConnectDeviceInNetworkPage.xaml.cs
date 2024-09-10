using Aquaponia.Domain.Entities;
using Aquaponia.Domain.Interfaces;
using Aquaponia.DTO.Entities;
using System.Diagnostics;
using System.Text.Json;
namespace PI_AQP.Views;

public partial class ConnectDeviceInNetworkPage : ContentPage
{
    public string Name { get; set; }
    public string Password { get; set; } = String.Empty;

    public struct WebServer { public string ip; };


    const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
    const string CHARACTERISTIC_WIFI_UUID = "6fd27a35-0b8a-40cb-ad23-3f3f6c0d8626";
    const string CHARACTERISTIC_INFO_UUID = "eeaaf2ad-5264-47a1-a49f-5b274ab1a0fe";
    const string CHARACTERISTIC_SYSTEM_INFO_UUID = "26f8dc2f-213a-4048-a898-07eaf4f2089e";
    const string CHARACTERISTIC_ROUTINES_UUID = "4b9c96d4-cf69-45ca-a157-9ac8d0b7b155";
    const string CHARACTERISTIC_CONFIGURATION_UUID = "b371220d-3559-410d-8a47-78b06df6eb3a";

    private DevicesBluetoothDTO _device;
    private ConnectionWifiDTO _hotpost;

    private readonly IDevicesConnectionService _devicesServices = default!;
    public ConnectDeviceInNetworkPage(IDevicesConnectionService devicesServices, DevicesBluetoothDTO device, ConnectionWifiDTO hotpost)
    {
        _devicesServices = devicesServices;
        _device = device;
        _hotpost = hotpost;

        Name = _hotpost.ssid;

        BindingContext = this;
        InitializeComponent();
    }

    public async void BtnConnecting_Clicked(object sender, EventArgs args)
    {
        string data = JsonSerializer.Serialize(new
        {
            ssid = Name,
            password = Password,
        });
        try
        {
            //await _devicesServices.SendMessage(_device.id, SERVICE_UUID, CHARACTERISTIC_WIFI_UUID, data);
            //string response = await _devicesServices.Request(_device.id, SERVICE_UUID, CHARACTERISTIC_WIFI_UUID);

            ////JsonElement webServer = JsonSerializer.Deserialize<WebServer>(response);
            ////IPAddress.Parse(webServer.ip);

            ////Application.Current.MainPage = new AppShell();

            //Debug.WriteLine(response, "Conectado");
        }
        catch (Exception e)
        {
            await DisplayAlert("Ops", "Não foi possível se conectar", "Tentar novamente");
            Debug.WriteLine(e.Message, "Error");
        }

    }
}
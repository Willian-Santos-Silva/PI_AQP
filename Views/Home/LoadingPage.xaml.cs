using Aquaponia.Domain.Entities;
using Aquaponia.Domain.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using PI_AQP.Helpers;

namespace PI_AQP.Views;

public partial class LoadingPage : ContentPage
{
    readonly IDevicesConnectionService _devicesServices;
    public DevicesBluetoothDTO _device;
    public LoadingPage(IDevicesConnectionService devicesServices)
    {
        _devicesServices = devicesServices;
        InitializeComponent();
    }

    public async void OnPageAppearing(object sender, EventArgs e)
    {
        if(await CheckAndRequestPermissions())
        {
            if(Preferences.Get("deviceID_BLE", null) != null)
            {
                _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };

                LoadingDescription.Text = "Conectando dispositivo...";
                if (await _devicesServices.TryConnectAsync(_device.id))
                {
                    await Toast.Make("Dispositivo conectado com sucesso!", ToastDuration.Short).Show();
                    Preferences.Set("deviceID_BLE", _device.id.ToString());

                    Application.Current.MainPage = new AppShell();
                    return;
                }
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
#pragma warning disable CS8602
                //App.Current.MainPage = new NavigationPage(new ConnectDevicePage(_devicesServices));
                App.Current.MainPage = new NavigationPage(new DiscoveryDevicesPage(_devicesServices));

            });
        }
    }

    #region PERMISSIONS
    private async Task<bool> CheckAndRequestPermissions()
    {
        LoadingDescription.Text = "Validando permissões...";
        PermissionStatus status = await Permissions.CheckStatusAsync<PermissionsExtends>();
        if (status == PermissionStatus.Granted)
        {
            return true;
        }

        LoadingDescription.Text = "Solicitando permissões...";
        status = await Permissions.RequestAsync<PermissionsExtends>();

        return status == PermissionStatus.Granted;
    }
    #endregion
}
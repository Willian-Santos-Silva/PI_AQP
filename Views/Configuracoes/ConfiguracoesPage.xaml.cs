using Aquaponia.Domain.Entities;
using Aquaponia.DTO.Entities;
using CommunityToolkit.Maui.Views;
using PI_AQP.Mapper;
using PI_AQP.Models;
using PI_AQP.Services;
using PI_AQP.Views.General;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace PI_AQP.Views;

public partial class ConfiguracoesPage : ContentPage, INotifyPropertyChanged
{
    public ICommand OnSaveChanges { get; set; }
    private Configuracao _configuracao;
    public Configuracao configuracao
    {
        get => _configuracao;
        set
        {
            if (_configuracao != value)
            {
                _configuracao = value;
                OnPropertyChanged(nameof(configuracao));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private BluetoothCaracteristica _configuracaoCaracteristica;
    private BluetoothCaracteristica _rtcCaracteristica;
    private DevicesBluetoothDTO _device;

    const string SERVICE_UUID = "02be3f08-b74b-4038-aaa4-5020d1582eba";
    const string CHARACTERISTIC_CONFIGURATION_UUID = "b371220d-3559-410d-8a47-78b06df6eb3a";
    const string CHARACTERISTIC_RTC_UUID = "a5939a1a-0b50-48d0-8d03-fad87790ab4a";

    public ConfiguracoesPage()
    {
        OnSaveChanges = new Command<InputNumberView>(SaveChange);
        configuracao = new();
        BindingContext = this;

        _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
        _configuracaoCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_CONFIGURATION_UUID);
        _configuracaoCaracteristica.CallbackOnUpdate(GetConfiguration);

        _rtcCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_UUID, CHARACTERISTIC_RTC_UUID);

        InitializeComponent();
    }
    private async Task StartServices()
    {
        await _configuracaoCaracteristica.StartService();
        await _configuracaoCaracteristica.OnStartUpdate();
        await _configuracaoCaracteristica.Request();

        await _rtcCaracteristica.StartService();
    }

    protected override async void OnAppearing()
    {
        var loading = new PopupLoadingSpinner();
        try
        {
            this.ShowPopup(loading);

            await StartServices();

            await loading.CloseAsync();
        }
        catch (Exception e)
        {
            await loading.CloseAsync();
            await DisplayAlert("Erro", e.Message, "Ok");
        }
    }

    public async void OnOpenModalUpdateRTC(object sender, TappedEventArgs args)
    {
        var popup = new ModalView();

        var result = await this.ShowPopupAsync(popup, CancellationToken.None);

        if (result is bool boolResult)
        {
            if (boolResult)
            {
                await SalvarDataEHora();
            }
        }
    }
    private async void RestartPage()
    {
        var currentShell = Shell.Current;
        var shell = new AppShell(); // Crie uma nova instância do Shell
        Application.Current.MainPage = shell;
        if (shell != null)
            await shell.Navigation.PushAsync(new ConfiguracoesPage());
    }
    private async Task SalvarDataEHora()
    {
        try
        {
            ConfiguracoesDTO  dto = _configuracao.ToDTO();

            await _rtcCaracteristica.SendMessage(JsonSerializer.Serialize(new { rtc = dto.rtc }));
            await _configuracaoCaracteristica.Request();
        }
        catch (Exception e)
        {
            await DisplayAlert("Erro", e.Message, "Ok");
        }
    }

    private void GetConfiguration(CharacteristicBluetoothDTO ble)
    {
        try
        {
            string response = Encoding.UTF8.GetString(ble.Value);


            ConfiguracoesDTO? dto = JsonSerializer.Deserialize<ConfiguracoesDTO>(response) ?? new ConfiguracoesDTO();
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                configuracao = dto.ToModel();
            });
        }
        catch (Exception e)
        {
            DisplayAlert("Erro", e.Message, "Ok");
        }
    }

    public async void SaveChange(InputNumberView e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                await _configuracaoCaracteristica.SendMessage(JsonSerializer.Serialize<ConfiguracoesDTO>(_configuracao.ToDTO()));
                await StartServices();
            }
            catch (Exception e)
            {
                await DisplayAlert("Erro", e.Message, "Ok");
            }
        });
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
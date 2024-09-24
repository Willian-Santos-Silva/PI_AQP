using Aquaponia.Domain.Entities;
using Aquaponia.Domain.Interfaces;
using PI_AQP.Views;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace PI_AQP.Services
{
    public class BluetoothClientService : IDevicesConnectionService
    {
        protected readonly IAdapter _adapter;
        private List<IDevice> ListDevicesIDevices { get; set; }

        public BluetoothClientService()
        {
            _adapter = CrossBluetoothLE.Current.Adapter;

            ListDevicesIDevices = new List<IDevice>();

            _adapter.DeviceDiscovered += (s, a) =>
            {
                if (String.IsNullOrEmpty(a.Device.Name))
                    return;

                var device = a.Device;
                ListDevicesIDevices.Add(device);
            };
            _adapter.DeviceDisconnected += (s, a) =>
            {
                App.Current.MainPage = new NavigationPage(new DiscoveryDevicesPage(this));
            };

        }

        public async Task<bool> TryConnectAsync(Guid deviceId)
        {
            try
            {
                //var devices = _adapter.GetKnownDevicesByIds([deviceId]);

                //await _adapter.DisconnectDeviceAsync(devices.First());
                //if (devices.Count() > 0)
                //    return devices.First().State == DeviceState.Connected;

                var device = await _adapter.ConnectToKnownDeviceAsync(deviceId, new ConnectParameters());
                if (device == null) throw new Exception("Device Not Found");

                return device.State == DeviceState.Connected;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<DevicesBluetoothDTO>> StartScanningAsync()
        {
            ListDevicesIDevices.Clear();

            await _adapter.StartScanningForDevicesAsync();

            ObservableCollection<DevicesBluetoothDTO> ListDevice = new ObservableCollection<DevicesBluetoothDTO>();

            foreach (var device in ListDevicesIDevices)
            {
                ListDevice.Add(new DevicesBluetoothDTO
                {
                    id = device.Id,
                    ssid = device.Name,
                    rssi = device.Rssi
                });
            }

            return ListDevice;
        }

        public DevicesBluetoothDTO GetDevice(Guid deviceId)
        {
            var device = _adapter.GetSystemConnectedOrPairedDevices().FirstOrDefault(s => s.Id == deviceId);

            if (device == null) throw new Exception("Device Not Found");

            return new()
            {
                id = device.Id,
                ssid = device.Name,
                rssi = device.Rssi
            };
        }
    }




    public class BluetoothCaracteristica
    {
        private List<byte> dataBuffer = new List<byte>();
        private string _serviceUUID;
        private string _characteristicUUID;
        private Guid _deviceId;
        private Action<CharacteristicBluetoothDTO> callback;

        protected readonly IAdapter _adapter;
        protected IDevice device = default!;
        protected ICharacteristic _characteristic = default!;
        protected IService _service = default!;

        public BluetoothCaracteristica(Guid deviceId, string serviceUUID, string characteristicUUID)
        {
            _adapter = CrossBluetoothLE.Current.Adapter;
            _serviceUUID = serviceUUID;
            _characteristicUUID = characteristicUUID;
            _deviceId = deviceId;

        }

        public async Task StartService()
        {
            try
            {
                if (_characteristic != null)
                {
                    return;
                }
                device = await _adapter.ConnectToKnownDeviceAsync(_deviceId, new ConnectParameters());

                _service = await device.GetServiceAsync(new Guid(_serviceUUID));

                if (_service == null)
                    throw new Exception("SERVIÇO NÃO ENCONTRADO");

                _characteristic = await _service.GetCharacteristicAsync(new Guid(_characteristicUUID));

                if (_characteristic == null)
                    throw new Exception("CARACTERÍSTICA NÃO ENCONTRADA");

                var requestMtu = 512;
                var responseMtu = await device.RequestMtuAsync(requestMtu);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task SendMessage(string data)
        {
            int dataSize = data.Length;
            int offset = 0;
            while (offset < dataSize)
            {
                int chuncksize = Math.Min(60, dataSize - offset);
                await _characteristic.WriteAsync(Encoding.ASCII.GetBytes(data.Substring(offset, chuncksize)));
                offset += chuncksize;
            }
            await _characteristic.WriteAsync([0xFF]);
        }

        public async Task<string> Request()
        {
            if (_characteristic == null)
                return default!;

            var (data, resultCode) = await _characteristic.ReadAsync();

            return Encoding.ASCII.GetString(data);
        }
        public void CallbackOnUpdate(Action<CharacteristicBluetoothDTO> onUpdateValue)
        {
            callback = onUpdateValue;
        }
        private void EventCallback(object? s, CharacteristicUpdatedEventArgs e)
        {
            if (e.Characteristic.Value.Last() != 0xFF)
            {
                dataBuffer.AddRange(e.Characteristic.Value);
                return;
            }
            dataBuffer.RemoveAll(b => b == 0x00);

            Debug.WriteLine(Encoding.ASCII.GetString(dataBuffer.ToArray()), "invoke");

            callback.Invoke(new CharacteristicBluetoothDTO { Value = dataBuffer.ToArray() });
            dataBuffer.Clear();
        }


        public Task OnStartUpdate()
        {
            dataBuffer.Clear();
            _characteristic.ValueUpdated += EventCallback;
            //if(_characteristic.CanUpdate)
                return _characteristic.StartUpdatesAsync();
        }
        public async Task OnPauseUpdate()
        {
            _characteristic.ValueUpdated -= EventCallback;
            await _characteristic.StopUpdatesAsync();
            dataBuffer.Clear();
        }
    }
}
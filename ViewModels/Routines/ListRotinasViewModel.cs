using Aquaponia.Domain.Entities;
using Aquaponia.DTO.Entities;
using PI_AQP.Mapper;
using PI_AQP.Models;
using PI_AQP.Services;
using PI_AQP.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace PI_AQP.ViewModels
{
    public class ListRotinasViewModel
    {
        public ICommand EditTappedCommand { get; }
        public ICommand RemoveTappedCommand { get; }
        public ObservableCollection<RotinasCardModel> listRotinas { get; set; }


        public BluetoothCaracteristica _rotinasListCaracteristica;
        public BluetoothCaracteristica _rotinasDeleteCaracteristica;
        public DevicesBluetoothDTO _device;

        const string SERVICE_ROUTINES_UUID = "bb1db1b1-6696-4bfa-a140-8d5836e980c8";
        const string CHARACTERISTIC_UPDATE_ROUTINES_UUID = "4b9c96d4-cf69-45ca-a157-9ac8d0b7b155";
        const string CHARACTERISTIC_GET_ROUTINES_UUID = "7dc355e9-6089-4a6f-b22c-f65820cad8c0";
        const string CHARACTERISTIC_DELETE_ROUTINES_UUID = "77fb44d3-2e17-4cec-9c22-e9a413c4eea7";
        public ListRotinasViewModel()
        {
            listRotinas = new ObservableCollection<RotinasCardModel>();

            _device = new() { id = Guid.Parse(Preferences.Get("deviceID_BLE", "")) };
            _rotinasListCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_ROUTINES_UUID, CHARACTERISTIC_GET_ROUTINES_UUID);
            _rotinasListCaracteristica.CallbackOnUpdate(UpdateRotinas);

            _rotinasDeleteCaracteristica = new BluetoothCaracteristica(_device.id, SERVICE_ROUTINES_UUID, CHARACTERISTIC_DELETE_ROUTINES_UUID);

            RemoveTappedCommand = new Command<RotinasCardModel>(RemoveItem);
            EditTappedCommand = new Command<RotinasCardModel>(EditItem);
        }
        public void UpdateRotinas(CharacteristicBluetoothDTO ble)
        {
            try
            {
                string response = Encoding.ASCII.GetString(ble.Value);

                var rotinasDto = JsonSerializer.Deserialize<List<RotinasDTO>>(response) ?? new List<RotinasDTO>();

                while (listRotinas.Count > 0) { listRotinas.RemoveAt(0); }
                foreach (var rotina in rotinasDto.ToViewModelRotinasCardModel())
                {
                    listRotinas.Add(rotina);
                }
                listRotinas.First().isOn = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public async Task StartService()
        {
            try
            {
                await _rotinasListCaracteristica.StartService();
                await _rotinasListCaracteristica.OnStartUpdate();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message, "rotinas list");
            }
        }
        public async Task Reload()
        {
            await _rotinasListCaracteristica.Request();
        }
        private async void RemoveItem(RotinasCardModel item)
        {
            if (listRotinas.Count() < 1)
                return;

            await _rotinasDeleteCaracteristica.StartService();
            await _rotinasDeleteCaracteristica.SendMessage(JsonSerializer.Serialize(new { id = item.Id }));
            //if (_rotinasService.Delete(item.id))
            listRotinas.Remove(item);

            if (listRotinas.Count() >= 1)
                listRotinas.First().isOn = true;
        }
        private async void EditItem(RotinasCardModel item)
        {
            bool[] dw = new bool[7];
            foreach (var r in listRotinas)
            {
                if (r.Id == item.Id)
                    continue;

                for (int i = 0; i < 7; i++)
                {

                    dw[i] |= r.rotinaDTO.WeekDays[i];
                }
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                { "_rotina", item.rotinaDTO },
                { "dwUtilizados", dw }
            };

            await Shell.Current.GoToAsync(nameof(UpdateIrrigationRoutinePage), parameters);
        }

    }
}
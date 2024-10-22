using Android.Bluetooth;
using Android.Content;
using Android.Locations;
using Android.Provider;
using Aquaponia.Domain.Interfaces;
using PI_AQP.Platforms.Android.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Dependency(typeof(BluetoothService))]
namespace PI_AQP.Platforms.Android.Services
{
    public class BluetoothService : IBluetoothService
    {
        public bool IsBluetoothEnabled()
        {
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            return bluetoothAdapter != null && bluetoothAdapter.IsEnabled;
        }
        public void OpenBluetoothSettings()
        {
            Intent intent = new Intent(Settings.ActionBluetoothSettings);
            intent.AddFlags(ActivityFlags.NewTask); // Importante para abrir fora do contexto de uma Activity
            MauiApplication.Current.StartActivity(intent);
        }
    }
}

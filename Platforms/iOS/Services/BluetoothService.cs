using PI_AQP.Platforms.iOS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Dependency(typeof(BluetoothService))]
namespace PI_AQP.Platforms.iOS.Services
{
    public class BluetoothService : IBluetoothService
    {
        public bool IsBluetoothEnabled()
        {
            var centralManager = new CBCentralManager();
            return centralManager.State == CBCentralManagerState.PoweredOn;
        }

        public void OpenBluetoothSettings()
        {
            var url = new NSUrl(UIApplication.OpenSettingsUrlString);
            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquaponia.Domain.Interfaces;
using PI_AQP.Platforms.iOS.Services;

[assembly: Dependency(typeof(LocationService))]
namespace PI_AQP.Platforms.iOS.Services
{
    public class LocationService : ILocationService
    {
        public bool IsLocationEnabled()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        public void OpenLocationSettings()
        {
            var url = new NSUrl(UIApplication.OpenSettingsUrlString);
            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }
    }
}

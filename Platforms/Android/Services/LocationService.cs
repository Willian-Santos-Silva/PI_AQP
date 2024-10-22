using Android.Content;
using Android.Locations;
using PI_AQP.Platforms.Android.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquaponia.Domain.Interfaces;
using Android.Provider;

[assembly: Dependency(typeof(PI_AQP.Platforms.Android.Services.LocationService))]
namespace PI_AQP.Platforms.Android.Services
{
    public class LocationService : ILocationService
    {
        public bool IsLocationEnabled()
        {
            LocationManager locationManager = (LocationManager)MauiApplication.Current.GetSystemService(Context.LocationService);
            if (locationManager == null)
            {
                throw new NullReferenceException("LocationManager não foi instanciado corretamente.");
            }

            return locationManager.IsProviderEnabled(LocationManager.GpsProvider) ||
                   locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
        }
        public void OpenLocationSettings()
        {
            Intent intent = new Intent(Settings.ActionLocationSourceSettings);
            intent.AddFlags(ActivityFlags.NewTask); // Importante para abrir fora do contexto de uma Activity
            MauiApplication.Current.StartActivity(intent);
        }
    }
}

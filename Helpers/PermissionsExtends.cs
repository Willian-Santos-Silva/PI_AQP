using static Microsoft.Maui.ApplicationModel.Permissions;

namespace PI_AQP.Helpers
{
    public class PermissionsExtends : BasePlatformPermission
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string permission, bool isRuntime)>
        {
            (Android.Manifest.Permission.AccessFineLocation, true),
            (Android.Manifest.Permission.AccessCoarseLocation, true),
            (Android.Manifest.Permission.ChangeWifiState, true),
            (Android.Manifest.Permission.ChangeNetworkState, true),
            (Android.Manifest.Permission.AccessWifiState, true),
            (Android.Manifest.Permission.Internet, true),
            (Android.Manifest.Permission.AccessNetworkState, true),
            (Android.Manifest.Permission.NearbyWifiDevices, true),
            //(Android.Manifest.Permission.ReadExternalStorage, true),
            ////(Android.Manifest.Permission.AccessBackgroundLocation, true),
            //(Android.Manifest.Permission.WriteExternalStorage, true),

            (Android.Manifest.Permission.Bluetooth, true),
            (Android.Manifest.Permission.BluetoothConnect, true),
            (Android.Manifest.Permission.BluetoothScan, true),
            (Android.Manifest.Permission.BluetoothAdvertise, true),
            (Android.Manifest.Permission.BluetoothAdmin, true)
        }.ToArray();
#endif
    }
}

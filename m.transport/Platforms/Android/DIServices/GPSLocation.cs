using m.transport.Interfaces;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(IGPSLocation))]
namespace m.transport.Android
{
    public class GPSLocation : IGPSLocation
    {
        public void StartLocationTracking()
        {
            // NO-OP
        }

        public void StopLocationTracking()
        {
            // NO-OP
        }

        public bool CheckLocationPermission()
        {
            return false;
        }
    }
}
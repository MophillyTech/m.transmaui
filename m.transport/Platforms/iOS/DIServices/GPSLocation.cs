using System;
using m.transport.Interfaces;
using m.transport.iOS;
using Foundation;
using UIKit;
using CoreTelephony;
using ObjCRuntime;
using System.Runtime.InteropServices;
using Foundation;
using CoreLocation;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(IGPSLocation))]
namespace m.transport.iOS.DIServices
{
    public class GPSLocation : IGPSLocation
    {
        public GPSLocation()
        {
        }

        public void StartLocationTracking()
        {
            LocService.Instance.StartLocationUpdates();
        }

        public void StopLocationTracking()
        {
            LocService.Instance.StopLocationUpdates();
        }

        public bool CheckLocationPermission()
        {
            return CLLocationManager.Status == CLAuthorizationStatus.Denied || CLLocationManager.Status == CLAuthorizationStatus.Restricted;
		}
    }
}

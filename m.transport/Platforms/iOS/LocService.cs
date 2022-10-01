using System;
using CoreLocation;
using m.transport;
using m.transport.iOS;
using UIKit;

namespace m.transport.iOS
{
	public sealed class LocService
	{
        protected CLLocationManager locMgr;
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        private LocService()
		{
            this.locMgr = new CLLocationManager();
            this.locMgr.PausesLocationUpdatesAutomatically = false;
            LocationUpdated += ReportLocation;

        }

		public static LocService Instance { get { return Nested.instance; } } 

		private class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested()
			{
			}

			internal static readonly LocService instance = new LocService();
		}


        public void StartLocationUpdates()
        {
            locMgr.RequestAlwaysAuthorization();
            locMgr.RequestWhenInUseAuthorization();
            locMgr.AllowsBackgroundLocationUpdates = true;

            // We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
            // the popover when the app is first launched, or by changing the permissions for the app in Settings
            if (CLLocationManager.LocationServicesEnabled)
            {

                //set the desired accuracy, in meters
                LocMgr.DesiredAccuracy = 1;

                LocMgr.LocationsUpdated += async (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                };

                LocMgr.StartUpdatingLocation();
            }
        }

        public void StopLocationUpdates()
        {
            LocMgr.StopUpdatingLocation();
        }

        public CLLocationManager LocMgr
        {
            get { return this.locMgr; }
        }

        //This will keep going in the background and the foreground
        public void ReportLocation(object sender, LocationUpdatedEventArgs e)
        {

            CLLocation location = e.Location;
            Console.WriteLine("Altitude: " + location.Altitude + " meters");
            Console.WriteLine("Longitude: " + location.Coordinate.Longitude);
            Console.WriteLine("Latitude: " + location.Coordinate.Latitude);
            Console.WriteLine("Course: " + location.Course);
            Console.WriteLine("Speed: " + location.Speed);

            m.transport.LocationManager.Instance.IosReportLocation(location.Coordinate.Latitude, location.Coordinate.Longitude);
        }

    }
}
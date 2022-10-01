using System;
using m.transport.Data;
using Autofac;
using System.Collections.ObjectModel;
using m.transport.Domain;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using m.transport.ViewModels;
using System.Collections.Generic;
using System.Linq;
using m.transport.Interfaces;
using System.Threading.Tasks;
using System.Timers;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public sealed class LocationManager
	{
		protected readonly ICurrentLoadRepository loadRepo;
		protected readonly ILoginRepository loginRepo;

		public static bool OmitVehicleCheckLocationReporting { get; set; }
		private int errorCounter = 0;
		public static DateTime CheckTime { get; set; }

		private LocationManager()
		{
			loadRepo = App.Container.Resolve<ICurrentLoadRepository>();
			loginRepo = App.Container.Resolve<ILoginRepository>();
			CheckTime = DateTime.MinValue;
		}

		public static LocationManager Instance { get { return Nested.instance; } }

		private class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested()
			{
			}

			internal static readonly LocationManager instance = new LocationManager();
		}

		public void SetCheckLocationTimer()
		{
			CheckTime = DateTime.Now.AddMinutes(5);
		}

		private bool CheckForReportingVIN()
		{
			if (loadRepo.SelectedLoad.Vehicles != null)
			{
				foreach (DatsVehicleV5 veh in loadRepo.SelectedLoad.Vehicles)
				{
					if (veh.VehicleStatus == "Loaded" && veh.IsShipmentTrackedByVehicle == 1)
					{
						return true;
					}
#if DEBUG
					System.Diagnostics.Debug.WriteLine("Checking Loading status");
					return true;
#endif
				}
			}


			return false;
		}

		// For Android
		public async Task ReportLocation()
		{
			bool reportLocation = true;

			if (loginRepo.LoginResult != null && loginRepo.LoginResult.GPSTrackingInd == 1)
			{
				if (OmitVehicleCheckLocationReporting)
				{
					System.Diagnostics.Debug.WriteLine("Reporting 5 min delivery location");
				}

				if (!OmitVehicleCheckLocationReporting)
				{
					System.Diagnostics.Debug.WriteLine("Android: Checking VIN status for location report");
					reportLocation = CheckForReportingVIN();
				}

				OmitVehicleCheckLocationReporting = false;

				if (reportLocation)
				{
					try
					{
						var request = new GeolocationRequest(GeolocationAccuracy.Best);
						var location = await Geolocation.GetLocationAsync(request);

						if (location != null)
						{
							System.Diagnostics.Debug.WriteLine("Reporting: " + location.Latitude.ToString() + " :: " + location.Longitude.ToString());
							SendLocationAsync(location.Latitude.ToString(), location.Longitude.ToString());
						}
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("Android error sending location to dispatch");
					}
				}
			}
		}

		// Cleanup For IOS
		public void IosReportLocation(double latitude, double longitude)
		{
			if (CheckTime >= DateTime.Now)
			{
				return;
			}

			bool reportLocation = true;

			if (loginRepo.LoginResult != null && loginRepo.LoginResult.GPSTrackingInd == 1)
			{
				if (OmitVehicleCheckLocationReporting)
				{
					System.Diagnostics.Debug.WriteLine("Reporting 5 min delivery location");
				}

				if (!OmitVehicleCheckLocationReporting)
				{
					System.Diagnostics.Debug.WriteLine("Checking VIN status for location report");
					reportLocation = CheckForReportingVIN();
				}

				OmitVehicleCheckLocationReporting = false;

				if (reportLocation)
				{
					try
					{
						SendLocationAsync(latitude.ToString(), longitude.ToString());
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("IoS error sending location to dispatch");
					}
				}
			}

			SetCheckLocationTimer();
		}

		public async Task<bool> TurnOnGPSTrackingAsync()
		{
			try
			{
				var request = new GeolocationRequest(GeolocationAccuracy.High);
				var location = await Geolocation.GetLocationAsync(request);
				if (location != null)
				{
					return true;
				}
			}
			catch (Exception ex)
			{

			}

			return false;

		}

		private void SendLocationAsync(string longitude, string lattitude)
		{
			System.Diagnostics.Debug.WriteLine("Latitude: " + lattitude + ", longtitude: " + longitude);
			loadRepo.UpdateLocationCompleted += OnUplodateLocationCompleted;
			loadRepo.UpdateLocationAsync(longitude, lattitude);
		}

		private void OnUplodateLocationCompleted(object sender, AsyncCompletedEventArgs args)
		{
			if (args.Error != null)
			{
				errorCounter++;
				return;
			}

			errorCounter = 0;
		}

		public void SendEmailNotification()
		{
			System.Diagnostics.Debug.WriteLine("Sending email notification");
			loadRepo.SendEmailCompleted += OnSendEmailNotificationCompleted;
			loadRepo.SendGPSEmailNotifcation();
		}

		private void OnSendEmailNotificationCompleted(object sender, AsyncCompletedEventArgs args)
		{
			if (args.Error == null)
			{
				System.Diagnostics.Debug.WriteLine("Sending email successful");
			}
		}

		public bool IsUserLoggedInAndRequireLocationTracking()
		{
			return loginRepo.LoginResult != null && loginRepo.LoginResult.GPSTrackingInd == 1;
		}
	}
}


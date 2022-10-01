using m.transport.Interfaces;
using m.transport.iOS.DIServices;
using UIKit;
using Xamarin;

namespace m.transport.iOS
{
	public class Application
	{
		static Application()
		{
			App.RegisterType<NetworkAvailability,INetworkAvailability>();
			App.RegisterType<LoadAndSaveFiles,ILoadAndSaveFiles>();
			App.RegisterType<ResourceLoader, ILoadResource>();
			App.RegisterType<GPSLocation, IGPSLocation>();

		}

		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}

	}
}


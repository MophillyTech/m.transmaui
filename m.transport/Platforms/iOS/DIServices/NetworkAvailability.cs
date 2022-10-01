using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using m.transport.Interfaces;
using m.transport.iOS.DIServices;
using CoreFoundation;
using Foundation;
using SystemConfiguration;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(NetworkAvailability))]
namespace m.transport.iOS.DIServices
{
	public class NetworkAvailability : INetworkAvailability
	{
		public bool IsNetworkAvailable()
		{
			NetworkReachabilityFlags flags;
			return IsNetworkAvailable(out flags);
		}

		// 
		// Raised every time there is an interesting reachable event, 
		// we do not even pass the info as to what changed, and 
		// we lump all three status we probe into one
		//
		public static event EventHandler ReachabilityChanged;

		static void OnChange(NetworkReachabilityFlags flags)
		{
			var h = ReachabilityChanged;
			if (h != null)
				h(null, EventArgs.Empty);
		}

		static NetworkReachability defaultRouteReachability;
		static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
		{
			if (defaultRouteReachability == null)
			{
				defaultRouteReachability = new NetworkReachability(new IPAddress(0));
				defaultRouteReachability.SetNotification (OnChange);
				// defaultRouteReachability.SetCallback(OnChange);
				defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
			}
			if (!defaultRouteReachability.TryGetFlags(out flags))
				return false;
			return IsReachableWithoutRequiringConnection(flags);
		}

		public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
		{
			// Is it reachable with the current network configuration?
			bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

			// Do we need a connection to reach it?
			bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

			// Since the network stack will automatically try to get the WAN up,
			// probe that
			if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
				noConnectionRequired = true;

			return isReachable && noConnectionRequired;
		}
	}
}
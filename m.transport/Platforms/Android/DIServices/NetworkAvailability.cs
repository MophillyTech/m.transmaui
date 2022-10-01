using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using m.transport.Interfaces;
using m.transport.Android.DIServices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(NetworkAvailability))]
namespace m.transport.Android.DIServices
{
	public class NetworkAvailability : INetworkAvailability
	{
		private readonly Context _context;

		public NetworkAvailability ()
		{
			_context = Forms.Context;
		}

		public bool IsNetworkAvailable()
		{
			var connectivityManager = (ConnectivityManager)_context.GetSystemService(Context.ConnectivityService);
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			return (activeConnection != null) && activeConnection.IsConnected;
		}
	}
}
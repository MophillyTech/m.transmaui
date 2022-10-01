using System;
using m.transport.Android;
using m.transport.Interfaces;
using Android.Telephony;
using Android.Content;
using Android.Net;
using Android.Views;
using Android.Runtime;
using Android.App;
using Android.Preferences;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(HardwareInfo))]
namespace m.transport.Android
{
	public class HardwareInfo : IHardwareInfo
	{
		private readonly Context _context;
		private static int retry = 0;

		public HardwareInfo()
		{
			_context = Forms.Context;
		}


		public string Version
		{
			get
			{
				return global::Android.OS.Build.VERSION.Release;
			}
		}

		public string Manufacturer
		{
			get
			{
				return global::Android.OS.Build.Manufacturer;
			}
		}

		public bool IsPortrait
		{
			get
			{
				IWindowManager windowManager = global::Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

				var rotation = windowManager.DefaultDisplay.Rotation;
				bool isLandscape = rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270;
				return !isLandscape;
			}
		}

		public string Carrier
		{
			get
			{
				string carrier = string.Empty;

				try
				{

					using (TelephonyManager mTelephonyMgr = (TelephonyManager)Forms.Context.GetSystemService(global::Android.Content.Context.TelephonyService))
					{
						//carrier = mTelephonyMgr.SimOperatorName;
						carrier = mTelephonyMgr.NetworkOperatorName;
					}
				}
				catch (Exception ex)
				{
					carrier = ex.Message;
				}

				return carrier;
			}
		}

		public string Name
		{
			get
			{
				// ???no standard implementation on Android???
				string name = string.Empty;
				return name;
			}
		}

		public int Width
		{
			get
			{
				return _context.Resources.DisplayMetrics.WidthPixels;
				//return Forms.Context.Resources.DisplayMetrics.WidthPixels;
			}
		}

		public int Height
		{
			get
			{
				return _context.Resources.DisplayMetrics.HeightPixels;
				//return Forms.Context.Resources.DisplayMetrics.HeightPixels;
			}
		}

		// For Android, the DeviceInfo plugin handles Model, so we don't need to do anything here
		public string Model
		{
			get
			{
				return string.Empty;
			}
		}

		public void ClearData()
		{
			((ActivityManager)_context.GetSystemService(Context.ActivityService)).ClearApplicationUserData();

		}

		public bool GetClearDataCompleted()
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
			return prefs.GetBoolean("clear_data_app_on_upgrade", true);
		}

		public void Landscape()
		{
			((Activity)Forms.Context).RequestedOrientation = ScreenOrientation.Landscape;
		}

		public void Portrait()
		{
			((Activity)Forms.Context).RequestedOrientation = ScreenOrientation.Portrait;
		}

		public void SetClearDataCompleted()
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutBoolean("clear_data_app_on_upgrade", false);
			editor.Apply();
		}

		private bool IsNetworkAvailable()
		{
			var connectivityManager = (ConnectivityManager)_context.GetSystemService(Context.ConnectivityService);
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			return (activeConnection != null) && activeConnection.IsConnected;
		}
	}
}


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
using Android.App;
using System;
using m.transport.Android.alpha;
using Microsoft.AppCenter.Crashes;
using m.transport.Android.alpha.Broadcast;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Alert))]
namespace m.transport.Android.DIServices
{
	public class Alert : IAlert
	{
		private readonly MainActivity _activity;

		public Alert()
		{
			_activity = MainApplication.getActivity();
		}

		public  void ShowAlert(string title, string msg, string[] items, Action<string> onSelect)
		{
            try
            {
				//_activity.DisplayCustomDialog(title, msg, items, onSelect);
				MainActivity.DisplayCustomDialog(title, msg, items, onSelect);
			}
			catch (Exception ex) {
                Crashes.TrackError(ex);
            }
		}

		public void ShowAlert(string title, string msg, Action onSelect)
		{
			try
			{
				_activity.DisplayDialog(title, msg, onSelect);
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
			}
		}
	}
}
using System;
using AndroidHUD;
using m.transport.Interfaces;
using m.transport.Android;
using Android.App;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Logger))]
namespace m.transport.Android
{
	public class Logger : ILogger
	{
		public Logger ()
		{
		}

		public string ReadLog() {
			return string.Empty;
		}

		public void Log(string message) {
		}

		public void ClearLog() {
		}

		public void CopyLog() {
		}
	}
}


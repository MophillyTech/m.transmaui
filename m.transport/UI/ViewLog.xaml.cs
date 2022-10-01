using System;
using System.Collections.Generic;
using m.transport.Interfaces;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ViewLog : ContentPage
	{
		ILogger log;

		public ViewLog ()
		{
			InitializeComponent ();

			log = DependencyService.Get<ILogger> ();

			LogData.Text = log.ReadLog ();
		}

		protected void CloseDialog(object sender, EventArgs args) {
			this.Navigation.PopModalAsync ();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			if (string.IsNullOrEmpty (LogData.Text)) {
				DisplayAlert ("Log Empty", "The Log is empty", "OK");
			}
		}

		protected void ClearLog(object sender, EventArgs args) {
			log.ClearLog ();
			LogData.Text = string.Empty;
			DisplayAlert ("Log Cleared", "The Log has been cleared", "OK");
		}

		protected void CopyLog(object sender, EventArgs args) {
			log.CopyLog ();
			DisplayAlert ("Log Copied", "The Log has been copied to the clipboard", "OK");
		}
	}
}


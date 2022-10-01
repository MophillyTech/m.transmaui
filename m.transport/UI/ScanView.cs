using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace DAI.POC
{
	public class ScanView : ContentPage
	{
		Label lblVIN;
		Entry txtVIN;
		CurrentLoad load;

		public ScanView (CurrentLoad load)
		{
			this.load = load;

			Title = "Scan VIN";

			Button btnScan = new Button { VerticalOptions = LayoutOptions.Center, Text = "Scan" };
			Button btnSearch = new Button { Text = "Search" };
			Button btnCancel = new Button { Text = "Cancel" };

			txtVIN = new Entry () { Placeholder = "Enter VIN" };

			btnScan.Clicked += HandleScanClicked;
			btnSearch.Clicked += HandleSearchClicked;
			btnCancel.Clicked += HandleCancelClicked;
			lblVIN = new Label ();
			lblVIN.XAlign = TextAlignment.Center;

			StackLayout sl = new StackLayout ();

			sl.Padding = new Thickness (20, 50, 20, 50);

			sl.Children.Add (btnScan);
			sl.Children.Add (lblVIN);
			sl.Children.Add (txtVIN);
			sl.Children.Add (btnSearch);
			sl.Children.Add (btnCancel);

			Content = sl;

		}

		void HandleCancelClicked (object sender, EventArgs e)
		{
			//Navigation.PopModalAsync ();
			Navigation.PopAsync ();
		}

		async void HandleSearchClicked (object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty (txtVIN.Text)) {
				DisplayAlert ("Error", "Please enter a valid VIN", "OK", "Cancel");
			} else {

				txtVIN.Text = txtVIN.Text.ToUpper ();

				Vehicle v = AppData.Loads [0].FindVIN (txtVIN.Text);

				if (v == null) {
					bool add = await DisplayAlert ("Error", "VIN Not Found.  Would you like to add a new vehicle?", "Add", "Cancel");

					if (add) {
						load.AddVIN (txtVIN.Text);
					}
				} else {
					//App.GetTab ().CurrentPage = App.GetTab ().Children [0]; // .SelectedItem = App.GetTab ().Children [0];
					//App.GetCurrentLoad().SelectVIN (txtVIN.Text);
					//Navigation.PopModalAsync ().ContinueWith ( (continuation) => {
					load.SelectVIN (txtVIN.Text);
					//});
				}
			}
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			txtVIN.Focus ();
		}

		void HandleScanClicked (object sender, EventArgs e)
		{
			DependencyService.Get<IBarCodeScanner> ().Read (delegate(string s) {
				if (!string.IsNullOrEmpty(s)) {
				Microsoft.Maui.Controls.Device.BeginInvokeOnMainThread(delegate() {
					lblVIN.Text = s.ToUpper();
					txtVIN.Text = s.ToUpper();
					HandleSearchClicked(null,null);
				});
				}
			});
		}

	}
}


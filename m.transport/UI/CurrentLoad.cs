using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Linq;
namespace DAI.POC
{
	public class CurrentLoad : ContentPage
	{
		LoadTable lt;
		ProgressBar pb;
		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			lt.Refresh ();

			pb.Progress = AppData.Loads [0].LoadProgress;
		}
		public CurrentLoad ()
		{
			StackLayout sl = new StackLayout ();
			//Button btnLoad = new Button () { Text = "Load Vehicles" };
			//btnLoad.Clicked += HandleClicked;
			//sl.Children.Add (btnLoad);

			StackLayout hsl = new StackLayout () { Padding = new Thickness(20,5,5,20), Orientation = StackOrientation.Horizontal };
			pb = new ProgressBar () {
				Progress = AppData.Loads [0].LoadProgress,
				BackgroundColor = Color.Green,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			hsl.Children.Add (pb);
			sl.Children.Add (hsl);

			lt = new LoadTable (AppData.Loads [0]);

			sl.Children.Add(lt);

			Content = sl; 
			Title = "Current Load";

			ToolbarItems.Add(new ToolbarItem("Load", string.Empty, delegate() { LoadVehicles(); }));
			ToolbarItems.Add(new ToolbarItem("Sync", string.Empty, delegate() {}));
		}

		void LoadVehicles()
		{
			//Navigation.PushModalAsync (new ScanView (this));
			Navigation.PushAsync (new ScanView (this));
		}

		public async void SelectVIN(string VIN) {
			//Navigation.PopModalAsync ();
			await Navigation.PopAsync();
			lt.SelectVIN (VIN);
		}

		public async void AddVIN(string VIN) {
			await Navigation.PopAsync ();
			//await Navigation.PopModalAsync ();
			//Navigation.PushModalAsync (new SelectDealer (AppData.Loads [0].Dealers));

			string[] dlrs = (from x in AppData.Loads [0].Dealers
			                 select x.Name).ToArray ();

			string result = await DisplayActionSheet ("Select Dealer", "Cancel", null, dlrs);

			if (!string.IsNullOrEmpty (result)) {
				Vehicle v = new Vehicle ();
				v.VIN = VIN;
				v.Status = "Loaded";
				v.Year = 2014;
				v.Make = "Chevrolet";
				v.Model = "Corvette";
				v.Color = "Red";

				//AppData.Loads [0].DealerLoads.Find (dl => dl.Dealer.Name == result).Vehicles.Add (v);
				//lt = new LoadTable (AppData.Loads [0]);
				lt.AddVehicle (result, v);
				lt.SelectVIN (VIN);
			}
			//System.Diagnostics.Debug.WriteLine (result);
		}
	}
}


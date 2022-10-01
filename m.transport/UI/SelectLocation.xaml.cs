using System;
using System.Collections.Generic;
using m.transport.Data;
using m.transport.ViewModels;
using m.transport.Domain;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectLocation : ContentPage
	{	
		private ILoginRepository loginRepo;
		private ICurrentLoadRepository loadRepository;
		private CustomObservableCollection<GroupedVehicles> groupedVehicles;
		private List<DatsLocation> locations;
		private string type;

		public SelectLocation (ICurrentLoadRepository loadRepository, ILoginRepository loginRepo, CustomObservableCollection<GroupedVehicles> groupedVehicles, List<DatsLocation> locations, string type)
		{
			this.loadRepository = loadRepository;
			this.loginRepo = loginRepo;
			this.groupedVehicles = groupedVehicles;
			this.locations = locations;
			this.type = type;

			ViewModel = new SelectLocationViewModel (groupedVehicles);

			InitializeComponent();

			LocationList.ItemsSource = ViewModel.Locations;
			LocationList.ItemTemplate = new DataTemplate(typeof(SelectLocationCell));
		}

		public SelectLocationViewModel ViewModel
		{
			get { return (SelectLocationViewModel) BindingContext; }
			set
			{
				BindingContext = value;
			}
		}

		async void OnActivated(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		public void LocationSelected(object sender, EventArgs ea)
		{
			var l = (DatsLocation)LocationList.SelectedItem;

			PrintReport (l);

			LocationList.SelectedItem = null;

		}

		private async void PrintReport(DatsLocation loc)
		{
			CustomObservableCollection<VehicleViewModel> vehicles = null;
			bool hasVehicleInLoadState = false;

			bool selectAll = loc.Name == "All" ? true : false;

			foreach (GroupedVehicles g in ViewModel.GroupedVehicles) {

				if (selectAll || g.Location == null && loc.Name == "Unexpected Pickup Location" || g.Location == loc) {
					vehicles = g.Vehicles;
					if(type == "Load Summary"){
						hasVehicleInLoadState = vehicles.Any(s => (s.DatsVehicle.VehicleStatus == "Loading" || s.DatsVehicle.VehicleStatus == "Loaded"));
					} else if(type == "Gate Pass") {
						hasVehicleInLoadState = vehicles.Any(s => (s.DatsVehicle.VehicleStatus == "Loading" || s.DatsVehicle.VehicleStatus == "Loaded"));
					}

					if(hasVehicleInLoadState)
						break;
				}

			}
				
			if (hasVehicleInLoadState) {
			
				bool resp = await DisplayAlert(type, "Would you like to print " + type + " for " + loc.Name + " ?", "Yes", "No");

				if (resp)
				{
					Boolean success = true;
					if(type == "Load Summary"){
						success = PaperWork.PrintLoadSummary(loginRepo, groupedVehicles, locations, loc.DisplayName);
					} else if(type == "Gate Pass") {
						success = PaperWork.PrintGatePass(loadRepository, loginRepo, groupedVehicles, locations, loc.DisplayName);
					}

					if(!success)
						await DisplayAlert ("Bluetooth is Turned Off", "To print a report, bluetooth must be turned on in the Settings app.", "OK");
					else
						await Navigation.PopToRootAsync();

				}

			} else {
				await DisplayAlert("No Vehicles!", "There are no vehicles on the truck to print a " + type + " for " + loc.Name, "OK");
			}
				
		}
	}


}


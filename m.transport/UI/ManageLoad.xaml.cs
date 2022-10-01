using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Utilities;
using m.transport.ViewModels;
using System.Text.RegularExpressions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ManageLoad : ContentPage
	{
		CurrentLoad parent;
		private int locID;
		IBarcode barcode = null;
		//private bool scanned = false;
		private string scanInput;
		private bool scanMode = false;
		private string addedVINAndAddDamage = null;
		CustomNavigationPage customSelectVehiclePage;
		SelectVehicle selectVehiclePage = null;
		private DatsLocation location;
		private bool PageLoading = false;

		public ManageLoad(int locID, CurrentLoad parent)
		{
			this.parent = parent;
			this.locID = locID;

			ViewModel = new ManageLoadViewModel(locID, InspectionType.Loading);

			InitializeComponent();

			VehicleList.IsGroupingEnabled = true;
			VehicleList.ItemTemplate = new DataTemplate(typeof(ManageVehicleCellContent));
			VehicleList.GroupHeaderTemplate = new DataTemplate(typeof(LoadCellHeader));
			VehicleList.HasUnevenRows = true;            

			location = ViewModel.Locations.FirstOrDefault(l => l.LocationId == locID);

			selectVehiclePage = new SelectVehicle (locID, InspectionType.Loading, 
				async delegate(VehicleViewModel v) {
					//using (InsightsManager.TrackTime ("SelectVehicle popping")) {
						await Navigation.PopModalAsync ();
					//}

					if (v == null) {
						return;
					}

					//using (InsightsManager.TrackTime ("SelectVehicle pushing")) {
						NavigateToInspection (v);
					//}
				}, async delegate() {
					//using (InsightsManager.TrackTime ("SelectVehicle popping")) {
						await Navigation.PopModalAsync ();
					//}

					SelectVehicle (null, null);
				}
			);

			customSelectVehiclePage = new CustomNavigationPage(selectVehiclePage);
		}

		private  async void NavigateToInspection(VehicleViewModel v)
		{
			await Navigation.PushModalAsync(new CustomNavigationPage (new Inspection (v, InspectionType.Loading, location)));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			ViewModel.RefreshList();
		}

		private void NavigateToInspection(String searchVIN)
		{
			VehicleViewModel vm = null;

			foreach(GroupedVehicles gp in ViewModel.VehiclesGrouped)
			{
				foreach(VehicleViewModel v in gp.Vehicles)
				{
					if(v.VIN == searchVIN){
						vm = v;
					}
				}
			}
			if (vm != null) {
				NavigateToInspection (vm);
			} 
		}

		public ManageLoadViewModel ViewModel
		{
			get { return (ManageLoadViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public async void ScanVIN(object sender, EventArgs ea)
		{
			MessagingCenter.Subscribe<ManageLoad> (this, MessageTypes.VINScanned, s => ValidateVIN ());

            if (!DependencyService.Get<ICamera>().CheckPermission())
            {
                await DisplayAlert("Not authorized", "Camara permission is turned off. Go To Setting > Vehicle Mobile > Privacy to turn camera on. Note, app will restart when permission is changed", "OK");
            }
            else
            {
				if (barcode == null)
				{
					barcode = DependencyService.Get<IBarcode>();
				}

				barcode.Scan(delegate (string s)
				{
					scanInput = s;
					scanMode = true;
					MessagingCenter.Send<ManageLoad>(this, MessageTypes.VINScanned);
				});
            }
		}

		public void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (textChangedEventArgs.NewTextValue == null || textChangedEventArgs.NewTextValue.Length == 0)
				return;

			//for Android this method gets trigger but not ios, we can tell if it is scanned vin vs
			//input vin based on the length from the previous entry
//			if (textChangedEventArgs.OldTextValue != null && textChangedEventArgs.OldTextValue.Length + 1 != textChangedEventArgs.NewTextValue.Length)
//				return;

			string vin = textChangedEventArgs.NewTextValue.ToUpper();

            string message;
            ScannerUtilities.ValidateInput(vin, out message);

            if (!String.IsNullOrEmpty(message))
            {
                VIN.Text = textChangedEventArgs.OldTextValue;
                DisplayAlert("Error", message, "OK");
            }
		}


		async public void SelectVehicle(object sender, EventArgs ea)
		{
			// this block is intended to block the user for a few seconds before displaying the SelectVehicle
			// modal, so that they will learn to use the Scan function instead.  
			// 6237 - change delay to 5s
			DependencyService.Get<IHud>().Show("Loading Vehicles...");
#if DEBUG
			await System.Threading.Tasks.Task.Delay(100);
#else
			try 
            {          
                int MobileSelectVinDelaySeconds = 1000*Convert.ToInt32(App.Container.Resolve<IAppSettingsRepository>().Settings["MobileSelectVinDelaySeconds"]);
                await System.Threading.Tasks.Task.Delay(MobileSelectVinDelaySeconds);
            }
            catch (Exception)
            {
                await System.Threading.Tasks.Task.Delay(5000);
            }
#endif
            DependencyService.Get<IHud>().Dismiss();
			Device.BeginInvokeOnMainThread(async () =>
			{
                // an Exception will be thrown if the previous modal is still present
                // this should not happen, but does sometimes.  To prevent a crash, we'll catch it

                try
                {
                    await Navigation.PushModalAsync(customSelectVehiclePage);
                } catch (Exception ex) {
                    // for now, swallow it, but should log it via AppCenter
                }
			});
		}



		public async void SearchVIN(object sender, EventArgs ea)
		{
			scanMode = false;
			scanInput = VIN.Text;
			ValidateVIN ();
		}


		public async void ValidateVIN()
		{
			MessagingCenter.Unsubscribe<ManageLoad> (this, MessageTypes.VINScanned);

			VIN.Unfocus ();
			DatsLocation loc = ViewModel.Locations.FirstOrDefault(l => l.LocationId == locID);

			Device.BeginInvokeOnMainThread(async () => {

                if (string.IsNullOrEmpty(scanInput))
                    return;

                string vinentered;
                string msg;
                ScannerUtilities.ValidateVIN(scanInput, out vinentered, out msg);

                VIN.Text = string.Empty;

                if (!string.IsNullOrEmpty(msg)) {
                    DisplayAlert("Error", msg, "OK");
                    return;
                }

				// 5414 - search all VINs, not just currently selected load/run
                VehicleViewModel matchingVehicle = ViewModel.SelectedVehicles.FirstOrDefault (v => v.VIN.EndsWith(vinentered.ToUpper()));

				string inputMode = scanMode ? "Scan" : "Enter";
				string inputModeVerb = scanMode ? "Scanning" : "Entering";

                string vinValidated = string.Empty;

                if (vinentered.Length > 8) 
                {
                    vinValidated = vinentered.Substring(vinentered.Length - 8);   
                } else
                {
                    vinValidated = vinentered.ToUpper();
                }


				if (matchingVehicle != null && 
					(matchingVehicle.DatsVehicle.VehicleStatus == VehicleStatus.Loading.ToString() || matchingVehicle.DatsVehicle.VehicleStatus == VehicleStatus.Assigned.ToString()))
				{
					DependencyService.Get<ISound>().PlaySound(SoundType.Success);

					if (locID > 0 && matchingVehicle.DatsVehicle.PickupLocationId != locID) {

						bool resp = await DisplayAlert("Vehicle Found", "The reported vehicle is from " + matchingVehicle.DatsVehicle.PickupLocationName +
							" but you are loading at " + loc.Name + ". Is this correct?", "Yes", "No");

						if (!resp)
						{
							return;
						}
					}

					var actionSheetOptions = new List<String> {
						matchingVehicle.DatsVehicle.HasPreExistingDamages ? "Add/Review Damage" : "Add Damage"
					};

                    string actionSheetTitle = vinValidated + " found";

					if (matchingVehicle.DatsVehicle.HasPreExistingDamages)
					{
						actionSheetTitle += " w/ dmg";
					}
					else
					{
						int vehicleCount = 
							(from v in ViewModel.SelectedVehicles
								where v.DatsVehicle.VehicleStatus == VehicleStatus.Loaded.ToString() || v.DatsVehicle.VehicleStatus == VehicleStatus.Loading.ToString()
								select v).Distinct().Count();
						if (matchingVehicle.DatsVehicle.VehicleStatus == VehicleStatus.Assigned.ToString ())
							vehicleCount++;

						if (scanMode) {
							actionSheetTitle += ". Scanned (" + vehicleCount + "/" + ViewModel.SelectedVehicles.Count + ")";
						} else {
							actionSheetTitle += ". Entered (" + vehicleCount + "/" + ViewModel.SelectedVehicles.Count + ")";
						}

						actionSheetOptions.Add(inputMode + " Next VIN");
						actionSheetOptions.Add("Done " + inputModeVerb);
					}

					switch (Device.RuntimePlatform)
					{
						case Device.Android:
                            actionSheetOptions.Add("Cancel");
							DependencyService.Get<IAlert>().ShowAlert(actionSheetTitle, "", actionSheetOptions.ToArray(), (selection) =>
							{
								HandleExistingSelection(matchingVehicle,selection);
							});
							break;
						case Device.iOS:
							string action = await DisplayActionSheet(actionSheetTitle, "Cancel", null, actionSheetOptions.ToArray());
							HandleExistingSelection(matchingVehicle, action);
							break;
					}
				}
				else
				{
					DependencyService.Get<ISound>().PlaySound(SoundType.Error);

					if (matchingVehicle != null) {
                        await DisplayAlert ("Error",  vinValidated + " is already loaded", "OK");
						return;
					}
                    else if (vinentered.Length < 17) {
                        await DisplayAlert("Error", "VIN not in Load. Try again with a complete VIN", "OK");
                        return;
                    }
						
                    DatsVehicleV5 dv = GetDatsVehicle (vinentered.ToUpper (), "", "");
					//has a pickup location
					if(loc != null){
						dv.PickupLocationName = loc.Name;
						dv.PickupLocationId = loc.Id;
					}
					if (string.IsNullOrEmpty(dv.DropoffLocationName)) 
						dv.DropoffLocationName = "Unknown";

                    var options = new List<String> {
                        "Add With Damage",
                        "Add Without Damage",
                        inputMode + " Different VIN"
                    };


					switch (Device.RuntimePlatform)
					{
						case Device.Android:
							options.Add("Cancel");
                            DependencyService.Get<IAlert>().ShowAlert(vinentered + " NOT Found", "", options.ToArray(), (selection) =>
							{
								HandleNewSelection(dv, selection);
							});
							break;
						case Device.iOS:
                            string action = await DisplayActionSheet(vinentered + " NOT Found", "Cancel", null, options.ToArray());
							HandleNewSelection(dv, action);
							break;
					}
				}
			});
		}

        private void HandleNewSelection(DatsVehicleV5 dv, string result) {
			switch (result)
			{
				case "Cancel":
					break;
				case "Add With Damage":
					addInputVIN(true, dv);
					break;
				case "Add Without Damage":
					addInputVIN(false, dv);
					break;
				case "Scan Different VIN":
					ScanVIN(null, null);
					break;
				case "Enter Different VIN":
					break;
			}
        }

        private void HandleExistingSelection(VehicleViewModel matchingVehicle, string result) {
			switch (result)
			{
				case "Cancel":
					break;
				case "Add Damage":
				case "Add/Review Damage":
					matchingVehicle.SetVehicleStatus(VehicleStatus.Loading);
					NavigateToInspection(matchingVehicle);
					break;
				case "Scan Next VIN":
					matchingVehicle.SetVehicleStatus(VehicleStatus.Loading);
					ScanVIN(null, null);
					break;
				case "Done Scanning":
				case "Done Entering":
					matchingVehicle.SetVehicleStatus(VehicleStatus.Loading);
					//await Navigation.PopAsync();
					break;
				case "Enter Next VIN":
					VIN.Focus();
					matchingVehicle.SetVehicleStatus(VehicleStatus.Loading);
					break;
			}
        }

		private void addInputVIN(bool navigateToDamage, DatsVehicleV5 dv)
		{
			ViewModel.AddVehicle (dv);

			ViewModel.CreateList();
			selectVehiclePage.UpdateVehicleList();

			if (navigateToDamage)
			{
				addedVINAndAddDamage = dv.VIN;
                NavigateToInspection(addedVINAndAddDamage);
			}
		}

		public DatsVehicleV5 GetDatsVehicle(string vin, string pickupLocationName, string dropOffLocationName)
		{
			DatsVehicleV5 vehicle = new DatsVehicleV5();
			vehicle.VIN = vin;
			vehicle.VinKey = vin.Substring (vin.Length - 8);
			vehicle.SetVehicleStatus(VehicleStatus.Loading); 
			vehicle.PickupLocationName = pickupLocationName;
			vehicle.DropoffLocationName = dropOffLocationName;
			vehicle.PickupLocationId = -2;
			vehicle.DropoffLocationId = -1;
			return vehicle;
		}


	}
}


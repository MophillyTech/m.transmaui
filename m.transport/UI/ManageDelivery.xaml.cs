using System;
using System.Collections.Generic;
using Autofac;
using m.transport.Data;
using m.transport.Utilities;
using m.transport.Domain;
using m.transport.ViewModels;
using m.transport.Interfaces;
using System.Linq;
using m.transport.Models;
using System.ComponentModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ManageDelivery : ContentPage
	{
		//private string selectedLocation;
		//private List<GroupedVehicles> vehicleList;
		private int selectedLocID;
		private string navigationAction;
		private DeliveryDamage deliveryDamagePage;

		CustomObservableCollection<VehicleViewModel> selectedDeliveryVehicles = new CustomObservableCollection<VehicleViewModel>();
		CustomObservableCollection<VehicleViewModel> unselectedDeliveryVehicles = new CustomObservableCollection<VehicleViewModel>();

		public ManageDelivery(int locID)
		{
			ViewModel = new ManageLoadViewModel(locID, InspectionType.Delivery);

			InitializeComponent();
			this.selectedLocID = locID;
			if (selectedLocID == 0)
				SelectedLocationLabel.Text = "Unknown";
			else
				SelectedLocationLabel.Text = ViewModel.Locations.FirstOrDefault (l => l.LocationId == locID).DisplayName;

			VehicleList.IsGroupingEnabled = true;
			VehicleList.ItemTemplate = new DataTemplate(typeof(SelectDeliverVehicleCell));
			VehicleList.GroupHeaderTemplate = new DataTemplate(typeof(LoadCellHeader));
			VehicleList.HasUnevenRows = true;
			VehicleList.ItemsSource = ViewModel.VehiclesGrouped;

	
			ToolbarItems.Add(new ToolbarItem("Next", string.Empty, async delegate
			{

				selectedDeliveryVehicles.Clear();
				unselectedDeliveryVehicles.Clear();

				foreach (GroupedVehicles x in ViewModel.VehiclesGrouped) {
					foreach (var v in x.Vehicles) {
						if (v.Selected) {
							selectedDeliveryVehicles.Add(v);
							}else if(v.DatsVehicle.VehicleStatus != VehicleStatus.Delivering.ToString() && v.DatsVehicle.DropoffLocationId != locID){
							unselectedDeliveryVehicles.Add(v);
						}
					}
				}

				Device.BeginInvokeOnMainThread(async () => {

					if (selectedDeliveryVehicles.Count > 0) {
						switch (Device.RuntimePlatform)
						{
							case Device.Android:
								DependencyService.Get<IAlert>().ShowAlert("Report damages?", "", new[] { "Yes", "No", "Cancel" }, HandleSelection);
								break;
							case Device.iOS:
								string sel = await DisplayActionSheet("Report damages?", "Cancel", null, new[] { "Yes", "No" });
								HandleSelection(sel);
								break;
						}
					} else {
						await DisplayAlert("Error","You must select one or more vehicles","OK");
					}
				});
			}));
		}

        private async void HandleSelection(string sel) {
			if (sel == "No")
			{
				navigationAction = AppResource.DELIVERY_COMMENT_RETURN_ACTION;
				await Navigation.PushAsync(new DeliveryConditions(unselectedDeliveryVehicles, selectedDeliveryVehicles, selectedLocID, ViewModel.DeliveryInfo));
			}
			else if (sel == "Yes")
			{
				if (deliveryDamagePage == null)
				{
					navigationAction = AppResource.DELIVERY_DAMAGE_RETURN_ACTION;
					deliveryDamagePage = new DeliveryDamage(unselectedDeliveryVehicles, selectedDeliveryVehicles, selectedLocID, ViewModel.DeliveryInfo);
				}
				await Navigation.PushAsync(deliveryDamagePage);
			}
		}

		private void ResetDamage(){

			foreach (VehicleViewModel v in selectedDeliveryVehicles) {
				ViewModel.RemoveDamagePhotos (v.DatsVehicle.VehicleId, v.DatsVehicle.VIN);
				v.DatsVehicle.ClearTempDamages ();
				v.CheckDamage ();
				v.DatsVehicle.DropOffInspectionNotes = string.Empty;
				ViewModel.DeliveryInfo.Comment = string.Empty;
			}

			ViewModel.RefreshList();

		}
						
		protected override void OnAppearing()
		{
			base.OnAppearing();
			bool hasDamage = false;
			bool hasVehicleComment = false;
			bool hasDeliveryComment = !string.IsNullOrEmpty(ViewModel.DeliveryInfo.Comment);

			foreach (VehicleViewModel v in selectedDeliveryVehicles) {
				if (v.DatsVehicle.GetInspectionDamageCodes (InspectionType.Temp).Any ()) {
					hasDamage = true;
				}

				if (!string.IsNullOrEmpty(v.DatsVehicle.DropOffInspectionNotes)) {
					hasVehicleComment = true;
				}
			}

			if(deliveryDamagePage != null)
			{
				//deliveryDamagePage.RemoveEventListener ();
				deliveryDamagePage = null;
			}

			if (hasDamage || hasVehicleComment || hasDeliveryComment) {
				Device.BeginInvokeOnMainThread(async () => {

					switch (Device.RuntimePlatform)
					{
						case Device.Android:
							DependencyService.Get<IAlert>().ShowAlert(AppResource.DELIVERY_DAMAGE_TITLE, "", new[] { navigationAction, AppResource.DELIVERY_DISCARD_DAMAGE }, HandleDamageSelection);
                            break;
						case Device.iOS:
							string sel = await DisplayActionSheet(AppResource.DELIVERY_DAMAGE_TITLE, AppResource.DELIVERY_DISCARD_DAMAGE, null, new[] { navigationAction });
							HandleDamageSelection(sel);
							break;
					}
				});
			} else {
				ViewModel.ProcessDeliverySelection ();
			}
		}

        private async void HandleDamageSelection(string sel) {
			switch (sel)
			{
				case AppResource.DELIVERY_DAMAGE_RETURN_ACTION:
					deliveryDamagePage = new DeliveryDamage(unselectedDeliveryVehicles, selectedDeliveryVehicles, selectedLocID, ViewModel.DeliveryInfo);
					await Navigation.PushAsync(deliveryDamagePage);
					break;
				case AppResource.DELIVERY_COMMENT_RETURN_ACTION:
					await Navigation.PushAsync(new DeliveryConditions(unselectedDeliveryVehicles, selectedDeliveryVehicles, selectedLocID, ViewModel.DeliveryInfo));
					break;
				case AppResource.DELIVERY_DISCARD_DAMAGE:
					deliveryDamagePage = null;
					ResetDamage();
					break;
			}
        }
			
		public ManageLoadViewModel ViewModel
		{
			get { return (ManageLoadViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public void SelectVehicle(object sender, EventArgs ea) {

			var v = (VehicleViewModel)VehicleList.SelectedItem;
			v.Selected = !v.Selected;
			VehicleList.SelectedItem = null;
		}

	}
}


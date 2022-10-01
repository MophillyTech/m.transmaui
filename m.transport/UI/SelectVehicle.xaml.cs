using System;
using System.Collections.Generic;
using System.Linq;
using m.transport.Utilities;
using m.transport.Domain;
using m.transport.ViewModels;
using m.transport.Interfaces;
using System.Collections.ObjectModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectVehicle : ContentPage
	{
		Action<VehicleViewModel> selected;
		Action nextVin;

		public SelectVehicle(int locID, InspectionType inspectionType, Action<VehicleViewModel> onSelect, Action onNext)
		{
			//using (InsightsManager.TrackTime("SelectVehicle construction"))
			//{
				selected = onSelect;
				nextVin = onNext;
				ViewModel = new SelectVehicleViewModel(locID, inspectionType);

				InitializeComponent();
			//}

			VehicleList.ItemsSource = ViewModel.VehicleList;
		}

		public SelectVehicleViewModel ViewModel
		{
			get { return (SelectVehicleViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public async void Cancel(object sender, EventArgs ea)
		{
//			Device.OnPlatform(Android: 
//				() => MessagingCenter.Send<ContentPage>(this, MessageTypes.DamagesChanged));
			await Navigation.PopModalAsync();
		}

		public void UpdateVehicleList()
		{
			ViewModel.RefreshVehicleList();
		}

		public async void Select(object sender, EventArgs ea)
		{
			//using (InsightsManager.TrackTime("SelectVehicle Select"))
			//{
				if (VehicleList.SelectedItem == null)
				{
					await DisplayAlert("Error", "Please select a vehicle", "OK");
				}
				else
				{
					var vehicleViewModel = (VehicleViewModel)VehicleList.SelectedItem;
					ObservableCollection<DamageViewModel> dmgList;
					if (ViewModel.Type == InspectionType.Loading)
						dmgList = vehicleViewModel.LoadingDamageList;
					else
						dmgList = vehicleViewModel.DeliveryDamageList;

					if (dmgList.Any (i => i.IsDeletable == false)) {
						displayOptionWithDamage (vehicleViewModel);
					} else {
						displayOptionWithNoDamage (vehicleViewModel);
					}

					VehicleList.SelectedItem = null;
				}
			//}
		}

		private async void displayOptionWithDamage(VehicleViewModel vm){
			switch (Device.RuntimePlatform)
			{
				case Device.Android:
					DependencyService.Get<IAlert>().ShowAlert("Existing Damage Found!", "", new[] { "Add/Review Damage", "Cancel" }, (selection) =>
					{
						HandleDamageSelection(vm, selection);
					});
					break;
				case Device.iOS:
					string sel = await DisplayActionSheet("Existing Damage Found!", "Cancel", null, new[] { "Add/Review Damage" });
					HandleDamageSelection(vm, sel);
					break;
			} 
		}

        private void HandleDamageSelection(VehicleViewModel vm, string sel) {
			if (sel == "Cancel")
			{
				return;
			}
			else if (sel == "Add/Review Damage")
			{
				vm.SetVehicleStatus(VehicleStatus.Loading);
				selected(vm);
			}
		}

		private async void displayOptionWithNoDamage (VehicleViewModel vm){
			switch (Device.RuntimePlatform)
			{
				case Device.Android:
					DependencyService.Get<IAlert>().ShowAlert("No Existing Damage Found!", "", new[] { "Add Damage", "Select Next VIN", "Done Selecting", "Cancel" }, (selection) =>
					{
						HandleNoDamageSelection(vm, selection);
					});
					break;
				case Device.iOS:
					string sel = await DisplayActionSheet("No Existing Damage Found!", "Cancel", null, new[] { "Add Damage", "Select Next VIN", "Done Selecting" });
					HandleNoDamageSelection(vm, sel);
					break;
			}
		}

        private void HandleNoDamageSelection(VehicleViewModel vm, string sel) 
        {
			if (sel == "Cancel")
			{
				return;
			}
			else if (sel == "Add Damage")
			{
				vm.SetVehicleStatus(VehicleStatus.Loading);
				selected(vm);
			}
			else if (sel == "Select Next VIN")
			{
				vm.SetVehicleStatus(VehicleStatus.Loading);
				nextVin();
			}
			else if (sel == "Done Selecting")
			{
				vm.SetVehicleStatus(VehicleStatus.Loading);
				selected(null);
			}
		}

	}
}


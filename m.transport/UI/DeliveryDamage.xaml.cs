using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using m.transport.Domain;
using m.transport.Utilities;
using m.transport.ViewModels;
using System.ComponentModel;
using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class DeliveryDamage : ContentPage
	{	

		CustomObservableCollection<VehicleViewModel> unselectedVehicles;

		public DeliveryDamage (CustomObservableCollection<VehicleViewModel> unselectedVehicles, CustomObservableCollection<VehicleViewModel> selectedVehicles, int selectedLocID, DeliveryInfo info)
		{
			this.unselectedVehicles = unselectedVehicles;

			ViewModel = new DeliveryDamageViewModel(selectedVehicles, info);
			InitializeComponent();

			ToolbarItems.Add (new ToolbarItem ("Next", string.Empty, async delegate {
				await Navigation.PushAsync(new DeliveryConditions(unselectedVehicles, selectedVehicles, selectedLocID, ViewModel.DeliveryInfo));
			}));

			// this a workaround for the fact that Android does not fire OnAppearing on subsequent views of a page
			//Device.OnPlatform (Android: () => MessagingCenter.Subscribe<ContentPage> (this, MessageTypes.DamagesChanged, s => Refresh ()));

			//VehicleList.ItemsSource = ViewModel.SelectedVehicles;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.Refresh();
		}

		public DeliveryDamageViewModel ViewModel
		{
			get { return (DeliveryDamageViewModel)BindingContext; }
			set { BindingContext = value; }
		}
	}
}


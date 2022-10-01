using System;
using System.Collections.Generic;
using m.transport.ViewModels;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class LocationDetail : ContentPage
	{	
		public LocationDetail (DatsLocation loc)
		{
			ViewModel = new LocationDetailViewModel(loc);

			InitializeComponent();
		}

		public LocationDetailViewModel ViewModel
		{
			get { return (LocationDetailViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public void PhoneSelected(object sender, EventArgs ea)
		{
			Device.OpenUri (new Uri("tel://" + this.ViewModel.Phone));
		}

		public void ContactPhoneSelected(object sender, EventArgs ea)
		{
			Device.OpenUri (new Uri("tel://" + this.ViewModel.ContactPhone));
		}

		public async void AddressSelected(object sender, EventArgs ea)
		{
			//await Navigation.PushAsync(new MapPage(this.ViewModel.Name, this.ViewModel.Address));
		}

	}
}


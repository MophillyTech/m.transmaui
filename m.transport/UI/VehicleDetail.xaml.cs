using m.transport.ViewModels;
using System;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class VehicleDetail : ContentPage
	{
		public VehicleDetail (VehicleViewModel vehicle, InspectionType inspectionType)
		{
			BindingContext = vehicle;
			InitializeComponent();

			if (inspectionType == InspectionType.Loading)
				VehicleDetailList.ItemsSource = vehicle.LoadingDetailList;
			else
				VehicleDetailList.ItemsSource = vehicle.DeliveringDetailList;

		}

		public void NoHighlight(object sender, EventArgs ea)
		{
			VehicleDetailList.SelectedItem = null;
		}
	
	}
		
}


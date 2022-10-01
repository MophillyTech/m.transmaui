using System;
using System.Collections.Generic;
using m.transport.Models;
using m.transport.Domain;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectDeliverVehicleCellContent : ContentView
	{
		public SelectDeliverVehicleCellContent ()
		{
			InitializeComponent ();
		}
	}

	public class SelectDeliverVehicleCell : ViewCell {

		//		public VehicleCell(DatsVehicle v) {
		//			VehicleCellContent vc = new VehicleCellContent ();
		//			vc.FindByName<Label> ("VIN").Text = v.VIN;
		//			vc.BindingContext = v;
		//			View = vc;
		//		}

		public SelectDeliverVehicleCell () 
		{
			View = new SelectDeliverVehicleCellContent ();
			//Height = 85;
			this.BindingContextChanged += (object sender, EventArgs e) => 
			{
				VehicleViewModel v = (VehicleViewModel) this.BindingContext;
				//Device.OnPlatform(iOS: () => {
				//	if(v == null)
				//		return;
				//	if(v.HasDeliveringDamagePhoto && v.DeliveryDamageList.Count == 1){
				//		Device.OnPlatform(iOS: () => {Height = 90;});
				//	}else{
				//		Height = 50 + (v.DeliveryDamageList.Count * 18);
				//	}

				//});
                if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
                {
                    if (v == null)
                        return;
                    if (v.HasDeliveringDamagePhoto && v.DeliveryDamageList.Count == 1)
                    {
                        if (DeviceInfo.Current.Platform == DevicePlatform.iOS) { Height = 90; };
                    }
                    else
                    {
                        Height = 50 + (v.DeliveryDamageList.Count * 18);
                    }
                }

            };
		}

	}
}
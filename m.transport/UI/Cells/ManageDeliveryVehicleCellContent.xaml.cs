using System;
using System.Collections.Generic;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{

	public partial class ManageDeliveryVehicleCellContent : ContentView
	{
		public ManageDeliveryVehicleCellContent ()
		{
			InitializeComponent ();
		}
			
	}

	public class ManageDeliveryVehicleCell : ExtendedViewCell {
	

		public ManageDeliveryVehicleCell () 
		{
			View = new ManageDeliveryVehicleCellContent ();
			BindingContextChanged += (sender, e) => 
			{
				var v = (VehicleViewModel) BindingContext; 
				//Device.OnPlatform(iOS: () => {
				//	if(v == null)
				//		return;

				//	if(v.HasDeliveringDamagePhoto && v.DeliveryDamageList.Count == 1){
				//		Device.OnPlatform(iOS: () => {Height = 90;});
				//	}else{
				//		Height = 50 + (v.DeliveryDamageList.Count * 18);
				//	}

				//	if (v.DatsVehicle.HasDropOffInspectionNotes)
				//	{
				//		Height = Height + (18 * (v.DatsVehicle.DropOffInspectionNotes.Length / 40 + 1));
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

                    if (v.DatsVehicle.HasDropOffInspectionNotes)
                    {
                        Height = Height + (18 * (v.DatsVehicle.DropOffInspectionNotes.Length / 40 + 1));
                    }
                }

            };
		}

	}
}


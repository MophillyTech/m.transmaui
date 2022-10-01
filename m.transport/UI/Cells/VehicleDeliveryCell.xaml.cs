using System;
using m.transport.ViewModels;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class VehicleDeliveryCell : ExtendedViewCell
	{
		public VehicleDeliveryCell()
		{
			InitializeComponent ();

			//Device.OnPlatform(iOS: () => {OverallLayout.Padding = new Thickness (0, 0, 10, 0);});
            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                OverallLayout.Padding = new Thickness(0, 0, 10, 0);
            }
            BindingContextChanged += (sender, e) =>
			{
				var v = (VehicleViewModel)BindingContext;
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

		public async void VehicleSelected(object sender, EventArgs ea)
		{
			var parent = this.GetContentPage();
			var vehicleViewModel = (VehicleViewModel)this.BindingContext;
			await parent.Navigation.PushAsync(new VehicleDetail(vehicleViewModel, InspectionType.Delivery));
		}
	}

}
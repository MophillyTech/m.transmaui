using System;
using Google.Android.Material.Internal;
using m.transport.Domain;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class DeliveryDamageCellContent : ContentView
	{
		public DeliveryDamageCellContent()
		{
			InitializeComponent();

			var click = new TapGestureRecognizer();
			click.Tapped +=(s,e)=>AddDamageClicked();
			AddLabel.GestureRecognizers.Add(click);
		}

		public async void AddDamageClicked()
		{
			DeliveryDamage parent = (DeliveryDamage) this.GetContentPage();
			CustomNavigationPage inspection = new CustomNavigationPage (new Inspection((VehicleViewModel)this.BindingContext, InspectionType.Delivery));
			await parent.Navigation.PushModalAsync(inspection);
		}
	}

	public class DeliveryDamageCell : ExtendedViewCell
	{
		public DeliveryDamageCell()
		{
			View = new DeliveryDamageCellContent();

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

				//	if (v.DatsVehicle.HasDropOffInspectionNotes)
				//	{
				//		Height = Height + (18 * (v.DatsVehicle.DropOffInspectionNotes.Length / 30 + 1));
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
                        Height = Height + (18 * (v.DatsVehicle.DropOffInspectionNotes.Length / 30 + 1));
                    }
                }
            };
		}

	}
}

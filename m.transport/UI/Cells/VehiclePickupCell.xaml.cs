using System;
using m.transport.ViewModels;
using System.ComponentModel;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class VehiclePickupCell : ExtendedViewCell
	{
		public VehiclePickupCell()
		{
			InitializeComponent ();

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS) { OverallLayout.Padding = new Thickness (0, 0, 10, 0);};

			this.BindingContextChanged += (object sender, EventArgs e) =>
			{
				VehicleViewModel v = (VehicleViewModel)this.BindingContext;
				if(v == null)
					return;
				v.PropertyChanged += OnPropertyChanged;
			};
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			var v = (VehicleViewModel)BindingContext;

			if (v == null)
				return;

			if (e.PropertyName == "Status") {
				object color;
				Application.Current.Resources.TryGetValue("RedBackground", out color);
				ColorBox.Color = (Color) color;
				Status.Text = v.DatsVehicle.VehicleStatus;
			}
		}

		public async void VehicleSelected(object sender, EventArgs ea)
		{
			var parent = this.GetContentPage();
			await parent.Navigation.PushAsync(new VehicleDetail((VehicleViewModel)this.BindingContext, InspectionType.Loading));
		}
	}

}

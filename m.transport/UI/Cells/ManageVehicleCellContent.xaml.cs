using System;
using System.Collections.Generic;
using m.transport.Domain;
using m.transport.ViewModels;
using System.ComponentModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ManageVehicleCellContent : ExtendedViewCell
	{
		public ManageVehicleCellContent()
		{
			InitializeComponent();

			BindingContextChanged += (sender, e) =>
			{
				var v = (VehicleViewModel)BindingContext;
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
				Application.Current.Resources.TryGetValue ("RedBackground", out color);
				Box.Color = (Color)color;
				Status.Text = v.DatsVehicle.VehicleStatus;
			}else if (e.PropertyName == "RemoveManageEvent") {
				System.Diagnostics.Debug.WriteLine ("RemoveManageEvent: " + v.VIN);
				v.PropertyChanged -= OnPropertyChanged;
			} else if (e.PropertyName == "HasDamagePhoto") {
				Device.BeginInvokeOnMainThread(() =>
				{
					DamagePhotoImage.IsVisible = v.HasLoadingDamagePhoto;
				});
			}
		}

		public void RemoveEvent()
		{
//			if (BindingContext != null) {
//				var v = (VehicleViewModel)BindingContext;
//				v.PropertyChanged -= OnPropertyChanged;
//			}
		}
	}
}

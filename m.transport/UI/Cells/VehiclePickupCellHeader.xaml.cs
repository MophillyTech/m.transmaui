using System;
using m.transport.Domain;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class VehiclePickupCellHeader : ViewCell
	{
		public VehiclePickupCellHeader()
		{
			InitializeComponent ();

			var click = new TapGestureRecognizer();
			click.Tapped +=(s,e)=>OnLabelClicked();
			PickupLabel.GestureRecognizers.Add(click);

			click = new TapGestureRecognizer();
			click.Tapped +=(s,e)=>LocationSelected();
			Header.GestureRecognizers.Add(click);
		}

		public async void LocationSelected()
		{
			CurrentLoad parent = (CurrentLoad)this.GetContentPage();
			parent.ShowDetail = true;
			GroupedVehicles gp = (GroupedVehicles)BindingContext;
			DatsLocation loc = gp.Location;
			if (loc != null && loc.LocationId > 0)
			{
				await parent.Navigation.PushAsync(new LocationDetail(loc));
			}
		}

		public void OnLabelClicked() {
			var parent = (CurrentLoad)this.GetContentPage();
			GroupedVehicles gp = (GroupedVehicles)BindingContext;
			DatsLocation loc = gp.Location;
			parent.LoadForLocation(loc == null ? -1 : loc.LocationId);
		}

	}
}
using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
//using Xamarin.Forms.Maps;
using System.Collections.Generic;
using System.Linq;

namespace DAI.POC
{
	public class DealerMap : ContentPage
	{
		Map map;

		public DealerMap (List<Dealer> dealers)
		{
			map = new Map ();
			this.Content = map; 

			Title = "Dealers";

			foreach (Dealer d in dealers) {
				AddPin (d, true);
			}
		}


		public DealerMap (Dealer d)
		{
			map = new Map ();
			this.Content = map; 

			Title = d.Name;

			AddPin (d, false);
		}

		private async void AddPin(Dealer d, bool mult) {

			var positions = (await (new Geocoder()).GetPositionsForAddressAsync(d.Address)).ToList();
			if (!positions.Any())
				return;

			var position = positions.First();

			if (mult) {
				map.MoveToRegion (MapSpan.FromCenterAndRadius (position, Distance.FromMiles (20)));
			} else {
				map.MoveToRegion (MapSpan.FromCenterAndRadius (position,
					Distance.FromMiles (0.1)));
			}

			map.Pins.Add(new Pin
				{
					Label = d.Name,
					Position = position,
					Address = d.Address
				});
		}
	}
}


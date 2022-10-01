using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using DAI.POC;

namespace DAI.POC
{
	public class LoadView : ListView
	{
		public LoadView (Load load) : base()
		{
			this.ItemsSource = load.DealerLoads;
			this.IsGroupingEnabled = true;
			this.GroupDisplayBinding = new Binding ("Dealer.Name");
			this.GroupHeaderTemplate = null;

			this.ItemTemplate = new DataTemplate (typeof(TextCell)) 
				{ Bindings = {
					{ TextCell.TextProperty, new Binding("Description") }, 
					{ TextCell.DetailProperty, new Binding("SubDescription") } ,
					{ TextCell.TextColorProperty, new Binding("StatusColor") }
				}
			};


			this.ItemTapped += HandleItemTapped;
		}

		void HandleItemTapped (object sender, ItemTappedEventArgs e)
		{
			Vehicle v = (Vehicle) e.Item;

			Navigation.PushAsync (new VehicleDetailView (v));
		}
	}
}


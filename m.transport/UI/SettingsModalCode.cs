using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace DAI.POC
{
	public class SettingsModalCode : ContentPage
	{
		public SettingsModalCode ()
		{
			Title = "Settings";

			StackLayout sl = new StackLayout ();

			sl.Padding = new Thickness (20, 50, 20, 50);
			//sl.Children.Add (new Label { Text = "Settings", HorizontalOptions = LayoutOptions.Center });

			TableView tv = new TableView ();

			tv.Root = new TableRoot ();
			tv.Root.Add (new TableSection ());

			tv.Root [0].Add (new EntryCell { LabelColor = Color.FromRgb(0,0,255), Label = "Driver Name", Text = "John Smith" });
			tv.Root [0].Add (new EntryCell { LabelColor = Color.FromRgb(0, 0, 255), Label = "Truck Number", Text = "754" });
			tv.Root [0].Add (new EntryCell { LabelColor = Color.FromRgb(0, 0, 255), Label = "Mileage", Text = "85,754" });

			sl.Children.Add(tv);
			//sl.Children.Add (new BoxView { WidthRequest = 1, HeightRequest = 100 });

//			Button btn = new Button ();
//			btn.Text = "OK";
//			btn.Clicked += delegate {
//				this.Navigation.PopModalAsync();
//			};
//
//			sl.Children.Add (btn);

			ToolbarItems.Add(new ToolbarItem("OK",string.Empty,async delegate() {
				this.Navigation.PopModalAsync();
			}));

			Content = sl;
		}
	}
}


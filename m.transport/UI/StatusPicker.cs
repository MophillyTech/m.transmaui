using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace DAI.POC
{
	public class StatusPicker : ContentPage
	{
		public StatusPicker ()
		{
			Picker picker = new Picker ();

			picker.Title = "Status";

			picker.Items.Add ("Loaded");
			picker.Items.Add ("Loading");
			picker.Items.Add ("Exception");

			Content = picker;
		
		}
	}

	public class StatusPickerCell : ViewCell
	{
		Picker picker;



		public StatusPickerCell()
		{
			StackLayout sl = new StackLayout();
			sl.Orientation = StackOrientation.Horizontal;
			sl.Padding = new Thickness (10, 0, 0, 10);

			var label = new Label ();

			label.Text = "Status";

			label.WidthRequest = 100;
			label.TextColor = Color.FromRgb(0, 0, 255);

			sl.Children.Add (label);

			picker = new Picker ();

			picker.Items.Add ("Loaded");
			picker.Items.Add ("Loading");
			picker.Items.Add ("Exception");

			picker.HorizontalOptions = LayoutOptions.FillAndExpand;

			sl.Children.Add (picker);

			View = sl;
		}

//		protected override void OnBindingContextChanged()
//		{
//			var item = (YourType)BindingContext;
//
//			picker.Items.Clear()
//			picker.Items.Add("HOME");
//			picker.Items.Add("WORK");
//
//			if (item.isWork) picker.SelectedIndex = 1;
//		}
	}
}


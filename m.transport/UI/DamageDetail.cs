using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections;
using System.Collections.Generic;

namespace DAI.POC
{
	public class DamageDetail : ContentPage
	{
		VehicleDamage damage;
		TableView tv;

		public DamageDetail (VehicleDamage vd)
		{
			damage = vd;

			Title = "Damage Detail";

			tv = new TableView ();
			tv.Root = new TableRoot ();
			tv.Root.Add (new TableSection ());

			tv.Root [0].Add (new PickerCell<DamageAreaCode> ("Area", AppData.DamageAreaCodes));
			tv.Root [0].Add (new PickerCell<DamageTypeCode> ("Type", AppData.DamageTypeCodes));
			tv.Root [0].Add (new PickerCell<DamageSeverity> ("Severity", AppData.DamageSeveritys));

			Content = tv;
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			damage.Area = ((PickerCell<DamageAreaCode>)tv.Root [0] [0]).Selected;
			damage.Type = ((PickerCell<DamageTypeCode>)tv.Root [0] [1]).Selected;
			damage.Severity = ((PickerCell<DamageSeverity>)tv.Root [0] [2]).Selected;
		}
	}

	public class PickerCell<T> : ViewCell {

		Picker picker;
		string Label;
		IList data;

		public T Selected { get 
			{ 
				if (picker.SelectedIndex < 0)
					return default(T);
				return (T) data[picker.SelectedIndex];
			}
		}

		public PickerCell (string LabelText, IList data) : base ()
		{
			this.data = data;

			StackLayout sl = new StackLayout();
			sl.Orientation = StackOrientation.Horizontal;
			sl.Padding = new Thickness (10, 0, 0, 10);

			var label = new Label ();

			label.Text = LabelText;

			label.WidthRequest = 100;
			label.TextColor = Color.Blue;

			sl.Children.Add (label);

			picker = new Picker ();

			foreach (var item in data) {
				picker.Items.Add (item.ToString ());
			}

			picker.HorizontalOptions = LayoutOptions.FillAndExpand;

			sl.Children.Add (picker);

			View = sl;

		}
	}
}


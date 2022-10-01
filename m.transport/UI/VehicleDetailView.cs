using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace DAI.POC
{
	public class VehicleDetailView : ContentPage
	{
		Vehicle vehicle;
		StackLayout sl;
		TableView tv;
		DamageTable dt;

		public VehicleDetailView (Vehicle v) : base()
		{
			vehicle = v;

			Title = v.VIN;

			sl = new StackLayout ();

			tv = new TableView ();
			LabelCell status = new LabelCell () { Label = "Status", Text = v.Status };

			status.Tapped += HandleStatusTapped;
			//StackLayout sl = new StackLayout ();
			//sl.Children.Add (new Label { Text = v.Description });

			tv.Root = new TableRoot ();
			tv.Root.Add (new TableSection ());
			tv.Root [0].Add (new LabelCell () { Label = "Make", Text = v.Make });
			tv.Root [0].Add (new LabelCell () { Label = "Model", Text = v.Model });
			tv.Root [0].Add (new LabelCell () { Label = "Year", Text = v.Year.ToString() });
			tv.Root [0].Add (new LabelCell () { Label = "Color", Text = v.Color });
			tv.Root [0].Add (new LabelCell () { Label = "VIN", Text = v.VIN });
			tv.Root [0].Add (new LabelCell () { Label = "Location", Text = v.Location });
			tv.Root [0].Add (new LabelCell () { Label = "Status", Text = v.Status });
			//tv.Root [0].Add (status);
			//tv.Root [0].Add (new StatusPickerCell ());

			//dt = new DamageTable (v);

			//tv.Root.Add (new TableSection ("Damage"));

			TextCell damageCell = new TextCell { Text = "Damage" };
			damageCell.Tapped += delegate {
				Navigation.PushAsync(new DamagePage(v));
			};

			tv.Root [0].Add (damageCell);


			sl.Children.Add (tv);
			//sl.Children.Add (dt);

			Content = sl;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			//dt.Refresh ();
		}

		void HandleStatusTapped (object sender, EventArgs e)
		{
			Navigation.PushModalAsync (new StatusPicker ());
		}
	}
}


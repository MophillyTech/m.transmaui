using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace DAI.POC
{
	public class DamagePage : ContentPage
	{
		Vehicle v;
		TableView dt;

		public DamagePage (Vehicle v)
		{
			Title = "Damages";

			this.v = v;

			/*
			if (v.Damage.Count == 0) {
				VehicleDamage vd = new VehicleDamage ();
				vd.Area = AppData.DamageAreaCodes [0];
				vd.Severity = AppData.DamageSeveritys [0];
				vd.Type = AppData.DamageTypeCodes [0];
				v.Damage.Add (vd);
			}
*/
			dt = new TableView ();

			Content = dt;

			this.ToolbarItems.Add(new ToolbarItem("Add",string.Empty,delegate() {
				v.Status = "Loading";
				var d = new VehicleDamage ();
				v.Damage.Add(d);
				this.Navigation.PushAsync (new DamageDetail (d));
			}));
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			dt.Root = new TableRoot ("Damage");

			var ts = new TableSection ("Damage");

			foreach (VehicleDamage vd in v.Damage) {
				string area = (vd.Area == null ? string.Empty : vd.Area.Description);
				string type = (vd.Type == null ? string.Empty : vd.Type.Description);
				ts.Add (new TextCell { Text = area, Detail = type });
			}

			dt.Root.Add (ts);
		}
	}
}


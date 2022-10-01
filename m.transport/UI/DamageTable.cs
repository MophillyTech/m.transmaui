using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace DAI.POC
{
	public class DamageTable : TableView
	{
		Vehicle v;

		public DamageTable (Vehicle v)
		{
			this.v = v;

			Refresh ();
		}
			
		public void Refresh() {
			this.Root = new TableRoot ("Damage");

			var ts = new TableSection ("Damage");

			foreach (VehicleDamage vd in v.Damage) {
				string area = (vd.Area == null ? string.Empty : vd.Area.Description);
				string type = (vd.Type == null ? string.Empty : vd.Type.Description);
				ts.Add (new TextCell { Text = area, Detail = type });
			}


		}

	}
}


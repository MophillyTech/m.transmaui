using System;
using System.Collections.Generic;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class VehicleDetailCell : ViewCell
	{
		public VehicleDetailCell ()
		{
			InitializeComponent ();
			this.BindingContextChanged += (object sender, EventArgs e) =>
			{
				Details d = (Details)this.BindingContext;
				if(d == null)
					return;
                if (DeviceInfo.Current.Platform == DevicePlatform.iOS) { Height = d.IsDamage ? 80 : 53;};
			};



		}
	}
}


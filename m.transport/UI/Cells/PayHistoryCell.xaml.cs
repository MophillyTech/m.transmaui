using System;
using System.Collections.Generic;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class PayHistoryCell : ExtendedViewCell
	{
		public PayHistoryCell ()
		{
			InitializeComponent ();

			this.BindingContextChanged += (object sender, EventArgs e) =>
			{
				DatsRunHistory d = (DatsRunHistory) BindingContext;
				//Device.OnPlatform(iOS: () => {
				//	Height = 70;
				//});
                if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
                {
                    Height = 70;
                }
            };
		}
	}
}


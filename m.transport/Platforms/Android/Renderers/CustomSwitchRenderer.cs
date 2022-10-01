using System;
using m.transport.Android;
using m.transport;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;

using Android.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRendererAttribute(typeof(CustomSwitch), typeof(CustomSwitchRenderer))]

namespace m.transport.Android
{
	public class CustomSwitchRenderer : SwitchRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Microsoft.Maui.Controls.Switch> e)
		{
			var cSwitch = (CustomSwitch) e.NewElement;

			base.OnElementChanged(e);

			if (Control != null)
			{                       
				Control.TextOn = cSwitch.OnText;
				Control.TextOff = cSwitch.OffText;
			}
		}
	}
}


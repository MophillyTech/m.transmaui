using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using m.transport;
using m.transport.iOS;
using Foundation;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;

//[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace m.transport.iOS
{
	public class CustomPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null)
			{
				var picker = e.NewElement as CustomPicker;
				if (picker != null)
				{
					Control.TextColor = picker.TextColor.ToUIColor();
				}
			}
		}
	}
}
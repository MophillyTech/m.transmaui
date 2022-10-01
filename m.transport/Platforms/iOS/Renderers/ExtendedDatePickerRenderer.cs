using System;
using UIKit;
using m.transport;
using m.transport.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;

//[assembly: ExportRenderer(typeof(DatePicker), typeof(ExtendedDatePickerRenderer))]

namespace m.transport.iOS
{
	public class ExtendedDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				Control.TextAlignment = UITextAlignment.Center;

				if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
				{
					UIDatePicker picker = (UIDatePicker)Control.InputView;
					picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
				}
			}
		}
	}
}


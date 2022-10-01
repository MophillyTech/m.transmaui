using System;
using m.transport.Android;
using m.transport;
using Microsoft.Maui;

using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
//[assembly: ExportRendererAttribute(typeof(ExtendedDatePicker), typeof(ExtendedDatePickerRenderer))]

namespace m.transport.Android
{
	public class ExtendedDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				Control.Gravity = global::Android.Views.GravityFlags.CenterHorizontal;
			}
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using m.transport;
using m.transport.Android;
using Microsoft.Maui;
using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace m.transport.Android
{
	public class CustomPickerRenderer : PickerRenderer
	{
		public CustomPickerRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null)
			{
				var picker = e.NewElement as CustomPicker;
				if (picker != null)
				{
					Control.SetTextColor(picker.TextColor.ToAndroid());
				}
			}
		}
	}
}
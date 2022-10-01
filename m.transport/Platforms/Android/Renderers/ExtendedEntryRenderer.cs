using System;
using m.transport.Android;
using m.transport;
using Android.Views;
using Microsoft.Maui;

using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRendererAttribute(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]

namespace m.transport.Android
{
	public class ExtendedEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				ExtendedEntry entry = ((ExtendedEntry)e.NewElement);

				if(entry.Align == "center")
					Control.Gravity = global::Android.Views.GravityFlags.Center;
				if(entry.AllCap)
					Control.InputType = global::Android.Text.InputTypes.ClassText | global::Android.Text.InputTypes.TextFlagCapWords;

			}
		}
	}
}


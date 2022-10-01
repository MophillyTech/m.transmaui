using System;
using m.transport;
using m.transport.iOS;
using UIKit;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;

//[assembly: ExportRendererAttribute(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]

namespace m.transport.iOS
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
					Control.TextAlignment = UITextAlignment.Center;
				if(entry.AllCap)
					Control.AutocapitalizationType = UITextAutocapitalizationType.Words;
				if (entry.HintTextColor != Color.FromRgb(255,255,255))
				{
					NSAttributedString placeholderString = new NSAttributedString(entry.Placeholder, new UIStringAttributes() { ForegroundColor = entry.HintTextColor.ToUIColor() });
					Control.AttributedPlaceholder = placeholderString;
				}
					
			}
		}
	}
}


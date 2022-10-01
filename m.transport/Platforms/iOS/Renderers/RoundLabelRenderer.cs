using System;
using m.transport;
using m.transport.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Platform;
//[assembly: ExportRenderer(typeof(RoundedLabel), typeof(RoundedLabelRenderer))]
namespace m.transport.iOS
{
	public class RoundedLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				var _xfViewReference = (RoundedLabel)Element;

				// Radius for the curves
				this.Layer.CornerRadius = (float)_xfViewReference.CurvedCornerRadius;

				this.Layer.BackgroundColor = _xfViewReference.CurvedBackgroundColor.ToCGColor();
			}
		}
	}
}

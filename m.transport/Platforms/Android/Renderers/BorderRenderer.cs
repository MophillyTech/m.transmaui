using System;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Widget;
using System.Linq;
using System.ComponentModel;
using m.transport.Android.alpha.Renderers;
using m.transport;
using Microsoft.Maui;
using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRendererAttribute(typeof(Border), typeof(BorderRenderer))]

namespace m.transport.Android.alpha.Renderers
{
	public class BorderRenderer : VisualElementRenderer<Border>
	{
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			//HandlePropertyChanged (sender, e);
            BorderRendererVisual.UpdateBackground(Element, this.ViewGroup);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Border> e)
		{
			base.OnElementChanged(e);
            BorderRendererVisual.UpdateBackground(Element, this.ViewGroup);
		}

		/*void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
            {
                BorderRendererVisual.UpdateBackground (Element, this.ViewGroup);
            }
        }*/

		protected override void DispatchDraw(Canvas canvas)
		{
			if (Element.IsClippedToBorder)
			{
				canvas.Save(SaveFlags.Clip);
				BorderRendererVisual.SetClipPath(this, canvas);
				base.DispatchDraw(canvas);
				canvas.Restore();
			}
			else
			{
				base.DispatchDraw(canvas);
			}
		}
	}
}

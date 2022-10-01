using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Util;
using m.transport;
using m.transport.Android;
using Microsoft.Maui;
using Android.Content;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

//[assembly: ExportRenderer(typeof(RoundedLabel), typeof(RoundedLabelRenderer))]
namespace m.transport.Android
{
	public class RoundedLabelRenderer : LabelRenderer
	{
		private GradientDrawable _gradientBackground;

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

            var view = (RoundedLabel)Element;
			if (view == null) return;

			// creating gradient drawable for the curved background
			_gradientBackground = new GradientDrawable();
			_gradientBackground.SetShape(ShapeType.Rectangle);
			_gradientBackground.SetColor(view.CurvedBackgroundColor.ToAndroid());

			// Thickness of the stroke line
			_gradientBackground.SetStroke(4, view.CurvedBackgroundColor.ToAndroid());

			// Radius for the curves
			_gradientBackground.SetCornerRadius(
				DpToPixels(this.Context,
				Convert.ToSingle(view.CurvedCornerRadius)));

			// set the background of the label
			Control.SetBackground(_gradientBackground);
		}

		/// <summary>
		/// Device Independent Pixels to Actual Pixles conversion
		/// </summary>
		/// <param name="context"></param>
		/// <param name="valueInDp"></param>
		/// <returns></returns>
		public static float DpToPixels(Context context, float valueInDp)
		{
			DisplayMetrics metrics = context.Resources.DisplayMetrics;
			return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
		}
	}
}

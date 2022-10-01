using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
    public class RoundedLabel : Label
    {
		public static readonly BindableProperty CurvedCornerRadiusProperty =
            BindableProperty.Create(nameof(CurvedCornerRadius),typeof(double),typeof(RoundedLabel),12.0);
		
        public double CurvedCornerRadius
		{
			get { return (double)GetValue(CurvedCornerRadiusProperty); }
			set { SetValue(CurvedCornerRadiusProperty, value); }
		}

		public static readonly BindableProperty CurvedBackgroundColorProperty =
	        BindableProperty.Create(nameof(CurvedCornerRadius),typeof(Color),typeof(RoundedLabel),Color.Default);
		
        public Color CurvedBackgroundColor
		{
			get { return (Color)GetValue(CurvedBackgroundColorProperty); }
			set { SetValue(CurvedBackgroundColorProperty, value); }
		}
    }
}

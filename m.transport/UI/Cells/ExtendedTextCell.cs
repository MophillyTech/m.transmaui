using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class ExtendedTextCell : TextCell
	{
		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create<ExtendedTextCell, double>(
				p => p.FontSize, -1);

		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		public static readonly BindableProperty AlwaysBounceVerticalProperty =
			BindableProperty.Create<ExtendedTextCell, bool>(
				p => p.AlwaysBounceVertical, true);

		public bool AlwaysBounceVertical
		{
			get { return (bool)GetValue(AlwaysBounceVerticalProperty); }
			set { SetValue(AlwaysBounceVerticalProperty, value); }
		}

		public static readonly BindableProperty EnableScrollingProperty =
			BindableProperty.Create<ExtendedTextCell, bool>(p => p.EnableScrolling, true);

		public bool EnableScrolling
		{
			get { return (bool)GetValue(EnableScrollingProperty); }
			set { SetValue(EnableScrollingProperty, value); }
		}

		public static readonly BindableProperty CustomHeightProperty =
			BindableProperty.Create<ExtendedTextCell, float>(p => p.CustomHeight, 44);

		public float CustomHeight
		{
			get { return (float)GetValue(CustomHeightProperty); }
			set { SetValue(CustomHeightProperty, value); }
		}

		public static readonly BindableProperty SeparatorColorProperty =
			BindableProperty.Create<ExtendedTextCell, Color>(p => p.SeparatorColor, Color.Transparent);

		public Color SeparatorColor
		{
			get { return (Color)GetValue(SeparatorColorProperty); }
			set { SetValue(SeparatorColorProperty, value); }
		}

		public static readonly BindableProperty SelectColorProperty =
			BindableProperty.Create<ExtendedTextCell, Color>(p => p.SelectColor, Color.Gray);

		public Color SelectColor
		{
			get { return (Color)GetValue(SelectColorProperty); }
			set { SetValue(SelectColorProperty, value); }
		}

		public static readonly BindableProperty DetailColorProperty =
			BindableProperty.Create<ExtendedTextCell, Color>(p => p.DetailColor, Color.Gray);

		public Color DetailColor
		{
			get { return (Color)GetValue(DetailColorProperty); }
			set { SetValue(DetailColorProperty, value); }
		}

		public static readonly BindableProperty AlignProperty =
			BindableProperty.Create<ExtendedTextCell, String>(p => p.Align, "left");

		public String Align
		{
			get { return (String)GetValue(AlignProperty); }
			set { SetValue(AlignProperty, value); }
		}
	}
}


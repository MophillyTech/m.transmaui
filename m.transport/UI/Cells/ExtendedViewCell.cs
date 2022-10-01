using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class ExtendedViewCell : ViewCell
	{
		public ExtendedViewCell ()
		{
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
			BindableProperty.Create<ExtendedTextCell, bool>(
				p => p.EnableScrolling, true);

		public bool EnableScrolling
		{
			get { return (bool)GetValue(EnableScrollingProperty); }
			set { SetValue(EnableScrollingProperty, value); }
		}
	}
}


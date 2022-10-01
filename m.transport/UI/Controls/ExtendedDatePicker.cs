using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class ExtendedDatePicker : DatePicker
	{

		public ExtendedDatePicker ()
		{
		}

		public static readonly BindableProperty AlignProperty =
			BindableProperty.Create<ExtendedTextCell, String>(p => p.Align, "center");

		public String Align
		{
			get { return (String)GetValue(AlignProperty); }
			set { SetValue(AlignProperty, value); }
		}
	}
}


using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class ExtendedEntry : Entry
	{
		public ExtendedEntry ()
		{
		}

		public static readonly BindableProperty AlignProperty =
			BindableProperty.Create<ExtendedEntry, String>(p => p.Align, "center");

		public String Align
		{
			get { return (String)GetValue(AlignProperty); }
			set { SetValue(AlignProperty, value); }
		}

		public static readonly BindableProperty AllCapProperty =
			BindableProperty.Create<ExtendedEntry, bool>(p => p.AllCap, false);

		public bool AllCap
		{
			get { return (bool)GetValue(AllCapProperty); }
			set { SetValue(AllCapProperty, value); }
		}

		public static readonly BindableProperty HintTextColorProperty =
			BindableProperty.Create("HintTextColor", typeof(Color), typeof(ExtendedEntry), Color.Default);

		public Color HintTextColor
		{
			get { return (Color)GetValue(HintTextColorProperty); }
			set { SetValue(HintTextColorProperty, value); }
		}
	}
}


using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class CustomSwitch : Switch
	{
		public CustomSwitch ()
		{
		}

		public static readonly BindableProperty OffTextProperty =
			BindableProperty.Create<CustomSwitch, string>(
				p => p.OffText, "Off");

		public string OffText
		{
			get { return (string)GetValue(OffTextProperty); }
			set { SetValue(OffTextProperty, value); }
		}

		public static readonly BindableProperty OnTextProperty =
			BindableProperty.Create<CustomSwitch, string>(
				p => p.OnText, "On");

		public string OnText
		{
			get { return (string)GetValue(OnTextProperty); }
			set { SetValue(OnTextProperty, value); }
		}
	}
}


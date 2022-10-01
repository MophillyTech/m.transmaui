using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class ToggleButton : Button
	{
		public ToggleButton()
		{
			BorderRadius = 0;
			BorderWidth = 0;
			Clicked += (sender, args) =>
			{
				if (!IsSelected)
				{
					IsSelected = true;
				}
				else
				{
					if (AllowDeselect)
					{
						IsSelected = false;
					}
				}
				IsSelectedChanged(this, new EventArgs());
			};
		}

		/// <summary>
		/// This is a HACK.  I need a place to set things up *after* the binding has happened.  
		/// In WPF world that event would be OnInitialized/OnLoaded.
		/// In Xamarin.Forms there doesn't seem to be a corresponding event.
		/// Once the properties have been set once (probably through the binding system)
		/// their values are remembered and this code doesn't run anymore 
		/// </summary>

		private bool backgroundColorPropertiesSet;
		private bool textColorPropertiesSet;
		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
			if (propertyName == "BackgroundColor" && !backgroundColorPropertiesSet)
			{
				OriginalBackgroundColor = BackgroundColor;
				InvertedTextColor = BackgroundColor;
				backgroundColorPropertiesSet = true;
			}
			if (propertyName == "TextColor" && !textColorPropertiesSet)
			{
				OriginalTextColor = TextColor;
				InvertedBackgroundColor = TextColor;
				textColorPropertiesSet = true;
			}
		}

		public event EventHandler IsSelectedChanged = delegate { };


		public static readonly BindableProperty InvertedBackgroundColorProperty =
			BindableProperty.Create<ToggleButton, Color>(
			p => p.InvertedBackgroundColor, Color.Transparent);
		public Color InvertedBackgroundColor
		{
			get { return (Color)GetValue(InvertedBackgroundColorProperty); }
			set { SetValue(InvertedBackgroundColorProperty, value); }
		}

		
		public static readonly BindableProperty InvertedTextColorProperty =
			BindableProperty.Create<ToggleButton, Color>(
			p => p.InvertedTextColor, Color.Transparent);
		public Color InvertedTextColor
		{
			get { return (Color)GetValue(InvertedTextColorProperty); }
			set { SetValue(InvertedTextColorProperty, value); }
		}


		public static readonly BindableProperty OriginalBackgroundColorProperty =
			BindableProperty.Create<ToggleButton, Color>(
			p => p.OriginalBackgroundColor, Color.Transparent);
		public Color OriginalBackgroundColor
		{
			get { return (Color)GetValue(OriginalBackgroundColorProperty); }
			set { SetValue(OriginalBackgroundColorProperty, value); }
		}

		
		public static readonly BindableProperty OriginalTextColorProperty =
			BindableProperty.Create<ToggleButton, Color>(
			p => p.OriginalTextColor, Color.Transparent);
		public Color OriginalTextColor
		{
			get { return (Color)GetValue(OriginalTextColorProperty); }
			set { SetValue(OriginalTextColorProperty, value); }
		}

		
		public static readonly BindableProperty IsSelectedProperty =
			BindableProperty.Create<ToggleButton, bool>(
				p => p.IsSelected, false);
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		
		public static readonly BindableProperty AllowDeselectProperty =
			BindableProperty.Create<ToggleButton, bool>(
				p => p.AllowDeselect, false);

		public bool AllowDeselect
		{
			get { return (bool)GetValue(AllowDeselectProperty); }
			set { SetValue(AllowDeselectProperty, value); }
		}
	}
}

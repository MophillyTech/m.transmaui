using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class SegmentControl : StackLayout
	{
		public SegmentControl()
		{
			HorizontalOptions = LayoutOptions.FillAndExpand;
			Orientation = StackOrientation.Horizontal;
			Padding = 1.0;
			Spacing = 1.0;
		}

		public event Action<object, int?> SelectedSegmentChanged = delegate { }; 

		public static readonly BindableProperty AllowNullProperty =
			BindableProperty.Create<SegmentControl, bool>(
				p => p.AllowNull, false, BindingMode.OneWay, null, AllowNullPropertyChanged);

		public bool AllowNull
		{
			get { return (bool)GetValue(AllowNullProperty); }
			set { SetValue(AllowNullProperty, value); }
		}

		private static void AllowNullPropertyChanged(BindableObject bindable, bool oldvalue, bool newvalue)
		{
			bool allowNull = newvalue;
			//Debug.WriteLine("AllowNull changed to " + allowNull);
			var self = (SegmentControl)bindable;
			foreach (ToggleButton button in self.Children.OfType<ToggleButton>())
			{
				button.AllowDeselect = allowNull;
			}
		}

		public static readonly BindableProperty SelectedSegmentProperty =
			BindableProperty.Create<SegmentControl, int?>(
				p => p.SelectedSegment, null, BindingMode.OneWay, null, SelectedSegmentPropertyChanged);

		public int? SelectedSegment
		{
			get { return (int?)GetValue(SelectedSegmentProperty); }
			set {
				if (SelectedSegment != value)
				{
					//Debug.WriteLine("SelectedSegment = " + value);
					SetValue(SelectedSegmentProperty, value);
					SelectedSegmentChanged(this, SelectedSegment);
				}
			}
		}

		private static void SelectedSegmentPropertyChanged(BindableObject bindable, int? oldvalue, int? newvalue)
		{
			var self = (SegmentControl)bindable;
			//Debug.WriteLine("SelectedSegment changed to " + newvalue);
			// When the selectedSegment is changed make sure the children reflect this change
			for (int index = 0; index < self.Children.Count; index++)
			{
				var child = (ToggleButton)self.Children[index];
				var selected = (index == self.SelectedSegment);
				child.IsSelected = selected;
			}
			self.SetButtonStates();
		}

		private void SetButtonStates()
		{
			foreach (ToggleButton button in Children.OfType<ToggleButton>())
			{
				SetButtonState(button);
			}
		}

		protected override void OnChildAdded(Element child)
		{
			base.OnChildAdded(child);
			var button = child as ToggleButton;
			if (button != null)
			{
				button.IsSelectedChanged += ButtonOnIsSelectedChanged;
			}
		}
		protected override void OnChildRemoved(Element child)
		{
			var button = child as ToggleButton;
			if (button != null)
			{
				button.IsSelectedChanged -= ButtonOnIsSelectedChanged;
			}
			base.OnChildRemoved(child);
		}

		/// <summary>
		/// This is a HACK.  Similar to the Toggle button class I need a place to set things up 
		/// *after* the binding has happened.  In WPF world that event would be OnInitialized/OnLoaded.
		/// In Xamarin.Forms there doesn't seem to be a corresponding event.
		/// On the advice of Mark Smith, I am hooking LayoutChildren, 
		/// because it happens after things have been setup correctly 
		/// </summary>
		private bool initializeHasBeenCalled;
		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			base.LayoutChildren(x, y, width, height);

			if (!initializeHasBeenCalled)
			{
				if (AllowNull == false && SelectedSegment == null)
				{
					SelectedSegment = 0;
				}
			}
			SetButtonStates();
			initializeHasBeenCalled = true;
		}

		private void ButtonOnIsSelectedChanged(object sender, EventArgs eventArgs)
		{
			var buttonFiringEvent = (ToggleButton) sender;
			int indexOfButtonFiringEvent = -1;
			for (int index = 0; index < Children.Count; index++)
			{
				var child = (ToggleButton)Children[index];
				if (child == buttonFiringEvent)
				{
					indexOfButtonFiringEvent = index;
				}
			}
			if (buttonFiringEvent.IsSelected)
			{
				SelectedSegment = indexOfButtonFiringEvent;
			}
			else if (SelectedSegment == indexOfButtonFiringEvent)
			{
				SelectedSegment = null;
			}
		}

		private void SetButtonState(ToggleButton button)
		{
			double buttonWidth = this.Width / 2;
			//Debug.WriteLine("SetButtonState:" + button.Text + " " + button.IsSelected);
			if (button.IsSelected)
			{
				if (!button.Text.StartsWith("\u25CF "))
				{
					button.Text = "\u25CF " + button.Text;
				}
				button.BackgroundColor = button.OriginalBackgroundColor;
				button.TextColor = button.OriginalTextColor;//ActiveTextColor;
			}
			else
			{
				if (button.Text.StartsWith("\u25CF "))
				{
					button.Text = button.Text.Substring(2);
				}
				button.BackgroundColor = button.InvertedBackgroundColor;//InactiveBackgroundColor;
				button.TextColor = button.InvertedTextColor;
			}

			button.WidthRequest = buttonWidth;
		}

	}
}

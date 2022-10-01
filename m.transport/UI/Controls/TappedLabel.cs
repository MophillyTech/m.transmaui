using System;
using m.transport.ViewModels;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class TappedLabel : Label
	{
		public TappedLabel ()
		{
			var labelTapped = new TapGestureRecognizer();
			labelTapped.Tapped += OnLabelTapped;
			this.GestureRecognizers.Add(labelTapped);
		}

		internal void OnLabelTapped(object sender, EventArgs args)
		{
			MessagingCenter.Send(this, MessageTypes.PhotoDamageReason, ((DamageViewModel) BindingContext));
		}
	}
}


using System;
using m.transport.ViewModels;
using m.transport.Models;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class DmgImage : Image
	{
		
		public DmgImage ()
		{
			var photoTapped = new TapGestureRecognizer();
			photoTapped.Tapped += OnPhotoSelected;
			this.GestureRecognizers.Add(photoTapped);
		}
			
		internal void OnPhotoSelected(object sender, EventArgs args)
		{
			Photo p= (Photo) BindingContext;

			MessagingCenter.Send(this, MessageTypes.NavigatePhotoView, new string[]{((ItemsStackLayout)Parent).Title, p.ImagePath});
		}
	}
}


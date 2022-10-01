using System;
using System.Collections.Generic;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ViewImage
	{
		private string path;

		public ViewImage (string title, string path)
		{
			InitializeComponent ();

			ViewModel = new ViewImageViewModel (){Title = title, ImageSource = path };
			this.path = path;

		}

		async public void DeletePhoto(object sender, EventArgs e) {

			bool delete = await DisplayAlert ("Delete", "Are you sure you want to delete this photo?", "Delete", "Cancel");

			if (delete) {
				MessagingCenter.Send (this, MessageTypes.DeleteDamagePhoto, path);
				Navigation.PopAsync ();
			}
		}

		public ViewImageViewModel ViewModel
		{
			get { return (ViewImageViewModel)BindingContext; }
			set { BindingContext = value; }
		}
	}
}


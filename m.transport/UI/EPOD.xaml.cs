using System;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class EPOD : ContentPage
	{	
		public EPOD ()
		{
			InitializeComponent ();

			ToolbarItems.Add(new ToolbarItem("Submit",string.Empty, async delegate{

				await DisplayAlert("Delivery Complete", "EPOD Has Been Submitted","OK");

				await Navigation.PopToRootAsync();
			}));
		}
	}
}


using System;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class MapPage : ContentPage
	{	
		public MapPage (string name, string address)
		{
			InitializeComponent ();

			Content = new Map ();

		}
	}
}


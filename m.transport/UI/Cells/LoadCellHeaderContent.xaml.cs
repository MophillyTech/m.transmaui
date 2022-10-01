using System;
using System.Collections.Generic;
using m.transport.Models;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class LoadCellHeaderContent : ContentView
	{
		public LoadCellHeaderContent ()
		{
			InitializeComponent ();
		}

	}

	public class LoadCellHeader : ViewCell {

		public LoadCellHeader () {
			View = new LoadCellHeaderContent ();
			Height = 35;
		}
	}
}
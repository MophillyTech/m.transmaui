using System;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectLocationCellContent : ContentView
	{
		public SelectLocationCellContent ()
		{
			InitializeComponent ();
		}
	}

	public class SelectLocationCell : ViewCell {

		public SelectLocationCell () 
		{
			View = new SelectLocationCellContent ();
		}

	}
}


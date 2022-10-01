using System;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{	
	public partial class SelectDeliveryHistoryCellContent : ContentView
	{	
		public SelectDeliveryHistoryCellContent ()
		{
			InitializeComponent ();
		}
	}

	public class SelectDeliveryHistoryCell : ViewCell {

		public SelectDeliveryHistoryCell () 
		{
			View = new SelectDeliveryHistoryCellContent ();
		}

	}
}


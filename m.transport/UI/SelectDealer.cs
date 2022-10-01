using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace DAI.POC
{
	public class SelectDealer : ContentPage
	{
		public SelectDealer (List<Dealer> dealers)
		{
			Title = "Select a Dealer";

			ListView lv = new ListView ();

			lv.ItemsSource = dealers;

			lv.ItemTemplate = new DataTemplate (typeof(TextCell)) { Bindings = { { TextCell.TextProperty, new Binding("Name") } } };

			Content = lv;
		}
	}
}


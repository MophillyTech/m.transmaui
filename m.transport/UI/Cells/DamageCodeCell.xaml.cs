using System;
using System.Collections.Generic;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class DamageCodeContent : ContentView
	{
		public DamageCodeContent()
		{
			InitializeComponent();
		}
	}

	public class DamageCodeCell : ViewCell
	{

		public DamageCodeCell(Object obj)
		{
			var dc = new DamageCodeContent();
			Height = 85;
			//vc.BindingContext = v;

			if (obj.GetType() == typeof(DamageAreaCode))
			{
				dc.FindByName<Label>("code").Text = ((DamageAreaCode)obj).Code;
				dc.FindByName<Label>("description").Text = ((DamageAreaCode)obj).Description;
			}
			else if (obj.GetType() == typeof(DamageTypeCode))
			{
				dc.FindByName<Label>("code").Text = ((DamageTypeCode)obj).Code;
				dc.FindByName<Label>("description").Text = ((DamageTypeCode)obj).Description;
			}

			View = dc;
			//Height = 100;
		}

		public DamageCodeCell()
		{
			View = new DamageCodeContent();
			Height = 85;
			//this.BindingContextChanged += (object sender, EventArgs e) => {
			//		Object v = this.BindingContext;
			//};
		}

	}
}


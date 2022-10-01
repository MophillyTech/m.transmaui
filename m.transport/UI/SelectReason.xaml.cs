using System;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectReason : ContentPage
	{	
		Action<string> complete;
		private bool isMandatory = false;

		public SelectReason (Action<string> complete, List<string> options, string title, bool isMandatory = false)
		{
			this.isMandatory = isMandatory;
			this.complete = complete;
			Title = title;
			InitializeComponent ();

			//Do we really need a cancel button here?
//			ToolbarItems.Add (new ToolbarItem ("Cancel", string.Empty, async delegate {
//				await Navigation.PopModalAsync ();
//			}));

			ReasonList.ItemsSource = options;
		}

		public async void ReasonSelected(object sender, EventArgs ea) {

			if (complete != null) {
				complete ((string)ReasonList.SelectedItem);
			}

			await Navigation.PopModalAsync ();
		}

		protected override bool OnBackButtonPressed (){

			if (isMandatory) {
				DisplayAlert ("Error", "Please select a Photo Reason", "OK");
				return true;
			}

			return false;

		}

	}
}


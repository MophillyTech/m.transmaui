using System;
using System.Collections.Generic;
using m.transport.Data;
using Autofac;
using m.transport.Domain;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class DeliveryHistory : ContentPage
	{	
	
		public DeliveryHistory ()
		{
			ViewModel = new DeliveryHistoryViewModel(App.Container.Resolve<IAppSettingsRepository>());

			InitializeComponent();

			PaperList.ItemsSource = ViewModel.Papers;
			PaperList.ItemTemplate = new DataTemplate(typeof(SelectDeliveryHistoryCell));
		}

		public DeliveryHistoryViewModel ViewModel
		{
			get { return (DeliveryHistoryViewModel) BindingContext; }
			set { BindingContext = value; }
		}

		public void HistorySelected(object sender, EventArgs ea)
		{
			Paper p = (Paper)PaperList.SelectedItem;

			if (p != null) {
				PrintHistory (p);
			}

			PaperList.SelectedItem = null;

		}

		private async void PrintHistory(Paper p)
		{
			bool resp = await DisplayAlert("Delivery History", "Would you like to print the delivery receipt at "  
				                                + p.Location + " on " + p.Time.ToString() + " ?", "Yes", "No");

			if (resp)
			{
				Device.BeginInvokeOnMainThread (async () => {
					if (await this.BeginCallToServerAsync ("Printing Receipt...", offline: true)) {
					}
				});

				bool success = PaperWork.PrintDeliveryHistory (p.Data, p.Offsets);

				this.EndCallToServerAsync ();

				if (success) {
					await Navigation.PopModalAsync ();
				} else {

					await DisplayAlert ("Bluetooth is Turned Off", "To print a report, bluetooth must be turned on in the Settings app.", "OK");
				}
			}
		}

		async void OnActivated(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}


using System;
using System.Collections.Generic;
using m.transport.ViewModels;
using m.transport.Utilities;
using m.transport.Svc;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class PayHistory : ContentPage
	{
		public PayHistory ()
		{
			InitializeComponent ();
			ViewModel = new PayHistoryViewModel ();

			PayList.ItemTemplate = new DataTemplate(typeof(PayHistoryCell));
		}
			
		async void OnDone(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		public PayHistoryViewModel ViewModel
		{
			get { return (PayHistoryViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (ViewModel.PayHistory.Count == 0) {
				GetPayHistory ();
			}

		}

		private async void GetPayHistory()
		{
			if (await this.BeginCallToServerAsync ("Retrieving Pay History...")) {
				ViewModel.GetPayHistoryCompleted += OnGetPayHistoryCompleted;
				ViewModel.GetPayHistoryAsync();
			}
		}

		private void OnGetPayHistoryCompleted(object sender, GetRunListCompletedEventArgs e)
		{
			ViewModel.GetPayHistoryCompleted -= OnGetPayHistoryCompleted;
			this.EndCallToServerAsync(e);

			if(e.Error != null){
				Device.BeginInvokeOnMainThread (() => {
					Navigation.PopModalAsync();
				});
			}
				
		}

		public void RunSelected(object sender, EventArgs ea)
		{
			Device.BeginInvokeOnMainThread(async () => {
				await Navigation.PushAsync(new RunDetails((DatsRunHistory)PayList.SelectedItem));
					PayList.SelectedItem = null;
			});
		}
	}
}


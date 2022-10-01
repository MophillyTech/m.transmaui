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

	public partial class RunDetails : ContentPage
	{
		public RunDetails (DatsRunHistory runHistory)
		{
			InitializeComponent ();
			ViewModel = new RunDetailsViewModel (runHistory);

			RunList.ItemTemplate = new DataTemplate(typeof(RunDetailsCell));
		}
			
		public RunDetailsViewModel ViewModel
		{
			get { return (RunDetailsViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			GetRunDetails ();

		}

		private async void GetRunDetails()
		{
			if (await this.BeginCallToServerAsync ("Retrieving Run Details...")) {
				ViewModel.GetRunDetailsCompleted += OnGetRunDetailCompleted;
				ViewModel.GetRunDetailAsync();
			}
		}

		private void OnGetRunDetailCompleted(object sender, GetRunDetailCompletedEventArgs e)
		{
			ViewModel.GetRunDetailsCompleted -= OnGetRunDetailCompleted;
			this.EndCallToServerAsync(e);

			if(e.Error != null){
				Device.BeginInvokeOnMainThread (async() => {
					await Navigation.PopModalAsync();
				});
			}

		}
	}
}


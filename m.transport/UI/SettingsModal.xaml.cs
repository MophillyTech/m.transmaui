using System;
using m.transport.ViewModels;
using m.transport.Interfaces;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SettingsModal : ContentPage
	{
		public SettingsModal()
		{
			InitializeComponent();
			BindingContext = new SettingsViewModel();
		}

		void OnCancel(object sender, EventArgs e) {
			Device.BeginInvokeOnMainThread (async () => await Navigation.PopModalAsync ());
		}

		void OnActivated(object sender, EventArgs e)
		{
			DependencyService.Get<IHud> ().Show ("Verifying Url");

			((SettingsViewModel)BindingContext).Save (delegate {

				DependencyService.Get<IHud> ().Dismiss();

				Device.BeginInvokeOnMainThread(async () => {
					await DisplayAlert ("Settings Updated", "You will now be logged out.  Login again to apply the new settings.", "OK");
				
					await Navigation.PopModalAsync ();
				});

				((SettingsViewModel)BindingContext).PrepareLogout ();

				}, delegate {

					DependencyService.Get<IHud> ().Dismiss();

					Device.BeginInvokeOnMainThread(async () => {
						await DisplayAlert("Error","That is not a valid service url!","OK");
					});
			});

		}
	}
}

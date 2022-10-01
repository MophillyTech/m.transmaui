using System;
using m.transport.Interfaces;
using m.transport.ViewModels;
using m.transport.Data;
using Autofac;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class About : ContentPage
	{
		public About()
		{
			ViewModel = new AboutViewModel(App.Container.Resolve<ILoginRepository>());
			InitializeComponent();
			var bi = DependencyService.Get<IBuildInfo>();
			var hardware = DependencyService.Get<IHardwareInfo>();
            //Device.OnPlatform(
            //	iOS: () => {
            //		appVersion.Text = bi.Version + " (" + bi.BuildNumber + ")";

            //	},
            //	Android: () => {
            //	//	appVersion.Text = bi.BuildNumber + " (" + bi.Version + ")";
            //		appVersion.Text = bi.Version + " (" + bi.BuildNumber + ")";

            //	});

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
			{
                appVersion.Text = bi.Version + " (" + bi.BuildNumber + ")";
            }else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                appVersion.Text = bi.Version + " (" + bi.BuildNumber + ")";
            }
			
			bundleID.Text = bi.BundleID;
			osVersion.Text = hardware.Version;
		}

		async void OnActivated(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		public AboutViewModel ViewModel
		{
			get { return (AboutViewModel)BindingContext; }
			set { BindingContext = value; }
		}

	}
}

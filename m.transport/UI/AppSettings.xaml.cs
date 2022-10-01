using System;
using m.transport.Interfaces;
using m.transport.ViewModels;
using m.transport.Data;
using Autofac;
using System.Threading.Tasks;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
    public partial class AppSettings : ContentPage
    {
        public AppSettings()
        {
            InitializeComponent();
            ViewModel = new AppSettingsViewModel();
        }

        public AppSettingsViewModel ViewModel
        {
            get { return (AppSettingsViewModel)BindingContext; }
            set { BindingContext = value; }
        }

        async void OnActivated(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        public async void GPSToggled(object sender, EventArgs ea)
        {
            if (((ToggledEventArgs) ea).Value == false)
            {
                bool resp = await DisplayAlert("GPS required", "You will be logged off if GPS is disabled", "Yes", "No");
                if (resp)
                {
                    await Navigation.PopModalAsync();
                    MessagingCenter.Send<AppSettings>(this, MessageTypes.LogOut);
                }
                GPSSwitch.IsToggled = true;
            }
        }
    }
}

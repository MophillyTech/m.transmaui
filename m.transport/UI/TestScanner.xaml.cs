using System;
using System.Collections.Generic;

using m.transport.Interfaces;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class TestScanner : ContentPage
	{
		IBarcode barcode = null;
		private bool scanned = false;
		private char[] ignoreChar = new []{'I', 'O', 'Q' };
        ICamera camera = null;

		public TestScanner ()
		{
			InitializeComponent ();

			ToolbarItems.Add (new ToolbarItem ("Done", string.Empty, delegate {
				Cancel ();
			}));

            camera = DependencyService.Get<ICamera>();
		}

		public async void ScanVIN(object sender, EventArgs ea)
		{
            Button torch = null;

            if (Device.RuntimePlatform == Device.Android) {

                if (barcode == null)
                {
                    barcode = DependencyService.Get<IBarcode>();
                }

                barcode.Scan(delegate (string s)
                {
                    Scanned.Text = s;

                    string msg;
                    string cleanVIN;
                    ScannerUtilities.ValidateVIN(s, out cleanVIN, out msg);

                    if (!String.IsNullOrEmpty(msg)) {
                        DisplayAlert("Error", msg, "OK");
                    }
                    else
                    {
                        VIN.Text = cleanVIN;
                    }
                });
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                if (!DependencyService.Get<ICamera>().CheckPermission())
                {
                    await DisplayAlert("Not authorized", "Camara permission is turned off. Go To Setting > Vehicle Mobile > Privacy to turn camera on. Note, app will restart when permission is changed", "OK");
                }
                else
                {
                    if (barcode == null)
                    {
                        barcode = DependencyService.Get<IBarcode>();
                    }

                    barcode.Scan(delegate (string s)
                    {
                        Scanned.Text = s;

                        string msg;
                        string cleanVIN;
                        ScannerUtilities.ValidateVIN(s, out cleanVIN, out msg);

                        if (!String.IsNullOrEmpty(msg))
                        {
                            DisplayAlert("Error", msg, "OK");
                        }
                        else
                        {
                            VIN.Text = cleanVIN;
                        }
                    });
                }
            }
		}


		private void Cancel()
		{
			Navigation.PopModalAsync ();

		}
	}
}


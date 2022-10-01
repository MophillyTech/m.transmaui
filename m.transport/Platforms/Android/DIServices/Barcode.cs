using System;
using m.transport;
using m.transport.Android;
using System.Threading.Tasks;
using System.Drawing;
using m.transport.Interfaces;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using System.Threading;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Widget;
using Android;
using ZXing.Mobile;
using Microsoft.Maui;
using Microsoft.Maui.Controls;


//[assembly: xf.Dependency(typeof(Barcode))]
namespace m.transport.Android
{

	public class Barcode : IBarcode
	{
		public Barcode ()
		{
		}

        public async void Scan(Action<string> onRead, Action<Exception> onError = null)
        {

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            scanner.UseCustomOverlay = false;

            var result = await scanner.Scan();

            if (result != null)
            {
                onRead(result.Text);
            } else {
                if (onError != null) {
                    onError(new Exception());
                }
            }
        }
    }

}


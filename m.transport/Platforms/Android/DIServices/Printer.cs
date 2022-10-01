using System;
using m.transport.Interfaces;
using m.transport.Android;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using Android.Bluetooth;
using Java.Util;
using ag = Android.Graphics;
using ac = Android.Content;
using ai = Android.InputMethodServices;
using ar = Android.Resource;
using m.transport.Android.DIServices;
using g = Android;
using Android.OS;
using System.Threading;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Printer))]
namespace m.transport.Android
{
	public class Printer : IPrinter
	{
		private BluetoothSocket socket;
		private List<byte> feed = new List<byte> ();
		private List<byte> driverData = new List<byte> ();
		private List<byte> clientData = new List<byte> ();
		private int maxWidth = 400;
		private const int MAX_RETRY = 5;

		public Printer ()
		{
			GetSession ();
		}

		public bool IsPrinterAvailable() {
			return GetSession();
		}

		private bool GetSession() {
			int retryCounter = 0;
			while (retryCounter < MAX_RETRY) {

				try {
					if (socket == null) {
						BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
						if (adapter == null)
							throw new Exception ("No Bluetooth adapter found.");
						if (!adapter.IsEnabled)
							throw new Exception ("Bluetooth adapter is not enabled.");
						BluetoothDevice device = (from bd in adapter.BondedDevices
							where bd.Name == "Star Micronics"
							select bd).FirstOrDefault ();
						if (device == null) {
							retryCounter++;
							Thread.Sleep (3000);
							continue;
						}
						socket = device.CreateRfcommSocketToServiceRecord (UUID.FromString ("00001101-0000-1000-8000-00805f9b34fb"));
						socket.Connect ();
						break;
					} else {
						break;
					}
				} catch (Exception ex) {
					Console.WriteLine ("Printer: " + ex.Message);
					socket = null;
					retryCounter++;
					Thread.Sleep (3000);
				}
			}

			return (socket != null);
		}

		public void PrintText(string text, bool DoubleHeight = false, bool nextLine = true, bool IsBold = false, int margin = 0){
			Byte[] marginCommand = new Byte[] {0x1d, 0x4c, 0x00, 0x00};
			marginCommand[2] = (byte) (margin % 256);
			marginCommand[3] = (byte) (margin / 256);
			feed.AddRange (marginCommand);
			if(IsBold)
				feed.AddRange(new byte[]{0x1b, 0x45, 0x01});
			byte height = 0x00;
			if (DoubleHeight)
				height = 0x11;
			feed.AddRange (new byte[] { 0x1d, 0x21, height});
			feed.AddRange (
				// data
				System.Text.Encoding.ASCII.GetBytes (text));
			//remove bold
			if(IsBold)
				feed.AddRange(new byte[]{0x1b, 0x45, 0x00});
			if (nextLine) {
				feed.Add (0x0a);
			}
		}

		public void PrintBarcode(string text) {

			//TURN HRI character off
			feed.AddRange (new byte[] { 0x1d, 0x48, 0x00});

			//HEIGHT
			feed.AddRange (new byte[] { 0x1d, 0x68, 0x78});

			//WIDTH
			feed.AddRange (new byte[] { 0x1d, 0x77, 0x2});

			//CODE 39
			feed.AddRange (new byte[] { 0x1d, 0x6b, 69, (byte)text.Length});

			//VIN
			feed.AddRange (
				// data
				System.Text.Encoding.ASCII.GetBytes (text));

			feed.AddRange(new byte[] { 0x0a, 0x0a});
		}

		private int PixelBrightness(int r, int g, int b) {
			int level = (r + g + b) / 3;
			return level;
		}

		private ag.Bitmap ScaleImage (ag.Bitmap img)
		{
			ag.Bitmap baseImage = img;
			ag.Bitmap scaledImage;
			int width = img.Width;
			int height = img.Height;
			int scaledWidth = width;
			int scaledHeight = height;
			if (width > maxWidth) {
				scaledHeight = 200;
				scaledWidth = maxWidth;
				if (scaledWidth > width)
					scaledWidth = width;
				scaledImage = ag.Bitmap.CreateScaledBitmap (img, scaledWidth, scaledHeight, false);
			} else {
				scaledImage = baseImage;
			}
			return scaledImage;
		}

		private int PixelIndex(int x, int y, int w) {
			return (y * w) + x;
		}

		public void Close(){

		}
			
		public void PrintSignature(string path, bool isDriver ) {

			if (isDriver) {
				if (driverData.Count > 0) {
					feed.AddRange (driverData);
					return;
				}
			} else {
				if (clientData.Count > 0) {
					feed.AddRange (clientData);
					return;
				}
			}
		
			List<byte> imgData = new List<byte> ();

						/*
			ac.Res.AssetManager mgr = Forms.Context.Assets;
			ag.Bitmap bitmap = null;
			using (StreamReader sr = new StreamReader (mgr.Open ("jka.png"))) {
			var bytes = default(byte[]);
			using (var memstream = new MemoryStream ()) {
			sr.BaseStream.CopyTo (memstream);
			bytes = memstream.ToArray ();
			}
			bitmap = ag.BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length);
			}
			*/
			ag.Bitmap bitmap;

			if (path.Length >= 3 && path.Substring (0, 3) == "sti") {
				ac.Res.Resources res = Xamarin.Forms.Forms.Context.Resources;
				//bitmap = ag.BitmapFactory.DecodeStream (new ResourceLoader ().LoadStream ("sti_signature_embedded"));
				int id = m.transport.Android.alpha.Resource.Drawable.sti_signature_android_printable;
				bitmap = ag.BitmapFactory.DecodeResource (res, id);
				//bitmap = ag.BitmapFactory.DecodeFile ("/res/drawable/sti_signature.png");
			} else {
				bitmap = ag.BitmapFactory.DecodeFile (path);
			}

			ag.Bitmap scaled = ScaleImage (bitmap);

			int j = 0;
			int bitNdx = 0;
			byte pixel = 0x00;

			// command to send data to printer
			imgData.Add (0x1b);
			imgData.Add (0x58);
			imgData.Add (0x34);
			imgData.Add ((byte)(scaled.Width/8));
			imgData.Add ((byte)scaled.Height);

			// examine each pixel in raw data (4 bytes per pixel)
			for (int y = 0; y < scaled.Height; y++ ) {
				for (int x = 0; x < scaled.Width; x++) {
					try {
						int p = scaled.GetPixel (x, y);
						// get the rgba values
						int r = ag.Color.GetRedComponent (p);
						int g = ag.Color.GetBlueComponent (p);
						int b = ag.Color.GetGreenComponent (p);
						int a = ag.Color.GetAlphaComponent(p);
						// this may need some tweaking based on different sample images - but works for sig.png
						// figure the average to determine if a pixel is visible
						//int avg = PixelBrightness(r,g,b);
						int avg = (r + b + g + a) / 4;
						// if it is, set the appropriate bit in the output byte - each output byte will represent 8 pixels
						//Console.WriteLine(x + "," + y + " " + r + " " + g + " " + b + " " + a);
						if (r + g + b + a != 255 * 4) {
						//if ((avg > 0)) {
							byte mask = (byte)(1 << (7 - bitNdx));
							pixel |= mask;
							// this is just a counter of how many bits are set in the output, for debugging
							j++;
						}
						bitNdx++;
						// every 8 bits, add the pixel to the output data
						if (bitNdx == 8) {
							imgData.Add (pixel);
							pixel = 0x00;
							bitNdx = 0;
						}
					} catch (Exception ex) {
					}
				}
			}

			if (isDriver) {
				driverData.AddRange (imgData);
			} else {
				clientData.AddRange (imgData);
			}

			feed.AddRange (imgData);
		}

		public void PrintLogo(int n = 0){
			byte x = (byte)n;
			feed.AddRange (new byte[] { 0x1b, 0x66, x, 0x0c });
		}

		public void SkipLines(int count){
			for (int ndx = 0; ndx < count; ndx++) {
				feed.Add (0x0a);
			}
		}

		public int Length(){
			return feed.Count;
		}

		public void Clear(){
			feed.Clear ();
			driverData.Clear ();
			clientData.Clear ();
		}

		public void PrintData(){

			byte[] pdata = feed.ToArray ();
			int len = pdata.Length;
			if (len > 1024)
				len = 1024;
			bool cont = true;
			int offset = 0;

			try {
				do {
					socket.OutputStream.Write(pdata, offset, len);
					offset += len;
					if (offset >= pdata.Length) cont = false;
					len = pdata.Length - offset;
					if (len > 1024) len = 1024;
					System.Threading.Thread.Sleep(1000);
				} while(cont);

				feed.Clear ();
			} catch (Exception ex) {
				Console.WriteLine ("Printing: " + ex.Message);
				socket = null;
			}
		}

		public void PrintReceipt(byte[] data, byte[] index){

			PowerManager.WakeLock sWalkeLock;
			var pm = PowerManager.FromContext (Forms.Context);

			sWalkeLock = pm.NewWakeLock (WakeLockFlags.Full, "Screen ON");
			sWalkeLock.Acquire ();

			var offSets = Enumerable.Range(0, index.Length / 4)
				.Select(i => BitConverter.ToInt32(index, i * 4))
				.ToList();


			if (data.Length == 0)
				return;

			Console.WriteLine ("Data size: " + data.Length);

			byte[] printdata = feed.ToArray ();
			int offset = 0;
			int length = 0;

			try {

				foreach (int len in offSets) {
					
					if (offset >= data.Length)
						break;

					Console.WriteLine ("Start : " + offset + " len: " + len);
					length = len - offset;
					Console.WriteLine ("length: " + length);
					socket.OutputStream.Write(data, offset, length);
					offset = len;
					System.Threading.Thread.Sleep(2000);
				}
			} catch (Exception ex) {
				Console.WriteLine ("Printing: " + ex.Message);
				socket = null;
			}
			System.Threading.Thread.Sleep(3000);
			sWalkeLock.Release ();
			feed.Clear ();


		}

		public byte[] GetData() {
			return feed.ToArray ();
		}

		public bool CheckStatus(){
			return false;
		}
	}
}

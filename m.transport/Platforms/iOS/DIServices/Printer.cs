using System;
using m.transport.Interfaces;
using m.transport.iOS;
using ExternalAccessory;
using System.Linq;
using System.IO;
using System.Text;	
using System.Collections;
using Foundation;
using System.Threading.Tasks;
using System.Collections.Generic;
using UIKit;
using System.Runtime.InteropServices;
using CoreGraphics;
//using StarPrinter;
using System.Threading;
using System.Runtime.CompilerServices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Microsoft.Maui.Controls.Dependency(typeof(Printer))]
namespace m.transport.iOS
{
	public class Printer : IPrinter
	{
		EASession session;
		EAAccessory acc;
		int maxWidth = 400;
		private List<byte> feed = new List<byte> ();
		private List<byte> driverData = new List<byte> ();
		private List<byte> clientData = new List<byte> ();
		private const int MAX_RETRY = 5;

		const string protocol = "jp.star-m.starpro";

		public Printer ()
		{
			//GetSession ();
		}

		public bool IsPrinterAvailable () { 
			return GetSession ();
			//			bool connected = false;
			//
			//			try {
			//				SMPort port = new SMPort("BT:PRNT Star","mini",10000);
			//				connected = port.IsConnected();
			//				ReleasePort (port);
			//			} catch (Exception ex) {
			//				Console.WriteLine (ex.Message);
			//			}
			//
			//			return connected;
		}

		public void Clear(){
			feed.Clear ();
			driverData.Clear ();
			clientData.Clear ();
		}

		public void PrintData(){

			//Send (feed.ToArray ());

			if (feed.Count == 0)
				return;

			OpenStreams ();

			nint byteswritten = 0;
			byte[] printdata = feed.ToArray ();
			uint len = (uint)printdata.Length;
			if (len > 1024)
				len = 1024;
			bool cont = true;

			do {
				byteswritten += session.OutputStream.Write(printdata, len);
				NSRunLoop.Current.RunUntil(DateExtensions.DateTimeToNSDate(DateTime.Now.AddMilliseconds(1000)));
				if (byteswritten >= feed.Count()) {
					cont = false;
				} else {
					len = (uint)(feed.Count - byteswritten);
					if (len > 1024)
						len = 1024;
					printdata = feed.GetRange((int)byteswritten, (int)len).ToArray();
				}
			} while (cont);

			feed.Clear ();

			CloseStreams ();

			session.Dispose ();
			acc.Dispose ();
			session = null;
			acc = null;
		}

		public void PrintReceipt(byte[] data, byte[] index){
			UIApplication.SharedApplication.IdleTimerDisabled = true;
			var offSets = Enumerable.Range(0, index.Length / 4)
				.Select(i => BitConverter.ToInt32(index, i * 4))
				.ToList();

			feed = data.ToList ();

			if (feed.Count == 0)
				return;

			OpenStreams ();
			NSRunLoop.Current.RunUntil(DateExtensions.DateTimeToNSDate(DateTime.Now.AddMilliseconds(1000)));

			int start = 0;
			int length = 0;

			nint byteswritten = 0;
			byte[] printdata = feed.ToArray ();
			uint len;

			foreach (int offset in offSets) {
				Console.WriteLine ("offset: " + offset);


				length = offset - start;

				len = (uint) length;
				byteswritten += session.OutputStream.Write(printdata, len);
				start = length;

				NSRunLoop.Current.RunUntil(DateExtensions.DateTimeToNSDate(DateTime.Now.AddMilliseconds(5000)));

				if (byteswritten >= feed.Count ())
					break;

				printdata = feed.GetRange((int)byteswritten, (int)(feed.Count - byteswritten)).ToArray();

			}
		}

        [MethodImpl(MethodImplOptions.Synchronized)]
		public void Close(){
            Device.BeginInvokeOnMainThread(() =>
           {
               UIApplication.SharedApplication.IdleTimerDisabled = false;
           });

			if (session == null)
				return;

			CloseStreams ();

			session.Dispose ();
			acc.Dispose ();
			session = null;
			acc = null;

			Console.WriteLine ("Printer Session close");

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

		private int PixelIndex(int width, int x, int y)
		{
			return (y * width) + x;
		}

		private int PixelBrightness(int red, int green, int blue)
		{
			int level = (red + green + blue) / 3;
			return level;
		}

		public UIImage ScaleImage (UIImage img)
		{
			UIImage baseImage = img;
			UIImage scaledImage;

			int width = (int) img.Size.Width;
			int height = (int) img.Size.Height;

			int scaledWidth = width;
			int scaledHeight = height;

			if (width > maxWidth) {

				scaledHeight = 200;
				scaledWidth = maxWidth;

				scaledImage = baseImage.Scale (new CGSize (scaledWidth, scaledHeight));
			} else {
				scaledImage = baseImage;
			}

			return scaledImage;
		}

		public byte[] GetData()
		{
			return feed.ToArray ();
		}

		public void PrintSignature(string path, bool isDriver) {

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

			UIImage scaledImage = ScaleImage (new UIImage (path));

			List<byte> imgData = new List<byte> ();

			int width = (int)scaledImage.Size.Width;
			int height = (int)scaledImage.Size.Height;
			var bytesPerPixel = 4;
			var bytesPerRow = bytesPerPixel * width;
			var bitsPerComponent = 8;

			// command to send data to printer
			imgData.Add (0x1b);
			imgData.Add (0x58);
			imgData.Add (0x34);
			imgData.Add ((byte)(width/8));
			imgData.Add ((byte)height);

			// this will be the raw image data, 4 bytes (RGBA) per pixel
			var rawData = new byte[bytesPerRow * height];

			var handle = GCHandle.Alloc(rawData);

			try
			{
				using (var colorSpace = CGColorSpace.CreateDeviceRGB())
				{
					//Crashes on the next line with an invalid handle exception
					using (var context = new CGBitmapContext(Marshal.UnsafeAddrOfPinnedArrayElement(rawData, 0), width, height, bitsPerComponent, bytesPerRow, colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast))
					{
						context.DrawImage(new CGRect(0, 0, width, height), scaledImage.CGImage);
					}
				}
			}
			finally
			{
				handle.Free();
			}


			//ConvertToMonochromeSteinbergDithering (rawData, width, height, 1.5);

			int j = 0;
			int bitNdx = 0;
			byte pixel = 0x00;

			// examine each pixel in raw data (4 bytes per pixel)
			for (int x = 0; x < rawData.Length; x += 4) {

				// get the rgba values
				int r = (int) rawData [x];
				int g = (int) rawData [x + 1];
				int b = (int) rawData [x + 2];
				int a = (int) rawData [x + 3];


				// this may need some tweaking based on different sample images - but works for sig.png

				// figure the average to determine if a pixel is visible
				int avg = (r + b + g + a) / 4;

				// if it is, set the appropriate bit in the output byte - each output byte will represent 8 pixels
				if ((avg > 0)) {
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

			}

			if (isDriver) {
				driverData.AddRange (imgData);
			} else {
				clientData.AddRange (imgData);
			}

			feed.AddRange (imgData);
		}

		private void OpenStreams() {
			if (session.InputStream.Status != NSStreamStatus.Open) {
				session.InputStream.Open ();
			}

			if (session.OutputStream.Status != NSStreamStatus.Open) {
				session.OutputStream.Open ();
			}
		}

		private void CloseStreams() {
			if (session.InputStream.Status == NSStreamStatus.Open) {
				session.InputStream.Close();
			}

			if (session.OutputStream.Status == NSStreamStatus.Open) {
				session.OutputStream.Close ();
			}
		}

		private bool GetSession() {

			if (session == null) {

				int retryCounter = 0;

				while (retryCounter < MAX_RETRY) {

					var _accessoryList = EAAccessoryManager.SharedAccessoryManager.ConnectedAccessories;

					foreach(var obj in _accessoryList)
					{
						if (obj.ProtocolStrings.Contains (protocol)) {
							acc = obj;
							break;
						}
					}

					if (acc != null) {
						session = new EASession (acc, protocol);
						session.OutputStream.Schedule(NSRunLoop.Current, "kCFRunLoopDefaultMode");
						session.InputStream.Schedule(NSRunLoop.Current, "kCFRunLoopDefaultMode");
						break;
					}

					Thread.Sleep (1000);
					retryCounter++;
				}
			}

			return (session != null);
		}

		private int GetGreyLevel(byte r, byte g, byte b, byte a, double intensity) {

			if (a == 0) {
				return 255;
			}

			double z = (double)r + b + g;
			z = z / 3;

			int grey = (int) (z * intensity);

			if (grey > 255)
				grey = 255;

			return grey;
		}

		private byte[] ConvertToMonochromeSteinbergDithering(byte[] pixels, int width, int height, double intensity)
		{
			int[,] levelMap = new int[width,height];

			for (int y = 0; y < height; y++) {

				if ((y & 1) == 0) {
					for (int x = 0; x < width; x++) {

						int pixelIndex = PixelIndex (width, x, y);

						byte r = pixels [pixelIndex];
						byte g = pixels [pixelIndex + 1];
						byte b = pixels [pixelIndex + 2];
						byte a = pixels [pixelIndex + 3];

						levelMap [x,y] += (255 - GetGreyLevel (r, g, b, a, intensity));

						if (levelMap [x,y] >= 255) {
							levelMap [x,y] -= 255;
							pixels [pixelIndex] = 0;
							pixels [pixelIndex + 1] = 0;
							pixels [pixelIndex + 2] = 0;
						} else {
							pixels [pixelIndex] = 255;
							pixels [pixelIndex + 1] = 255;
							pixels [pixelIndex + 2] = 255;
						}

						int quantErr = levelMap [x,y] / 16;

						if (x < (width - 1)) {
							levelMap [x + 1,y] += quantErr * 7;
						}

						if (y < (height - 1)) {
							levelMap [x,y + 1] += quantErr * 5;

							if (x > 0) {
								levelMap [x - 1,y + 1] += quantErr * 3;
							}

							if (x < (width - 1)) {
								levelMap [x + 1,y + 1] += quantErr;
							}
						}
					}
				} else {

					for (int x = width - 1; x >- 0; x--) {

						int pixelIndex = PixelIndex (width, x, y);

						byte r = pixels [pixelIndex];
						byte g = pixels [pixelIndex + 1];
						byte b = pixels [pixelIndex + 2];
						byte a = pixels [pixelIndex + 3];

						levelMap [x,y] += (255 - GetGreyLevel (r, g, b, a, intensity));

						if (levelMap [x,y] >= 255) {
							levelMap [x,y] -= 255;
							pixels [pixelIndex] = 0;
							pixels [pixelIndex + 1] = 0;
							pixels [pixelIndex + 2] = 0;
						} else {
							pixels [pixelIndex] = 255;
							pixels [pixelIndex + 1] = 255;
							pixels [pixelIndex + 2] = 255;
						}

						int quantErr = levelMap [x,y] / 16;

						if (x > 0) {
							levelMap [x - 1,y] += quantErr * 7;
						}

						if (y < (height - 1)) {
							levelMap [x,y + 1] += quantErr * 5;

							if (x < (width - 1)) {
								levelMap [x + 1,y + 1] += quantErr * 3;
							}

							if (x > 0) {
								levelMap [x - 1,y + 1] += quantErr;
							}
						}
					}
				}
			} 

			return pixels;
		}
			
		//		private void Send(byte[] toPrint)
		//		{
		//			//Set the status to offline because this is a new attempt to print
		//			bool onlineStatus = false;
		//			SMPort sPort = null;
		//			//TRY -> Use the textboxes to check if the port info given will connect to the printer.
		//			try
		//			{
		//
		//				//Very important to only try opening the port in a Try, Catch incase the port is not working
		//				//SMPort port = new SMPort("BT:PRNT Star","mini",10000);
		//				sPort = SMPort.GetPort("BT:PRNT Star","mini",10000);
		//				//GetOnlineStatus() will return a boolean to let us know if the printer was reachable.
		//				onlineStatus = sPort.IsConnected();
		//			}
		//
		//			//CATCH -> If the port information is bad, catch the failure.
		//			catch (Exception ex)
		//			{
		//				if (sPort != null)
		//					ReleasePort(sPort);
		//
		//				throw new Exception(string.Format("Can not connect to the printer address: {0}", "BT:PRNT Star"), ex);
		//			}
		//
		//			//If it is offline, dont setup receipt or try to write to the port.
		//			if (onlineStatus == false)
		//			{
		//				ReleasePort(sPort);
		//				throw new Exception("Printer is offline");
		//			}
		//
		//			byte[] dataByteArray = toPrint; //Encoding.UTF8.GetBytes(toPrint);
		//
		//			//Write bytes to printer
		//			uint amountWritten = 0;
		//			while (dataByteArray.Length > amountWritten)
		//			{
		//				//This while loop is very important because in here is where the bytes are being sent out through StarIO to the printer
		//				var amountWrittenKeep = amountWritten;
		//				try
		//				{
		//					unsafe
		//					{
		//						IntPtr unmanagedPointer = Marshal.AllocHGlobal(dataByteArray.Length);
		//						Marshal.Copy(dataByteArray, 0, unmanagedPointer, dataByteArray.Length);
		//						// Call unmanaged code
		//
		//						amountWritten += sPort.WritePort(unmanagedPointer, amountWritten, (uint)dataByteArray.Length - amountWritten);
		//
		//						Marshal.FreeHGlobal(unmanagedPointer);
		//					}
		//				}
		//				catch (Exception)
		//				{
		//					// error happen while sending data
		//					ReleasePort(sPort);
		//					return;
		//				}
		//
		//				if (amountWrittenKeep == amountWritten) // no data is sent
		//				{
		//					SMPort.ReleasePort(sPort);
		//					return; //exit this entire function
		//				}
		//			}
		//
		//			ReleasePort(sPort);
		//		}
		//
		//		private static void ReleasePort(SMPort sPort)
		//		{
		//			SMPort.ReleasePort(sPort);
		//			sPort = null;
		//		}   
	}
}


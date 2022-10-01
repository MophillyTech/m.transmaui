using System;
using m.transport;
using m.transport.iOS;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using CoreGraphics;
using AVFoundation;
using m.transport.Interfaces;
using CoreFoundation;
using Xamarin;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Barcode))]
namespace m.transport.iOS
{
	public class Barcode : IBarcode
	{
		BarcodeVC vc;

		public Barcode ()
		{
		}

		public void Scan(Action<string> onRead, Action<Exception> onError){

			if (vc == null) {
				vc = new BarcodeVC ();
            }
            vc.onRead = onRead;

            // this is horrible - need to replace this with a test for modal caller
            try {
				// if called from a modal
				UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController.PresentViewController (vc, true, null);
			} catch (Exception ex) {
				// if called from a non-modal
				UIApplication.SharedApplication.Windows [0].RootViewController.PresentViewController (vc, true, null);
			}
		}
	}

	public class BarcodeVC : UIViewController
	{

		AVCaptureSession session;
		AVCaptureMetadataOutput output;
		public Action<string> onRead;
		UIButton btnCancel;
		UIButton btnTorch;
        UIView redLine;
		bool torch;
		AVCaptureVideoPreviewLayer previewLayer;
		MetadataOutputDelegate metadataDelegate;
		AVCaptureDeviceInput input;
		AVCaptureDevice camera;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//Insights.Track ("Scan VC Loaded");

			btnCancel = new UIButton (UIButtonType.RoundedRect);
			btnCancel.Frame = new CGRect(25, 25, 100, 35);
			btnCancel.BackgroundColor = UIColor.White;
			btnCancel.SetTitleColor (UIColor.Black, UIControlState.Normal);
			btnCancel.SetTitle ("Cancel", UIControlState.Normal);
			btnCancel.TouchDown += delegate {
				session.StopRunning();
				DismissViewController (true, null);
			};

			btnTorch = new UIButton (UIButtonType.RoundedRect);
			btnTorch.Frame = new CGRect(150, 25, 100, 35);
			btnTorch.BackgroundColor = UIColor.White;
			btnTorch.SetTitleColor (UIColor.Black, UIControlState.Normal);
			btnTorch.SetTitle ("Flash", UIControlState.Normal);
			btnTorch.TouchDown += delegate {
				if(HasTorch()) {
					torch = !torch;
					SetTorch(torch);
				}
			};

            session = new AVCaptureSession ();
			camera = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			input = AVCaptureDeviceInput.FromDevice (camera);
			session.AddInput (input);
			output = new AVCaptureMetadataOutput ();
			metadataDelegate = new MetadataOutputDelegate ();
			output.SetDelegate (metadataDelegate, DispatchQueue.MainQueue);

			//RectangleF region = new RectangleF (this.View.Bounds.X + 10, this.View.Bounds.Height / 2 - 35, this.View.Bounds.Width - 20, 70);

			//output.RectOfInterest = new RectangleF (region.X / this.View.Bounds.Width, region.Y / this.View.Bounds.Height, region.Width / this.View.Bounds.Width, region.Height / this.View.Bounds.Height);

			session.AddOutput (output);
			output.MetadataObjectTypes = AVMetadataObjectType.QRCode | AVMetadataObjectType.Code39Code | AVMetadataObjectType.Code39Mod43Code | AVMetadataObjectType.UPCECode | AVMetadataObjectType.Code128Code;

			previewLayer = new AVCaptureVideoPreviewLayer (session);
			//var view = new ContentView(UIColor.LightGray, previewLayer, metadataDelegate);
			previewLayer.MasksToBounds = true;
			//previewLayer.VideoGravity = AVCaptureVideoPreviewLayer.GravityResizeAspectFill;
			previewLayer.Frame = UIScreen.MainScreen.Bounds;
			//previewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft | AVCaptureVideoOrientation.LandscapeRight | AVCaptureVideoOrientation.Portrait | AVCaptureVideoOrientation.PortraitUpsideDown;

			//output.RectOfInterest = output.GetMetadataOutputRectOfInterestForRect(region);
			this.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

			this.View.Layer.AddSublayer (previewLayer);

			if (HasTorch ()) {
				View.AddSubview (btnTorch);
			}

			View.AddSubview (btnCancel);

            AddRedLine();


            metadataDelegate.MetadataFound += (s, e) => {
				session.StopRunning ();

				string code = e.StringValue;

				// Type 128 codes have a checkdigit in the first position that should be removed
				//if (e.Type.Contains(new NSString("128"))) {
				//	code = code.Substring(1,code.Length - 1);
				//}

//				if (code.Length > 17) {
//					code = code.Substring(code.Length - 17, 17);
//				}

				if (onRead != null) {
					onRead(code);
				}

				//Insights.Track("Scanned", new Dictionary<string,string>() {{ "Barcode", e.StringValue }});

				DismissViewController(true,null);

			};

			if (!session.Running) {
				session.StartRunning ();
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!session.Running) {
				session.StartRunning ();
			}
		}

        private void AddRedLine()
        {
            int screenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            int screenWidth = (int)UIScreen.MainScreen.Bounds.Width;
            int lineXPos = 0;
            int lineYPos = 0;
            int lineWidth = 0;

            if (previewLayer.Connection.SupportsVideoOrientation)
            {
                switch (UIApplication.SharedApplication.StatusBarOrientation)
                {
                    case UIInterfaceOrientation.Portrait:
                    case UIInterfaceOrientation.PortraitUpsideDown:
                        lineWidth = (int)(screenWidth * .84);
                        break;
                    case UIInterfaceOrientation.LandscapeLeft:
                    case UIInterfaceOrientation.LandscapeRight:
                        lineWidth = screenWidth;
                        break;
                }
            }

            lineXPos = (screenWidth - lineWidth) / 2;
            lineYPos = screenHeight / 2;

            redLine = new UIView();
            redLine.Frame = new CGRect(lineXPos, lineYPos, lineWidth, 2);
            redLine.BackgroundColor = UIColor.FromRGBA(255, 0, 0, 30);

            this.View.AddSubview(redLine);
        }

        public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

            redLine.RemoveFromSuperview();

            previewLayer.Frame = this.View.Bounds;

			if (previewLayer.Connection.SupportsVideoOrientation) {

				AVCaptureVideoOrientation vor = AVCaptureVideoOrientation.Portrait;

				switch (UIApplication.SharedApplication.StatusBarOrientation) {
				case UIInterfaceOrientation.Portrait:
					vor = AVCaptureVideoOrientation.Portrait;
					break;
				case UIInterfaceOrientation.PortraitUpsideDown:
					vor = AVCaptureVideoOrientation.PortraitUpsideDown;
					break;
				case UIInterfaceOrientation.LandscapeLeft:
					vor = AVCaptureVideoOrientation.LandscapeLeft;
					break;
				case UIInterfaceOrientation.LandscapeRight:
					vor = AVCaptureVideoOrientation.LandscapeRight;
					break;
				}

				previewLayer.Connection.VideoOrientation = vor;
			}

            AddRedLine();

        }

		public void SetTorch (bool on)
		{
			try
			{
				NSError err;
				var device = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
				device.LockForConfiguration(out err);
				if (on)
				{
					device.TorchMode = AVCaptureTorchMode.On;
					device.FlashMode = AVCaptureFlashMode.On;
				}
				else
				{
					device.TorchMode = AVCaptureTorchMode.Off;
					device.FlashMode = AVCaptureFlashMode.Off;
				}
				device.UnlockForConfiguration();
				device = null;
				torch = on;
			}
			catch { }
		}
			
		private bool HasTorch() {

			bool hasTorch = false;

			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);

			if (captureDevice != null)
				hasTorch = captureDevice.TorchAvailable;

			return hasTorch;
		}
	}

	public class MetadataOutputDelegate : AVCaptureMetadataOutputObjectsDelegate
	{
		public override void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
		{
			foreach(var m in metadataObjects)
			{
				if(m is AVMetadataMachineReadableCodeObject)
				{
					MetadataFound(this, m as AVMetadataMachineReadableCodeObject);
				}
			}
		}
		public event EventHandler<AVMetadataMachineReadableCodeObject> MetadataFound = delegate {};
	}

}


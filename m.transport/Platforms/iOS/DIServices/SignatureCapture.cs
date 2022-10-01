using System;
using System.Collections.Generic;
using System.Linq;
using m.transport.Interfaces;
using m.transport.iOS;
using Foundation;
using UIKit;
using SignaturePad;
using System.IO;
using CoreGraphics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(SignatureCapture))]
namespace m.transport.iOS
{
	public class SignatureCapture : ISignatureCapture
	{
		SignatureViewController svc;

		public void GetSignature(ILoadAndSaveFiles fileRepo,
			string userName,
			Action onStoredSignature,
			Action<Signature> onSuccess,
			Action<Exception> onError = null)
		{
			svc = new SignatureViewController(fileRepo, userName, onStoredSignature, onSuccess, onError);
			UIApplication.SharedApplication.Windows[0].RootViewController.PresentViewController(new LandscapeLeftNavigationController(svc), true, null);
		}
	}

	public class LandscapeLeftNavigationController : UINavigationController
	{
		public LandscapeLeftNavigationController(UIViewController viewController)
			: base(viewController)
		{
			this.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
		}

		public override bool ShouldAutorotate()
		{
			return true;
		}

		public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
		{
			return UIInterfaceOrientation.LandscapeLeft;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.LandscapeLeft;
		}
	}

	public class CustomSignaturePadView : SignaturePadView
	{
		public event Action OnTouchEnded = delegate { };

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			OnTouchEnded();
		}
	}

	public class SignatureViewController : UIViewController
	{

		CustomSignaturePadView pad;
		private readonly ILoadAndSaveFiles fileRepo;
		private readonly string userName;
		Action storedSignature;
		Action<Signature> success;
		Action<Exception> fail;
		private UIBarButtonItem done;
		private UIBarButtonItem clear;
		private UIBarButtonItem useSignature;
		private bool useStoreSignature = false;

		public SignatureViewController(ILoadAndSaveFiles fileRepo,
			string userName, Action storedSignature, Action<Signature> success, Action<Exception> fail)
		{
			this.fileRepo = fileRepo;
			this.userName = userName;
			this.success = success;
			this.fail = fail;
			this.storedSignature = storedSignature;
			this.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
		}

		public override bool ShouldAutorotate()
		{
			return false;
		}

		public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
		{
			return UIInterfaceOrientation.LandscapeLeft;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.LandscapeLeft;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Signature";

			useSignature = new UIBarButtonItem("Use signature on file", UIBarButtonItemStyle.Done,
				(s, args) =>
				{
					useStoreSignature = true;
					pad.LoadPoints(GetPointsFromBinary());
					UpdateControlButtons();
				});

			done = new UIBarButtonItem(UIBarButtonSystemItem.Done, 
				OnDoneClicked);

			clear = new UIBarButtonItem("Clear", UIBarButtonItemStyle.Done,
				(s, args) => { useStoreSignature = false; pad.Clear(); UpdateControlButtons(); });

			var cancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel,
				OnCancelClicked);

			if (IsDriverSignatureOnFile)
			{
				this.NavigationItem.SetRightBarButtonItem(useSignature, true);
			}
			this.NavigationItem.SetLeftBarButtonItem(cancel, true);

			pad = new CustomSignaturePadView();
			pad.OnTouchEnded += PadOnOnTouchEnded;
			this.View = pad;
		}

		private CGPoint[] GetPointsFromBinary()
		{
			var pointFs = new List<CGPoint>();
			var filename = fileRepo.GetFilePath(userName + ".points.bin");
			if (fileRepo.FileExists(filename))
			{
				byte[] bytes = fileRepo.LoadBinary(filename);
				int index = 0;
				while (index < bytes.Length)
				{
					double x = BitConverter.ToDouble(bytes, index);
					index += 8;
					double y = BitConverter.ToDouble(bytes, index);
					index += 8;
					pointFs.Add(new CGPoint((nfloat)x, (nfloat)y));
				}
			}
			return pointFs.ToArray();
		}

		private void PadOnOnTouchEnded() {
			useStoreSignature = false;
			UpdateControlButtons ();
		}

		private void UpdateControlButtons()
		{
			if (pad.IsBlank)
			{
				if (IsDriverSignatureOnFile)
				{
					this.NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[1] {useSignature}, true);
				}
				else
				{
					this.NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[0] {}, true);
				}
			}
			else
			{
				this.NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[2] { done, clear }, true);
			}
		}

		public bool IsDriverSignatureOnFile {
			get
			{
				return userName != null 
					&& fileRepo.FileExists(userName + ".png"); 
			}
		}

		private void OnCancelClicked(object sender, EventArgs args)
		{
			this.DismissViewController(true, null);
			fail(null);
		}



		private void OnDoneClicked(object sender, EventArgs args)
		{
			if (pad.IsBlank) 
			{
				new UIAlertView ("", "No signature to save.", null, "Okay", null).Show ();
			} 
			else if (useStoreSignature) 
			{
				storedSignature ();
				this.DismissViewController(true, null);
			}
			else
			{
				UIImage img = pad.GetImage(true);
				img = img.Scale(new CGSize(img.Size.Width / 2, img.Size.Height / 2));

				NSData data = img.AsPNG();

				string filename = userName + ".png";
				if (string.IsNullOrEmpty(userName))
				{
					filename = Path.GetTempFileName().Replace(".tmp", ".png");
				}
				else
				{
					filename = fileRepo.GetFilePath(userName + ".points.bin");
					fileRepo.SaveBinary(filename, GetBinaryOfPoints(pad.Points));
					filename = fileRepo.GetFilePath(userName + ".png");
				}
				fileRepo.SaveBinary(filename, data.ToArray());

				this.DismissViewController(true, null);

				if (success != null)
				{
					success(new Signature
					{
						Filename = filename,
						Bytes = data.ToArray(),
					});
				}
			}
		}

		private byte[] GetBinaryOfPoints(CGPoint[] points)
		{
			var bytes = new List<byte>();
			foreach (var point in points)
			{

				double x = (double)point.X;
				double y = (double)point.Y;

				bytes.AddRange(BitConverter.GetBytes(x));
				bytes.AddRange(BitConverter.GetBytes(y));
			}
			return bytes.ToArray();
		}
	}
}


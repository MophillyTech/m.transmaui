using System;
using m.transport;
using m.transport.iOS;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using m.transport.Interfaces;
using System.IO;
using Foundation;
using UIKit;
using CoreGraphics;
using AVFoundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Camera))]
namespace m.transport.iOS
{
	public class Camera : ICamera
	{
		private float MaxRes = 1000.0f;
		private float CompressionFactor = 0.5f;
		private float ThRes = 200.0f;

		//static MediaPicker picker;
		public Camera ()
		{
			//if (picker == null) {
			//	picker = new MediaPicker ();
            //}
		}

		static bool presented = false;

		public bool CheckPermission(){

			AVAuthorizationStatus authStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
			switch (authStatus)
			{
				case AVAuthorizationStatus.Restricted:
				case AVAuthorizationStatus.Denied:
					return false;
				case AVAuthorizationStatus.NotDetermined:
				case AVAuthorizationStatus.Authorized:
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void TakePhoto(Action<string> onRead, Action<Exception> onError, Action onCancel = null){

			if (presented)
				return;

			presented = true;

			//StoreCameraMediaOptions options = new StoreCameraMediaOptions ();

			//options.Directory = "photos";
			//options.Name = Guid.NewGuid ().ToString ();

			//picker.TakePhotoAsync(options).ContinueWith(t => {

			//	if (t.IsFaulted || t.IsCanceled) {
			//		onError(new Exception());
			//		presented = false;
			//	}


			//	MediaFile file = t.Result;
			//	// the path returned in MediaFile is not a "real" path
			//	var urls = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
			//	var p = urls [0].Path;

			//	var fileName = Path.GetFileName(file.Path);
			//	var path = Path.Combine (p, "photos", fileName);
			//	Resize(path);

			//	string thumbName = "th_" + fileName;
			//	var thPath = Path.Combine (p, "photos", thumbName);
			//	thPath = Thumbnail(path, thPath);

			//	presented = false;
			//	onRead(path + "," + thPath);

			//}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private string Thumbnail (string path, string thumbPath)
		{
			using (UIImage img = UIImage.FromFile (path)) {
				
				CGSize oldSize = img.Size;
				nfloat w = oldSize.Width;
				nfloat h = oldSize.Height;

				if (w > h) {
					if (w > ThRes) {
						h = h * (ThRes / w);
						w = ThRes;
					}
				} else {
					if (h > ThRes) {
						w = w * (ThRes / h);
						h = ThRes;
					}
					
				}
				
				CGSize newSize = new CGSize (w, h);

				using (UIImage scaled = img.Scale (newSize)) {
					
					NSData data = scaled.AsJPEG (CompressionFactor);

					File.WriteAllBytes (thumbPath, data.ToArray ());
				}
			}
			return thumbPath;
		}

		private void Resize(string path) {

			using (UIImage img = UIImage.FromFile (path)) {

				CGSize oldSize = img.Size;
				nfloat w = oldSize.Width;
				nfloat h = oldSize.Height;

				if (w > h) {
					if (w > MaxRes) {
						h = h * (MaxRes / w);
						w = MaxRes;
					}
				} else {
					if (h > MaxRes) {
						w = w * (MaxRes / h);
						h = MaxRes;
					}

				}

				CGSize newSize = new CGSize (w, h);

				using (UIImage scaled = img.Scale (newSize)) {

					NSData data = scaled.AsJPEG (CompressionFactor);

					File.WriteAllBytes (path, data.ToArray ());
				}
			}
		}
	}
}


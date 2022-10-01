using System;
using m.transport;
using m.transport.Android;
using System.Threading.Tasks;
using Xamarin.Media;
using m.transport.Interfaces;
using Android.App;
using ac = Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ag = Android.Graphics;
using Autofac;
using System.IO;
using media = Android.Media;
using Android;
using Android.Content.PM;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

//[assembly: XF.Dependency(typeof(Camera))]
namespace m.transport.Android
{
	public class Camera : ICamera
	{
		Action<string> onRead;
		Action<Exception> onError;
		Action onCancel;

		public Camera ()
		{
		}


		public bool CheckPermission(){
			return true;
		}

		public void TakePhoto(Action<string> onRead, Action<Exception> onError, Action onCancel){
			var intent = new ac.Intent (XF.Forms.Context, typeof(CameraActivity));
			this.onRead = onRead;
			this.onError = onError;
			this.onCancel = onCancel;

			// clear old handlers before assigning new one
			CameraActivity.ClearEvents ();
			AttachEvent ();
			XF.Forms.Context.StartActivity (intent);
		}

		private void AttachEvent()
		{
			CameraActivity.OnSuccess += onRead;
			CameraActivity.OnError += onError;
			CameraActivity.OnCancel += onCancel;
			CameraActivity.OnCleanup += DetatchEvent;
		}

		private void DetatchEvent()
		{
			CameraActivity.OnSuccess -= onRead;
			CameraActivity.OnError -= onError;
			CameraActivity.OnCancel -= onCancel;
			CameraActivity.OnCleanup -= DetatchEvent;

		}
	}

	[Activity (Label = "Camera", ConfigurationChanges=ac.PM.ConfigChanges.Orientation | ac.PM.ConfigChanges.ScreenSize)]
	public class CameraActivity : Activity {

		public static event Action<string> OnSuccess;
		public static event Action<Exception> OnError;
		public static event Action OnCancel;
		public static event Action OnCleanup;

		private int MaxRes = 1000;
		private int CompressionFactor = 50;
		private int ThRes = 200;

        const string cameraPermission = Manifest.Permission.Camera;
		const int RequestCameraId = 0;

        MediaPicker picker;

		readonly string[] PermissionsCamera =
		{
            Manifest.Permission.Camera
		};

		// hackish - be sure that any old handlers are cleared
		public static void ClearEvents() {
			CameraActivity.OnSuccess = null;
			CameraActivity.OnError = null;
			CameraActivity.OnCancel = null;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			picker = new MediaPicker (this);
			if (!picker.IsCameraAvailable) {
				//Console.WriteLine ("No camera!");
			}
			else {

				if (global::Android.OS.Build.VERSION.SdkInt < global::Android.OS.BuildVersionCodes.M || CheckSelfPermission(cameraPermission) == (int)Permission.Granted)
				{
                    launchCamera();
                } else
                {
					RequestPermissions(PermissionsCamera, RequestCameraId);
                }
			}

		}

        private void launchCamera() 
        {
			var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
			{
				Name = Guid.NewGuid().ToString(),
				Directory = "photos"
			});

			StartActivityForResult(intent, 1);
        }

		public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			switch (requestCode)
			{
                case RequestCameraId:
					{
						if (grantResults[0] == Permission.Granted)
						{
                            launchCamera();
						}
						else
						{
                            Finish();
						}
					}
					break;
			}
		}
			
		public override void OnBackPressed ()
		{
			Microsoft.Maui.Controls.Application a = App.Current;
			MainPage p = (MainPage)a.MainPage;
			if (p.LoginPage) {
				p.RemoveMessageSubscription ();
				Finish ();
			}else {
				base.OnBackPressed ();
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, ac.Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
				
			try{
				data.GetMediaFileExtraAsync (this).ContinueWith (t => {
					MediaFile file = t.Result;
					Resize(file.Path);
					OnSuccess(file.Path + "," + Thumbnail(file.Path));
					OnCleanup ();
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}catch(ArgumentNullException){
				//capture the cancel action when taking photo
				OnCancel();
				OnCleanup ();
			}
			finally {
				this.Finish ();
			}
		}

		private string Thumbnail (string path)
		{
			string imgPath = path;
			string thumbFile = Path.GetFileName (path);
			string thPath = imgPath.Replace (thumbFile, "th_" + thumbFile);

			ag.Bitmap bitmap = ag.BitmapFactory.DecodeFile (path); 
			ag.Bitmap thBitmap = bitmap.Copy (bitmap.GetConfig(), true);

			int w = thBitmap.Width;
			int h = thBitmap.Height;

			if (w > h) {
				if (w > ThRes) {
					h = ((h * ThRes) / w);
					w = ThRes;
				}
			} else {
				if (h > ThRes) {
					w = ((w * ThRes) / h);
					h = ThRes;
				}
			}
			
			ag.Bitmap resizedBitmap = ag.Bitmap.CreateScaledBitmap (thBitmap, w, h, true);

			var stream = new FileStream (thPath, FileMode.Create);
			resizedBitmap.Compress (ag.Bitmap.CompressFormat.Jpeg, CompressionFactor, stream);
			stream.Close ();

			return thPath;
		}
			
		private void Resize(string path) {

			ag.Bitmap bitmap = ag.BitmapFactory.DecodeFile (path); 

			int w = bitmap.Width;
			int h = bitmap.Height;

			string ori = string.Empty;
			if (w > h) {
				ori = "l";
				if (w > MaxRes) {
					h = ((h * MaxRes) / w);
					w = MaxRes;
				}
			} else {
				ori = "p";
				if (h > MaxRes) {
					w = ((w * MaxRes) / h);
					h = MaxRes;
				}
			}

			ag.Bitmap resizedBitmap = ag.Bitmap.CreateScaledBitmap (bitmap, w, h, true);

			Console.WriteLine ("resized: " + path);

			using (var os = new FileStream(path, FileMode.Truncate,FileAccess.Write))
			{
				resizedBitmap.Compress(ag.Bitmap.CompressFormat.Jpeg, CompressionFactor, os);
			}

			media.ExifInterface exif = new media.ExifInterface (path);
			exif.SetAttribute ("TAG_ORIENTATION", ori == "p" ? media.Orientation.Normal.ToString() : media.Orientation.Rotate90.ToString());
			exif.SaveAttributes ();
		}
	}
}


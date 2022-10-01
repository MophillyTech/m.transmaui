using System;
using System.Collections.Generic;
using System.Linq;
using m.transport.Interfaces;
//using m.transport.Android;
using SignaturePad;
using System.IO;
using System.Drawing;
using Android.App;
using ac = Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ag = Android.Graphics;
using Autofac;
using Android.Content.PM;
using Microsoft.AppCenter.Crashes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: XF.Dependency(typeof(SignatureCapture))]
namespace m.transport.Android
{
	public class SignatureCapture : ISignatureCapture
	{
		public void GetSignature(ILoadAndSaveFiles fileRepo,
			string userName,
			Action onStoredSignature,
			Action<m.transport.Interfaces.Signature> onSuccess,
			Action<Exception> onError = null)
		{
			var intent = new ac.Intent (Application.Context, typeof(SignatureActivity));
			intent.SetFlags (ac.ActivityFlags.NewTask);
			// clear old handlers before assigning new one
			SignatureActivity.ClearEvents ();

			SignatureActivity.OnSuccess += onSuccess;
			SignatureActivity.OnStoredSignature += onStoredSignature;

			intent.PutExtra ("username", userName);
			Application.Context.StartActivity (intent);
		}
	}

	[Activity (Label = "Signature", ScreenOrientation = ScreenOrientation.Landscape,
		 ConfigurationChanges=ac.PM.ConfigChanges.Orientation | ac.PM.ConfigChanges.ScreenSize)]
	public class SignatureActivity : Activity {

		public static event Action<m.transport.Interfaces.Signature> OnSuccess;
		public static event Action OnStoredSignature;
		public static event Action<Exception> OnError;
		private static string userName;
		private ILoadAndSaveFiles fileRepo = App.Container.Resolve<ILoadAndSaveFiles>();
		private static ag.Bitmap img = null;
		private bool storedSignature = false;
		private SignaturePad.SignaturePadView sig = null;
		private int pointCounter;


		// hackish - be sure that any old handlers are cleared
		public static void ClearEvents() {
			SignatureActivity.OnSuccess = null;
			SignatureActivity.OnError = null;
		}

		public override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			if (String.IsNullOrEmpty (userName)) {
				Window.SetTitle ("Customer Signature");
			} else {
				Window.SetTitle ("Driver Signature");
			}

		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			sig = new SignaturePadView (Application.Context);
			sig.StrokeWidth = 5.0f;
			sig.StrokeColor = ag.Color.Black;
			sig.BackgroundColor = ag.Color.White;

			sig.ClearLabel.Text = string.Empty;
            
			userName = this.Intent.GetStringExtra("username");

			Button clear = new Button (Application.Context);
			clear.Text = "Clear";
            clear.SetBackgroundColor(global::Android.Graphics.Color.DarkGray);
            clear.SetTextColor(global::Android.Graphics.Color.White);

            clear.Click += delegate {
				pointCounter = -1;
				img = null;
				storedSignature = false;
				sig.Clear();
			};

            Button store = new Button(Application.Context);
            store.SetBackgroundColor(global::Android.Graphics.Color.DarkGray);
            store.SetTextColor(global::Android.Graphics.Color.White);

            store.Text = "Use signature on file";
			store.Click += delegate {
				if (userName != null && fileRepo.FileExists (userName + ".png")) {
					pointCounter = (sig.Points).Length;
					storedSignature = true;
					sig.Clear();
					sig.LoadPoints(GetPointsFromBinary());
					img = null;
				}else{
					Toast.MakeText (this, "There is no stored signature", ToastLength.Short).Show ();
				}
			};

			bool isSaved = false;

			Button save = new Button (Application.Context);
            save.SetBackgroundColor(global::Android.Graphics.Color.DarkGray);
            save.SetTextColor(global::Android.Graphics.Color.White);

            save.Text = "Save";

			save.Click += delegate {
				if(sig.IsBlank){
					Toast.MakeText (this, "Please Sign", ToastLength.Short).Show ();
					return;
				}
				int pointSize = (sig.Points).Length;
				if(!storedSignature || pointCounter != pointSize) {
					img = sig.GetImage(ag.Color.Black, ag.Color.White, true, false);
					string filename = userName + ".png";
					if (string.IsNullOrEmpty(userName))
					{
						filename = System.IO.Path.GetTempFileName().Replace(".tmp", ".png");
					}
					else
					{
						filename = fileRepo.GetFilePath(userName + ".points.bin");
						fileRepo.SaveBinary(filename, GetBinaryOfPoints(sig.Points));
						filename = fileRepo.GetFilePath(userName + ".png");
					}
					byte[] data = null;
					using (var stream = new MemoryStream()) {
						img.Compress(ag.Bitmap.CompressFormat.Png,100,stream);
						data = stream.ToArray();
						fileRepo.SaveBinary(filename, data);
						Console.WriteLine("sig file saved");
					}
						
					OnSuccess(new m.transport.Interfaces.Signature
					{
						Filename = filename,
						Bytes = data
					});
				}else {
					img = null;
					OnStoredSignature();
				}
					
				this.Finish();

			};
		
			LinearLayout ll = new LinearLayout (Application.Context);

			ll.AddView (clear);
			//only have stored button for driver signature
			if(userName != null)
				ll.AddView (store);
			ll.AddView (save);

            LinearLayout.LayoutParams parms = (LinearLayout.LayoutParams)clear.LayoutParameters;
            parms.SetMargins(10, 10, 10, 10);
            clear.LayoutParameters = parms;
            save.LayoutParameters = parms;
            store.LayoutParameters = parms;

            sig.AddView (ll);
			SetContentView (sig);
		}

		protected override void OnPause ()
		{
			base.OnPause ();

            try
            {
                if (img != null)
                {
                    // the GC might have nuked our bitmap :(
                    if (img.Handle != IntPtr.Zero && !img.IsRecycled)
                        img.Recycle();
                    img.Dispose();
                    img = null;
                }
            } catch (Exception ex) {
                // log this in AppCenter
                Crashes.TrackError(ex);
            }

			GC.Collect ();
				
		}

		private byte[] GetBinaryOfPoints(System.Drawing.PointF[] points)
		{
			var bytes = new List<byte>();
			foreach (var point in points)
			{
				bytes.AddRange(BitConverter.GetBytes(point.X));
				bytes.AddRange(BitConverter.GetBytes(point.Y));
			}
			return bytes.ToArray();
		}

		private PointF[] GetPointsFromBinary()
		{
			var pointFs = new List<PointF>();
			var filename = fileRepo.GetFilePath(userName + ".points.bin");
			if (fileRepo.FileExists(filename))
			{
				byte[] bytes = fileRepo.LoadBinary(filename);
				int index = 0;
				while (index < bytes.Length)
				{
					var x = BitConverter.ToSingle(bytes, index);
					index += 4;
					var y = BitConverter.ToSingle(bytes, index);
					index += 4;
					pointFs.Add(new PointF(x, y));
				}
			}
			return pointFs.ToArray();
		}
	}
}
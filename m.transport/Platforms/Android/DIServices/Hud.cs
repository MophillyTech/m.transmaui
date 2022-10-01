using System;
using AndroidHUD;
using m.transport.Interfaces;
using m.transport.Android;
using Android.App;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Hud))]
namespace m.transport.Android
{
	public class Hud : IHud
	{
		public Hud ()
		{
		}

		public void Show() {
			AndHUD.Shared.Show(Forms.Context);
		}

		public void Show(string message) {
			AndHUD.Shared.Show(Forms.Context,message,-1,MaskType.Clear);
		}

		public void Dismiss() {
			AndHUD.Shared.Dismiss();
		}

		public void ShowToast(string message) {
			AndHUD.Shared.ShowToast(Forms.Context, message, MaskType.Clear, TimeSpan.FromSeconds(3), false);
		}
	}
}


using System;
using BigTed;
using m.transport.Interfaces;
using m.transport.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Hud))]
namespace m.transport.iOS
{
	public class Hud : IHud
	{
		public Hud ()
		{
		}

		public void Show() {
			BTProgressHUD.Show (maskType: ProgressHUD.MaskType.Gradient);
		}

		public void Show(string message) {
			BTProgressHUD.Show (message, maskType: ProgressHUD.MaskType.Gradient);
		}

		public void Dismiss() {
			BTProgressHUD.Dismiss ();
		}

		public void ShowToast(string message) {
			BTProgressHUD.ShowToast(message, ProgressHUD.MaskType.Gradient, false, 3000);
		}
	}
}


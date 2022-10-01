using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public class ExtendedContentPage : ContentPage
	{
		private bool active;

		public ExtendedContentPage () {

		}

		protected async Task onClick(Action action) {

			if (active) {
				Debug.WriteLine("Action Running");
				return;
			}

			Debug.WriteLine("Action Activated");
			active = true;

			action ();

			active = false;
			Debug.WriteLine("Action DeActivated");
		}
	}
}


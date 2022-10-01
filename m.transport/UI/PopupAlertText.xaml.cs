using System;
using System.Collections.Generic;
using m.transport.ViewModels;
using m.transport.Data;
using Autofac;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class PopupAlertText : ContentPage
	{
		Action<int?> onInput;
		private bool byPassInput = false;
		private bool isUser;
		private int? currentValue;
		private int? incrementalLimit;
		public PopupAlertText(string title, string msg, Action<int?> onInput, int? currentValue = null, int? incrementalLimit = null, bool isUser = true)
		{
			this.onInput = onInput;
			this.isUser = isUser;
			this.currentValue = currentValue;
			this.incrementalLimit = incrementalLimit;

			ViewModel = new PopupAlertTextViewModel(title, msg);
			InitializeComponent();

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
					ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty, delegate
						{
							Cancel();
						}));
				}
			

			ToolbarItems.Add(new ToolbarItem("Done", string.Empty, delegate
			{
				Complete();
			}));
			

		}

		private void Complete()
		{
			string input = enterMileage.Text;
			int integerInput;

			if (string.IsNullOrEmpty(input) || !int.TryParse(input, out integerInput) || integerInput <= 0)
			{
				ViewModel.IsInvalidInput = true;
				byPassInput = false;
				enterMileage.Text = string.Empty;
				return;
			}
			if (!byPassInput && integerInput < currentValue && isUser)
			{
				ViewModel.IsInvalidInput = true;
				byPassInput = true;
				enterMileage.Text = string.Empty;
			}
			else if ((!byPassInput && integerInput >= currentValue + incrementalLimit) && isUser)
			{
				ViewModel.IsInvalidInput = true;
				byPassInput = true;
				enterMileage.Text = string.Empty;
                enterMileage.Focus();
			}
			else
			{
				this.onInput(integerInput);
			}
		}

		private void Cancel()
		{
			Navigation.PopModalAsync ();
		
		}

		private void OnKeyboardDone (object sender, EventArgs e)
		{
			Complete ();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// auto display keyboard
			enterMileage.Focus();
		}

		public PopupAlertTextViewModel ViewModel
		{
			get { return (PopupAlertTextViewModel)BindingContext; }
			set { BindingContext = value; }
		}
	}
}


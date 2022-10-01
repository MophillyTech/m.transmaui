using System;
using m.transport.Domain;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class Comment : ContentPage
	{
		Action<string> onInput;

		public Comment(string title, string msg, string comment, Action<string> onInput)
		{
			this.onInput = onInput;
			ViewModel = new DeliveryCommentViewModel(title, msg, comment);
			InitializeComponent();

			BuildToolbar();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// auto display keyboard
			Notes.Focus();
		}

		public DeliveryCommentViewModel ViewModel
		{
			get { return (DeliveryCommentViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (textChangedEventArgs.NewTextValue == null || textChangedEventArgs.NewTextValue.Length == 0)
				return;

			if (textChangedEventArgs.NewTextValue.Length > 140)
			{
				DisplayAlert("Error", "Comment can't exeed 140 characters", "OK");
				Notes.Text = textChangedEventArgs.OldTextValue;
			}
		}

		private void BuildToolbar()
		{

			ToolbarItems.Clear();

			ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty, async () =>
			{
				Notes.Unfocus();
				await Navigation.PopModalAsync();
			}));

			ToolbarItems.Add(new ToolbarItem(" ", string.Empty, async () => { }));

			ToolbarItems.Add(new ToolbarItem("Done", string.Empty, async delegate
			{
				Notes.Unfocus();
				this.onInput(ViewModel.Notes);
			}));
		}
	}
}

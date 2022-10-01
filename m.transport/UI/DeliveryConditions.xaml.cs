using System;
using System.Collections.Generic;
using System.Linq;
using m.transport.Domain;
using m.transport.ViewModels;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class DeliveryConditions : ExtendedContentPage
	{
		CustomObservableCollection<VehicleViewModel> unselectedVehicles;
		CustomObservableCollection<VehicleViewModel> selectedVehicles;

		private double ScreenHeight = -1;
		private double OriginalCommentHeight = -1;

		public DeliveryConditions(CustomObservableCollection<VehicleViewModel> unselectedVehicles, CustomObservableCollection<VehicleViewModel> selectedVehicles, int selectedLocID, DeliveryInfo info)
		{
			this.unselectedVehicles = unselectedVehicles;
			this.selectedVehicles = selectedVehicles;

			ViewModel = new DeliveryConditionViewModel(unselectedVehicles, selectedLocID, info);

			InitializeComponent();

			Attended.IsToggled = true;
			Inspection.IsToggled = false;
			ViewModel.ShowReason = false;

			VehicleList.IsGroupingEnabled = false;
			VehicleList.ItemTemplate = new DataTemplate(typeof(SelectDeliverVehicleCell));
			VehicleList.HasUnevenRows = true;
			VehicleList.ItemsSource = unselectedVehicles;

			DropoffLocationEntry.BackgroundColor = ViewModel.DropLocationEnabled ? Color.FromRgb(255,255,255) : Color.FromRgb(128, 128, 128);

			ToolbarItems.Add(new ToolbarItem("Next", string.Empty, () => {
				Comment.Unfocus();
				if (ViewModel.Validation())
				{
					LocationError.Text = ViewModel.LocationErrorMsg;
					return;
				}

				CustomObservableCollection<VehicleViewModel> reloadedVehicles = new CustomObservableCollection<VehicleViewModel>();

				foreach (VehicleViewModel v in unselectedVehicles)
				{
					if (v.Selected)
						reloadedVehicles.Add(v);
				}

				this.onClick(async delegate () { await Navigation.PushAsync(new CompleteDelivery(reloadedVehicles, selectedVehicles, selectedLocID, ViewModel.DeliveryInfo)); });
			}));

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
				CommentLayout.HeightRequest = 140;
				Comment.HeightRequest = 110;
			}

			displaySurveyToggle();
		}

		public void EntryFocus(object sender, EventArgs ea)
		{
			Comment.Unfocus();
			Navigation.PushModalAsync(new CustomNavigationPage
											  (new Comment("Delivery Comment",
													 "Please Enter Comment",
													 Comment.Text,
													 async delegate (string comment)
													 {
														 Comment.Text = comment;
														 await Navigation.PopModalAsync();
													 })));

		}

		public void SurveyEntryFocus(object sender, EventArgs ea)
		{
			Comment.Unfocus();
			Navigation.PushModalAsync(new CustomNavigationPage
											  (new Comment("Survey Comment",
													 "Please Enter Comment",
													 SurveyComment.Text,
													 async delegate (string comment)
													 {
														 SurveyComment.Text = comment;
														 await Navigation.PopModalAsync();
													 })));

		}

		private void displaySurveyToggle()
		{
			foreach (VehicleViewModel v in selectedVehicles)
			{
				if (v.DatsVehicle.SafeDeliveryPromptRequiredInd == 1)
				{
					SurveyLayout.IsVisible = true;
					break;
				}
			}

			if (SurveyLayout.IsVisible)
			{
				Device.BeginInvokeOnMainThread(async () => {
					bool resp = await DisplayAlert("Survey Required", "Was there a safe unloading area?", "Yes", "No");
					if (!resp)
					{
						SelectSurvey(null, null);
						DisplaySurvey(true);
					}
					else
					{
						Survey.IsToggled = resp;
					}
				});
			}
		}

		public void SurveyToggled(object sender, EventArgs ea)
		{
			DisplaySurvey(!Survey.IsToggled);
		}

		private void DisplaySurvey(Boolean isVisible)
		{
			ViewModel.ShowSurvey = isVisible;
			SurveyCommentLayout.IsVisible = (ViewModel.ShowSurvey && Response.Text == "Other") ? true : false;
		}

		public void AttendedToggled(object sender, EventArgs ea)
		{
			if (Attended.IsToggled)
			{
				Inspection.IsEnabled = true;
			}
			else
			{
				Inspection.IsEnabled = false;
				Inspection.IsToggled = true;
			}

			displayReason();

			ViewModel.DeliveryInfo.Attended = Attended.IsToggled;
		}

		public void ReloadToggled(object sender, EventArgs ea)
		{
			if (Reload.IsToggled)
			{
				VehicleList.HeightRequest = 48 * unselectedVehicles.Count;
			}
			VehicleList.IsVisible = Reload.IsToggled && unselectedVehicles.Count > 0;
		}

		public void InspectionToggled(object sender, EventArgs ea)
		{
			displayReason();

			ViewModel.DeliveryInfo.LoadInspection = Inspection.IsToggled;
		}

		private void displayReason()
		{
			ViewModel.ShowReason = Attended.IsToggled && Inspection.IsToggled;
		}

		public async void SelectSurvey(object sender, EventArgs ea)
		{
			var selectReasonPage = new SelectReason(delegate (string r)
			{
				Response.Text = r;
				ViewModel.DeliveryInfo.UnsafeDeliveryResponse = r;

				if (r == "Other")
				{
					SurveyCommentLayout.IsVisible = true;
				}
				else
				{
					SurveyCommentLayout.IsVisible = false;
					ViewModel.MissingSurvey = false;
				}
			}, ViewModel.DeliveryResponses(), "Select Survey Reason");
			await Navigation.PushModalAsync(new CustomNavigationPage(selectReasonPage));
		}

		public void OnSurveyTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (textChangedEventArgs.NewTextValue == null || textChangedEventArgs.NewTextValue.Length == 0)
			{
				ViewModel.MissingSurvey = true;
				return;
			}

			if (textChangedEventArgs.NewTextValue.Length > 140)
			{
				DisplayAlert("Error", "Survey can't exeed 140 characters", "OK");
				SurveyComment.Text = textChangedEventArgs.OldTextValue;
				ViewModel.DeliveryInfo.UnsafeDeliveryNotes = textChangedEventArgs.OldTextValue;
			}
			else
			{
				ViewModel.DeliveryInfo.UnsafeDeliveryNotes = textChangedEventArgs.NewTextValue;
			}
		}


		public async void SelectReason(object sender, EventArgs ea)
		{
			List<string> options = new List<string> {
				"Close To Cutoff Time",
				"Damage Dispute",
				"Other",
				"No Staff Available",
				"Did Not Want Vehicle(s)",
				"Inclement Weather"
			};


			var selectReasonPage = new SelectReason(delegate (string r)
			{

				Reason.Text = r;
				ViewModel.DeliveryInfo.Reason = r;
				ViewModel.MissingReason = false;
			}, options, "Select Delay Reason");
			await Navigation.PushModalAsync(new CustomNavigationPage(selectReasonPage));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.MissingReason = false;
			ViewModel.MissingSurvey = false;

			foreach (VehicleViewModel v in selectedVehicles)
			{
				if (v.DatsVehicle.VehicleStatus != VehicleStatus.Delivering.ToString())
				{
					break;
				}

				v.DatsVehicle.ClearDamage(InspectionType.Delivery);
				v.SetVehicleStatus(VehicleStatus.Loaded);
			}

		}

		public DeliveryConditionViewModel ViewModel
		{
			get { return (DeliveryConditionViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public void SelectVehicle(object sender, EventArgs ea)
		{

			var v = (VehicleViewModel)VehicleList.SelectedItem;
			v.Selected = !v.Selected;
			VehicleList.SelectedItem = null;
		}

		public void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (textChangedEventArgs.NewTextValue == null || textChangedEventArgs.NewTextValue.Length == 0)
				return;

			if (textChangedEventArgs.NewTextValue.Length > 140)
			{
				DisplayAlert("Error", "Comment can't exeed 140 characters", "OK");
				Comment.Text = textChangedEventArgs.OldTextValue;
			}
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			base.LayoutChildren(x, y, width, height);

			if (ScreenHeight < 0)
			{
				ScreenHeight = height;
			}

			if (ScreenHeight > 0)
			{
				DeliveryLayout.HeightRequest = ScreenHeight - CommentLayout.Height;
			}
		}
	}
}


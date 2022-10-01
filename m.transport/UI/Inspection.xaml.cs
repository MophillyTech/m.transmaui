using System;
using System.Threading.Tasks;
using System.Windows.Input;
using m.transport.Utilities;
using m.transport.Domain;
using m.transport.ViewModels;
using System.Linq;
using m.transport.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using m.transport.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class Inspection : ContentPage
	{
		private IList<Photo> photoAddList = new List<Photo> ();
		private IList<Photo> photoDeleteList = new List<Photo> ();
		private InspectionType type;
		private bool isSubscribed = false;
		private bool first = true;

		private DelegateCommand<DamageViewModel> deleteDamageCommand;

		public Inspection(VehicleViewModel v, InspectionType inspectionType, DatsLocation loc = null)
		{
			type = inspectionType;

			deleteDamageCommand = new DelegateCommand<DamageViewModel>(DeleteDamage);
			ViewModel = new InspectionViewModel(v, inspectionType, deleteDamageCommand, loc);

			BuildToolbar();
			InitializeComponent();

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					CommentLayout.HeightRequest = 140;
					CommentEditor.HeightRequest = 110;
                    break;
			}
		}

		async protected override void OnAppearing()
		{
			base.OnAppearing();
			if(!isSubscribed)
				SubscribeMessage ();

			// 6030 - if no prior damages exist, automatically send them into the Add Damage WF
			if (first && !ViewModel.Damages.Any()) {
				first = false;
				await PushDamageWorkflow ();
			} 
		}

		public void SetVehicle(VehicleViewModel v)
		{
			first = true;
			ViewModel.SetVehicle (v);
			VehicleTitle.Text = ViewModel.VehicleHeader;
		}

		private void SubscribeMessage()
		{
			isSubscribed = true;

			MessagingCenter.Subscribe<DmgImage, string []> (this, MessageTypes.NavigatePhotoView, async (sender, arg) => await NavigateToPhotoViewer(arg));
			MessagingCenter.Subscribe<SelectDamageSeverity, string> (this, MessageTypes.CreateDamage, (sender, arg) => CreateDamage(arg));
			MessagingCenter.Subscribe<TappedLabel, DamageViewModel> (this, MessageTypes.PhotoDamageReason, (sender, arg) => SelectReason(arg));
			MessagingCenter.Subscribe<ViewImage, string> (this, MessageTypes.DeleteDamagePhoto, async (sender, arg) => await DeleteDamagePhoto (arg));
		}

		private async Task UnsubscribeMessage()
		{
            await Task.Run(() => {
				isSubscribed = false;

				MessagingCenter.Unsubscribe<DmgImage, string[]>(this, MessageTypes.NavigatePhotoView);
				MessagingCenter.Unsubscribe<SelectDamageSeverity, string>(this, MessageTypes.CreateDamage);
				MessagingCenter.Unsubscribe<TappedLabel, DamageViewModel>(this, MessageTypes.PhotoDamageReason);
				MessagingCenter.Unsubscribe<ViewImage, string>(this, MessageTypes.DeleteDamagePhoto);
            });
		}

		private void EnableDamagePhotoAction (DamageViewModel d)
		{
			if (ViewModel.Vehicle.DatsVehicle.DamagePhotoRequiredInd == 1 &&
			    !d.Photos.Any () && string.IsNullOrEmpty (d.PhotoReason)) {
				ShowPhotoAction (d);
            } else {
                ViewModel.AddDamage(d);
            }
		}
			
		public async Task DeleteDamagePhoto(string path) {

			DamageViewModel found = null;
			Photo photoFound = null;
            // could reduce this with Linq

            await Task.Run(async () =>
            {
                foreach (DamageViewModel dvm in ViewModel.Damages)
                {
                    foreach (Photo p in dvm.Photos)
                    {
                        if (p.ImagePath == path)
                        {
                            photoFound = p;
                            found = dvm;
                            break;
                        }
                    }
                }

                if (found != null && photoFound != null)
                {

                    ViewModel.Modified = true;

                    if (photoAddList.Contains(photoFound))
                    {
                        photoAddList.Remove(photoFound);
                        await DeleteStoredPhoto(photoFound);
                    }
                    else
                        photoDeleteList.Add(photoFound);

                    found.Photos.Remove(photoFound);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DetermineCurrentState(found);
                    });
                }
            });
		}

		private async void DetermineCurrentState(DamageViewModel dvm)
		{
			if (ViewModel.Vehicle.DatsVehicle.DamagePhotoRequiredInd == 1 && dvm.Photos.Count == 0) {
				if (dvm.PhotoReason.Length > 0) {
					dvm.SetPhotoVisbility (false);
					await Refresh ();
				}else
					ShowPhotoAction (dvm, !dvm.NewDamage);
			} else {
				await Refresh ();
			}
		}

		private async void CreateDamage(string dmgCode)
		{
            var d = new DamageViewModel(dmgCode, type, DeleteDamageCommand) { IsDeletable = true };
            EnableDamagePhotoAction(d);
		}

		public async void ShowPhotoAction(DamageViewModel dvm, bool inital = false)
		{
			var actionSheetOptions = new List<String> {
						AppResource.TAKE_PHOTO,
						AppResource.UNABLE_TO_TAKE_PHOTO
			};
			if (inital)
			{
				actionSheetOptions.Add(AppResource.RESTORE_CHANGE);
			}

			switch (Device.RuntimePlatform)
			{
				case Device.Android:
                    actionSheetOptions.Add(("Cancel"));
                    DependencyService.Get<IAlert>().ShowAlert(AppResource.PHOTO_REQUIRED, "", actionSheetOptions.ToArray(), (selection) =>
					{
						handleSelection(dvm, selection);
					});
					break;
				case Device.iOS:
					string result = await DisplayActionSheet(AppResource.PHOTO_REQUIRED, AppResource.CANCEL_DAMAGE, null, actionSheetOptions.ToArray());
					handleSelection(dvm, result);
					break;
			}
		}

		public async void handleSelection(DamageViewModel dvm, string result)
		{
            bool resp = false;
			switch (result) {
				default:
					DeleteDamage (dvm);
					break;
				case AppResource.TAKE_PHOTO:
					resp = await NavigateToCamera (dvm, true);

                    // they cancelled
                    if (!resp) {
                        DetermineCurrentState(dvm);
                    }
					break;
				case AppResource.UNABLE_TO_TAKE_PHOTO:
					SelectReason (dvm);
					break;
				case AppResource.RESTORE_CHANGE:
					RevertChange (dvm);
					break;
			}
		}

		private async void RevertChange(DamageViewModel dvm){

			if (dvm.OriginalPhotos.Count == 0) {
				dvm.SetPhotoVisbility(false);
				dvm.PhotoReason = dvm.OriginalPhotoReason;
			} else {
				dvm.Photos = new ObservableCollection<Photo> (dvm.OriginalPhotos);
				foreach(Photo p in dvm.Photos) {
					photoDeleteList.Remove (p);
				}
			}
				
			await Refresh ();
		}

		public async void SelectReason(DamageViewModel dvm)
		{
			if (dvm.Photos.Count > 0)
				return;

			var selectReasonPage = new SelectReason(async delegate(string r)
				{
					dvm.PhotoReason = r;
                    ViewModel.AddDamage(dvm);
				}, ViewModel.ReasonList, "Select Reason for No Photo", true);
			await Navigation.PushModalAsync(new CustomNavigationPage(selectReasonPage));
		}

		private async Task NavigateToPhotoViewer(string[] val) {

			await Navigation.PushAsync (new m.transport.ViewImage (val[0], val[1]));
		}

        public async Task<bool> NavigateToCamera(DamageViewModel dvm, bool isNewDamage = false)
        {
            var success = await (CameraManager.Instance).TakePhotoAsync((path1, path2) => {
                Photo p = new Photo() { ImagePath = path1, ThumbnailPath = path2 };
                photoAddList.Add(p);
                dvm.Photos.Add(p);
                dvm.SetPhotoVisbility(true);
                if (isNewDamage) {
                    ViewModel.AddDamage(dvm);
                }else {
                    Refresh();
                }
                ViewModel.Modified = true;
            }, async (err) => {
                await DisplayAlert("Error", err, "OK");
            });

            return success;
        }


		private async Task Refresh()
		{
            await Task.Run(() => { ViewModel.RefreshDamages(); });
        }
						
		private async Task PushDamageWorkflow()
		{
			var selectDamageLocation = new SelectDamageLocation(
				async delegate(string dmgCode)
				{
					await Navigation.PopModalAsync();
					CreateDamage(dmgCode);
				});
			await Navigation.PushModalAsync(new CustomNavigationPage(selectDamageLocation));
		}

		private async Task DeleteStoredPhoto(){
            await Task.Run(() =>
            {
                foreach(Photo p in photoDeleteList) 
                {
					DependencyService.Get<ILoadAndSaveFiles>().Delete(p.ImagePath);
					DependencyService.Get<ILoadAndSaveFiles>().Delete(p.ThumbnailPath);
                }
            });
		}

		private async Task DeleteStoredPhoto(Photo p)
		{
			await Task.Run(() =>
			{
				DependencyService.Get<ILoadAndSaveFiles>().Delete(p.ImagePath);
				DependencyService.Get<ILoadAndSaveFiles>().Delete(p.ThumbnailPath);
			});
		}

		private void BuildToolbar()
		{

			ToolbarItems.Clear();

			ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty, async () => 
			{
				await OnCancelClick();
			}));

			ToolbarItems.Add(new ToolbarItem(" ", string.Empty, async () => {}));

			ToolbarItems.Add(new ToolbarItem("Done", string.Empty, async delegate
			{
				await UnsubscribeMessage();
				
				//ViewModel.ConfirmShag();
                await DeleteStoredPhoto();
				await ViewModel.ConfirmDamages();
				await Navigation.PopModalAsync();
			}));
					
			ToolbarItems.Add(new ToolbarItem("Add", string.Empty, 
				async () => await PushDamageWorkflow()));
		}

		private async Task ReturnAction() {
			await CancelAction ();
			await Navigation.PopModalAsync();
		}

		public ICommand DeleteDamageCommand
		{
			get
			{
				return deleteDamageCommand;
			}
		}

		public async void DeleteDamage(DamageViewModel d)
		{

			bool delete = await DisplayAlert("Delete Damage", "Are you sure you want to delete this item?", "Delete", "Cancel");

			if (delete) {
                await Task.Run(() =>
                {
					ViewModel.DeleteDamage(d);
                });
			} else {
				EnableDamagePhotoAction (d);
			}
		}

		public InspectionViewModel ViewModel
		{
			get { return (InspectionViewModel)BindingContext; }
			set { BindingContext = value; }
		}

        protected override bool OnBackButtonPressed()
        {
            OnCancelClick();
            return true;
        }

		private async Task OnCancelClick() {
			if(ViewModel.Modified) {
				bool cancel = await DisplayAlert("Damage Modified", "Are you sure you want to cancel?", "Yes", "No");

				if (cancel) {
					await ReturnAction();
				}
			}else {
				await ReturnAction();
			}

		}

		private async Task CancelAction()
		{
            await Task.Run(async () =>
            {
                await UnsubscribeMessage();
                await DeleteStoredPhoto();
            });
		}

		public void EntryFocus(object sender, EventArgs ea)
		{
			CommentEditor.Unfocus();
			Navigation.PushModalAsync(new CustomNavigationPage
											  (new Comment("VIN Comments",
													 "Please Enter Comment",
													 CommentEditor.Text,
													 async delegate (string comment)
													 {
														 CommentEditor.Text = comment;
														 await Navigation.PopModalAsync();
													 })));

		}
	}
}


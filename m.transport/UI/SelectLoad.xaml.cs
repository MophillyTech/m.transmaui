using System;
using System.Linq;
using Autofac;
using m.transport.Data;
using m.transport.Interfaces;
using m.transport.Models;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using System.Collections.Generic;
using m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class SelectLoad : ContentPage
	{	
		private bool isCreateNewLoad = false;
		private DateTime lastSyncTime;
		private int runSize = 0;
		private CurrentLoad currentload;


		public SelectLoad (CurrentLoad currentload)
		{
			lastSyncTime = DateTime.Now;
			this.currentload = currentload;

			ToolbarItems.Add(new ToolbarItem("Cancel",string.Empty, async () => await Navigation.PopModalAsync()));
			ToolbarItems.Add(new ToolbarItem(string.Empty, "refresh.png", () => RefreshLoad()));
			ToolbarItems.Add(new ToolbarItem("Done",string.Empty, async delegate{

				if (!ViewModel.Loads.Any(l => l.Selected)) {
					await DisplayAlert("Error","Please select a Load","OK");
				} else {

					ProcessSelectedLoad();

				}
			}));
			ViewModel = new SelectLoadViewModel(App.Container.Resolve<ICurrentLoadRepository>());

			InitializeComponent();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			RefreshLoad ();
		}

		private async void RefreshLoad(){
			if (await this.BeginCallToServerAsync ("Retrieving Current Load Data...", "refreshload")) {
				isCreateNewLoad = false;
				GetCurrentLoad ();
			}
		}

		private void GetCurrentLoad()
		{
			ViewModel.GetCurrentLoadCompleted += ViewModelOnGetCurrentLoadCompleted;
			ViewModel.GetCurrentLoadAsync();
		}

		private async void CreateNewLoad(){

			ViewModel.CreateEmptyLoad ();
			currentload.UpdateToggleSelection(0);
			RefreshCurrentLoad ();
			await Navigation.PopModalAsync();
		}

		private void ViewModelOnGetCurrentLoadCompleted(object sender, GetCurrentLoadCompletedEventArgs args)
		{
			ViewModel.GetCurrentLoadCompleted -= ViewModelOnGetCurrentLoadCompleted;
            this.EndCallToServerAsync(args, "refreshload");

            if (args.Error != null)
            {
                Device.BeginInvokeOnMainThread( () => {
                    DisplayAlert("Error!",
                       "There was a problem fetching current load. Please contact Dispatch to resolve.", "OK");
                });

                return;
            }

			if (args != null && args.Result != null) {
				runSize = args.Result.Runs.Length;
				MessagingCenter.Send<SelectLoad>(this, MessageTypes.Exception);
				RefreshCurrentLoad ();
			}

			if (!isCreateNewLoad) {
				DependencyService.Get<ISound> ().PlaySound (
					args == null || args.Error != null ? SoundType.ErrorLoad : SoundType.SuccessLoad);
			} 

			Device.BeginInvokeOnMainThread(async() => {
				if(runSize > 1){
					await DisplayAlert("Attention!", 
						"You have multiple open Runs. Please contact Dispatch to resolve.", "OK");
				}
			});

			if(isCreateNewLoad){
				ProcessEmptyRun (true);
			}
				
			System.Diagnostics.Debug.WriteLine ("Syncing Setting check");
			ViewModel.UpdateConfiguration ();
		}

		private void ProcessEmptyRun(bool isEmptyRun)
		{
			Device.BeginInvokeOnMainThread(async() => {
				int vehicleCounter = ViewModel.LoadedVehicleCount();
				if (runSize > 0 && vehicleCounter > 0) {
					bool resp = await DisplayAlert ("Vehicle(s) On Truck", 
						"The system shows you currently have " +  vehicleCounter + " vehicle(s) on your truck. Have they already been delivered?", "Yes", "No");
					if (resp) {
						await DisplayAlert("Dispatch Update Needed!", 
							"Please call dispatch and ask them to mark your vehicles as 'Delivered'. Then do 'Refresh from Dispatch' to update the load in your mobile app.", "OK");
					}
					else{

						if(isEmptyRun)
							CreateNewLoad ();
						else
							BuildSelectedLoad();
					}
				} else {
					if(isEmptyRun)
						CreateNewLoad ();
					else
						BuildSelectedLoad();
				}
			});

		}

		private async void BuildSelectedLoad(){
			ViewModel.BuildSelectedLoads();
			this.currentload.UpdateToggleSelection(0);
            RefreshCurrentLoad();
			await Navigation.PopModalAsync();

		}

		private void RefreshCurrentLoad()
		{
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    MessagingCenter.Send<SelectLoad>(this, MessageTypes.RefreshCurrentLoad);
                    break;
            }
		}


		private void ProcessSelectedLoad(){

//			var vehicles = 
//				(from v in CurrentLoad.Vehicles
//					from ln in selectedLoadNumbers
//					where v.LoadNumber == ln
//					select v).ToArray();
//
			bool sameRun = false;

			foreach (Load l in ViewModel.Loads.Where(lo => lo.Selected == true).ToList()) {

				sameRun = false;

				List<DatsVehicleV5> vehList = ViewModel.Vehicles.Where (veh => veh.LoadNumber == l.LoadNumber).ToList ();

				foreach (DatsVehicleV5 v in ViewModel.Vehicles.Where(veh => veh.LoadNumber == l.LoadNumber).ToList()) {

					if(ViewModel.RunIds.Contains(v.RunId)){
						sameRun = true;
					}
				}

				if (!sameRun) {
					break;
				}
			}

			if (!sameRun)
				ProcessEmptyRun (sameRun);
			else
				BuildSelectedLoad ();
				
		}

		protected async void CheckForEmptyRun(object sender, EventArgs e) {

			if (DateTime.Compare(lastSyncTime.AddMinutes (3), DateTime.Now) < 0) {
				lastSyncTime = DateTime.Now;
				await this.BeginCallToServerAsync ("Creating Empty Run...", "EMPTY RUN");
				isCreateNewLoad = true;
				GetCurrentLoad ();
			} else {
				ProcessEmptyRun (true);
			}

		}
						
		public SelectLoadViewModel ViewModel
		{
			get { return (SelectLoadViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public void LoadSelected(object sender, EventArgs ea)
		{
			Load l = (Load)LoadList.SelectedItem;
			l.Selected = !l.Selected;
			LoadList.SelectedItem = null;
		}
	}

}


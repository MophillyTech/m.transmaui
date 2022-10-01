using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using System.ComponentModel;
using System.Threading;
using m.transport.Interfaces;
using System.Linq.Expressions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class CurrentLoad : ExtendedContentPage
	{
		private ILoginRepository loginRepository;
		private ICurrentLoadRepository currentLoadRepository;
		private int curToggleIndex = 0;
		private bool isUser;
		public bool ShowDetail { get; set; }
		private ToolbarItem sendItem;
		private ToolbarItem loadItem;
		private MainViewModel mainModel;
        private bool CheckForSplitLoad = false;
        private string actionId;
		private bool CheckForLocation = false;

		public CurrentLoad()
		{
						
			loginRepository = App.Container.Resolve<ILoginRepository>();
			currentLoadRepository = App.Container.Resolve<ICurrentLoadRepository>();
			ViewModel = new CurrentLoadViewModel(currentLoadRepository, loginRepository);
			mainModel = new MainViewModel (loginRepository, App.Container.Resolve<IAppSettingsRepository>(),currentLoadRepository);

            sendItem = new ToolbarItem(string.Empty, "sync.png", async () => await OnSend());
			loadItem = new ToolbarItem ("Load", string.Empty, async () => await Load (-1));

			ToolbarItems.Add (sendItem);
			ToolbarItems.Add (loadItem);

			InitializeComponent();
			ShowDetail = true;
			isUser = loginRepository.AccountType != "DEMO";

			switch (Device.RuntimePlatform)
			{
                case Device.Android:
					MessagingCenter.Subscribe<Login>(this, MessageTypes.RefreshCurrentLoad, (sender) => Refresh());
					MessagingCenter.Subscribe<SelectLoad>(this, MessageTypes.RefreshCurrentLoad, (sender) => RefreshVehicle());
					break;
			}

            MessagingCenter.Subscribe<CompleteDelivery>(this, MessageTypes.DeliveryCompleted, (sender) => { CheckForSplitLoad = true; });

			VehiclePickupList.ItemTemplate = new DataTemplate(typeof(VehiclePickupCell));
			VehiclePickupList.GroupHeaderTemplate = new DataTemplate(typeof(VehiclePickupCellHeader));

			VehicleDeliveryList.ItemTemplate = new DataTemplate(typeof(VehicleDeliveryCell));
			VehicleDeliveryList.GroupHeaderTemplate = new DataTemplate(typeof(VehicleDeliveryCellHeader));
			VehicleDeliveryList.IsVisible = false;
		}

        private void CheckSplitLoad() {

            // 13317 - if split load, display warning to driver

            // per Julie, we're disabling this until we can determine what data conditions should trigger the message

            /*
            var loads = (from v in currentLoadRepository.CurrentLoad.Vehicles select v.LoadId).Distinct().ToList();

            bool empty = true;

            // if there are any vehicles in delivery list, we will count that as a split load and display warning
            foreach (var v in VehicleDeliveryList.ItemsSource) {
                empty = false;
            }

            if (!empty)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Split Load", "Are your decks down?", "OK");
                });
            }
            */

        }

		private async Task PrinterCloseSession(){
			await Task.Run(() =>
			{
				Task.Delay(30000).Wait();
                Device.BeginInvokeOnMainThread(() =>
               {
                   DependencyService.Get<IPrinter>().Close();
               });
			});

		}

        async protected override void OnAppearing()
		{
			base.OnAppearing();
			Refresh ();

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
                    await PrinterCloseSession();
					break;
			}
           		
            if (CheckForSplitLoad) {
                CheckForSplitLoad = false;
                CheckSplitLoad();
            }
        }

		private void ToolItemButtonVsibility(){

			if (curToggleIndex == 0 && ToolbarItems.Count == 1) {
				ToolbarItems.Add (loadItem);
			} else if (curToggleIndex == 1 && ToolbarItems.Count == 2) {
				ToolbarItems.RemoveAt (1);
			}
		}
			
		private async void Refresh()
		{
			ToolItemButtonVsibility ();

			if (loginRepository.IsLoggedIn)
			{
				//set the accountype when logged in
				if (loginRepository.FirstTimeLogin)
				{
					isUser = loginRepository.AccountType != "DEMO" ? true : false;
					System.Diagnostics.Debug.WriteLine("Syncing Setting on Login");
					mainModel.RefreshAppSettingsComplete += ViewModelOnRefreshAppSettingsComplete;
					mainModel.UpdateConfiguration(false);
				}

				if (loginRepository.FirstTimeLogin && !ViewModel.HasSelectedLoadVehicles)
				{
					loginRepository.FirstTimeLogin = false;

					currentLoadRepository.DisplayOdometerPrompt = true;
					await PromptToSelectLoad();
				}
				else if (loginRepository.FirstTimeLogin)
				{
					//this makes sure that on re-login and finishing up a delivery, you won't get prompt with the refresh dispatch prompt
					loginRepository.FirstTimeLogin = false;
				}
                RefreshVehicle();
			}
		}

		private void RefreshVehicle()
		{
            Device.BeginInvokeOnMainThread(async() => {
				VehiclePickupList.ItemsSource = await ViewModel.InitPickupList();
				VehicleDeliveryList.ItemsSource = await ViewModel.InitDropOffList();
			});

			ChangeVehicleListVisbility(true);
		}

		private void ViewModelOnRefreshAppSettingsComplete(object sender, AsyncCompletedEventArgs args)
		{
			mainModel.RefreshAppSettingsComplete -= ViewModelOnRefreshAppSettingsComplete;
			MessagingCenter.Send<CurrentLoad>(this, MessageTypes.RefreshSetting);
		}


		private async Task PromptToSelectLoad()
		{
			Device.BeginInvokeOnMainThread(async () => {
				bool resp = await DisplayAlert("No Load Selected", "Use 'Refresh from Dispatch' option on the menu to select a load", "Refresh Now", "Skip");
				if (resp)
				{
					await Navigation.PushModalAsync(new CustomNavigationPage(new SelectLoad(this)));
				}
			});
		}

        internal void ChangeVehicleListVisbility(bool visbility)
        {
			Device.BeginInvokeOnMainThread(() =>
			{
				RunStack.IsVisible = visbility;
			});
        }

        public CurrentLoadViewModel ViewModel
		{
			get { return (CurrentLoadViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		public async void VehiclePickupSelected(object sender, EventArgs ea)
		{
			ShowDetail = true;

			Device.BeginInvokeOnMainThread(async () => {
				await Navigation.PushAsync(new VehicleDetail((VehicleViewModel) VehiclePickupList.SelectedItem, InspectionType.Loading));
				VehiclePickupList.SelectedItem = null;
			});
		}

		public async void VehicleDeliverySelected(object sender, EventArgs ea)
		{
			ShowDetail = true;

			Device.BeginInvokeOnMainThread(async () => {
				await Navigation.PushAsync(new VehicleDetail((VehicleViewModel)VehicleDeliveryList.SelectedItem, InspectionType.Delivery));
				VehicleDeliveryList.SelectedItem = null;
			});
		}

			
		public void UpdateToggleSelection(int index)
		{
			
			if (curToggleIndex == index)
				return;

            Device.BeginInvokeOnMainThread(() =>
           {
               Toggle.SelectedSegment = index;
               curToggleIndex = index;
               ChangeVehicleListHeader();
           });
		}

		public void ToggleMode(object sender, int? index)
		{

			if (curToggleIndex == index || index == null)
				return;

			curToggleIndex = index.Value;

			ChangeVehicleListHeader ();
		}

		private void ChangeVehicleListHeader(){
			
			ToolItemButtonVsibility ();
			switch (curToggleIndex)
			{
			case 0:
				VehiclePickupList.IsVisible = true;
				VehicleDeliveryList.IsVisible = false;
				break;
			case 1:
				VehiclePickupList.IsVisible = false;
				VehicleDeliveryList.IsVisible = true;
				break;
			}
		}
        
		private List<ToolbarItem> savedToolbarItems = new List<ToolbarItem>();


		public async Task OnSend() {

			switch (curToggleIndex)
			{
			case 0:
				OnSend (SubmitAction.Loading);
				break;
			case 1:
				OnSend (SubmitAction.Delivering);
				break;
			}
		}


		public async Task OnSend(SubmitAction action, bool byPassException = false)
		{
			if (byPassException) {
                //do nothing
            }
            else if (!ViewModel.IsDirty (curToggleIndex) && (action != SubmitAction.DeliveringException || action != SubmitAction.LoadingException)) 
			{
				switch (curToggleIndex)
				{
				case 0:
                    if (action == SubmitAction.LoadingException) {
                        UploadDamagePhotos();
                    } else {
                        await DisplayAlert("Warning!", "You have no loading vehicle.", "OK");
                    }
					break;
				case 1:
					await DisplayAlert("Warning!", 
						"You have no delivering vehicle.", "OK");
					break;
				default:
					await DisplayAlert("Warning!", 
						"You have no loading or delivering vehicle.", "OK");
					break;
				}

				return;
			}

			CheckForLocation = true;

			int ndx = 0;
			if (ViewModel.HasSelectedLoadVehicles)
			{
				foreach (VehicleViewModel v in ViewModel.Vehicles)
				{
                    //placeholder for future use
                    v.DatsVehicle.LogData = string.Empty;

                    if (string.IsNullOrEmpty(v.DatsVehicle.ExpectedPhotoCount)) {
                        v.DatsVehicle.ExpectedPhotoCount = string.Empty;
                    }
                        
					v.DatsVehicle.SaveTempDamages(InspectionType.Loading);

					// if two vehicles have the same VehicleID, even 0, the exception processing on the server will
					// NOT correctly handle them.  To workaround this, we will assign any new Vehicles a sequential int ID
					// When they are inserted into Vehicles on the server they should get a permanent VehicleID
					if (v.DatsVehicle.VehicleId == 0)
					{
						v.DatsVehicle.VehicleId = ndx;
						ndx--;
					}
				}

				if (isUser)
				{

					Device.BeginInvokeOnMainThread (async () => {
						if (action == SubmitAction.DeliveringException || action == SubmitAction.LoadingException) {
                            actionId = "deliveryException";
                            if (await this.BeginCallToServerAsync ("Sending Exception To Server...", actionId)) {
								System.Diagnostics.Debug.WriteLine ("Sending Exception To Server...");
								ViewModel.UploadCurrentLoadCompleted += ViewModelOnUpdateExceptionsCompleted;
							}else{
								return;
							}
						} else {
                            actionId = "regularDelivery";
                            if (await this.BeginCallToServerAsync ("Sending Info To Server...", actionId)) {
								System.Diagnostics.Debug.WriteLine ("Sending Info To Server...");
								ViewModel.UploadCurrentLoadCompleted += ViewModelOnUploadCurrentLoadCompleted;
							}else{
								return;
							}
						}

						if(action == SubmitAction.LoadingException) {
							UpdateToggleSelection(0);
						}


						switch (curToggleIndex)
						{
						case 0:
							ViewModel.SendCurrentLoadAsync (UploadStatus.Pickup);
							break;
						case 1:
							ViewModel.SendCurrentLoadAsync (UploadStatus.OfflineDelivery);
							break;
						}
					});
				}
				else
				{
					foreach (VehicleViewModel v in ViewModel.Vehicles)
					{
						if (v.DatsVehicle.VehicleStatus == "Loading")
							v.DatsVehicle.SetVehicleStatus(VehicleStatus.Loaded);
					}

					RefreshLoad ();
				}
			}
			else
			{
				await PromptToSelectLoad();
			}
		}
			
		private async void RefreshCurrentLoad(bool refresh = false){
            // TODO: This should check if there is network before
            actionId = "refreshingId";
			Device.BeginInvokeOnMainThread (async () => {
                if (await this.BeginCallToServerAsync ("Refeshing Current Load...", actionId)) {
					ViewModel.GetCurrentLoadCompleted += ViewModelOnGetCurrentLoadCompleted;
					if (curToggleIndex == 1) 
					{
						ViewModel.GetCurrentLoadAysnc (refresh, true);
					} 
					else 
					{
						ViewModel.GetCurrentLoadAysnc (refresh, false);
					}
				}
			});
		}

		private void ViewModelOnGetCurrentLoadCompleted(object sender, GetCurrentLoadCompletedEventArgs args){
			System.Diagnostics.Debug.WriteLine ("Refreshing load completed");
			ViewModel.GetCurrentLoadCompleted -= ViewModelOnGetCurrentLoadCompleted;
		
            this.EndCallToServerAsync (args, actionId);

			Device.BeginInvokeOnMainThread (async () => {
				if(currentLoadRepository.GetRunCount() > 1){
					await DisplayAlert("Attention!", 
						"You have multiple open Runs. Please contact Dispatch to resolve.", "OK");
				}

			});
				
			if (args.Error == null) {
                List<VehicleViewModel> tempVehicles = ViewModel.Vehicles;
				for (int i = ViewModel.Vehicles.Count - 1; i >= 0; --i) {
                    VehicleViewModel vm = tempVehicles[i];
					var foundVIN = args.Result.Vehicles.FirstOrDefault (x => x.VIN == vm.VIN);
					if (foundVIN != null) {
						if (foundVIN.ExceptionCode > 0) {
                            foundVIN.PickupLocationId = vm.DatsVehicle.PickupLocationId;
							foundVIN.OriginalDropoffLocation = vm.DatsVehicle.OriginalDropoffLocation;
							foundVIN.SignedBy = vm.DatsVehicle.SignedBy;
							foundVIN.AttendedInd = vm.DatsVehicle.AttendedInd;
							foundVIN.SubjectToInspectionInd = vm.DatsVehicle.SubjectToInspectionInd;
							foundVIN.RefusedSignCode = vm.DatsVehicle.RefusedSignCode;
							foundVIN.YardLocation = vm.DatsVehicle.YardLocation;
							foundVIN.DeliveryNotes = vm.DatsVehicle.DeliveryNotes;
							foundVIN.PickupInspectionNotes = vm.DatsVehicle.PickupInspectionNotes;
							foundVIN.DropOffInspectionNotes = vm.DatsVehicle.DropOffInspectionNotes;
                            foundVIN.ExpectedPhotoCount = vm.DatsVehicle.ExpectedPhotoCount;
                            //foundVIN.LoadInspectionDamageCodes = vm.DatsVehicle.LoadInspectionDamageCodes;
                            foundVIN.TempInspectionDamageCodes = vm.DatsVehicle.TempInspectionDamageCodes;
						}
						System.Diagnostics.Debug.WriteLine ("Refreshing: VIN found");
						vm.DatsVehicle = foundVIN;
					} 
                    else {
                        if ((vm.DatsVehicle.ExceptionFlag == 2 && vm.DatsVehicle.ExceptionCode == 12) ||
							(vm.DatsVehicle.ExceptionFlag == 1 && vm.DatsVehicle.ExceptionCode == 1)  ||
							(vm.DatsVehicle.ExceptionFlag == 1 && vm.DatsVehicle.ExceptionCode == 5)  ||
							(vm.DatsVehicle.ExceptionFlag == 1 && vm.DatsVehicle.ExceptionCode == 7))
                            ViewModel.Vehicles.Remove(vm);
                        else {
                            vm.DatsVehicle.ExceptionCode = 0;
                            vm.DatsVehicle.ExceptionFlag = 0;
                            vm.DatsVehicle.ExceptionId = 0;
                            vm.DatsVehicle.ExceptionInd = null;
                            vm.DatsVehicle.ExceptionMessage = null;
                            vm.DatsVehicle.ProcessInd = null;
                            vm.DatsVehicle.LegId = 0;
                            List<DatsVehicleV5> dv;
                            if (currentLoadRepository.SelectedLoad.Vehicles != null) {
                                dv = currentLoadRepository.SelectedLoad.Vehicles.ToList();
                            } else {
                                dv = new List<DatsVehicleV5>();
                            }

                            dv.Add(vm.DatsVehicle);
                            currentLoadRepository.SelectedLoad.Vehicles = dv.ToArray();

                        }
                         
					}

					if (foundVIN != null && foundVIN.LoadInspectionDamageCodes != vm.DatsVehicle.LoadInspectionDamageCodes) {
						Device.BeginInvokeOnMainThread (async () => {
							await DisplayAlert("Attention!", 
								"Please contact dispatch about the reported damage", "OK");
						});
						return;
					}
				}

				for (int i = ViewModel.Vehicles.Count - 1; i >= 0; --i)
				{
					VehicleViewModel vm = tempVehicles[i];
					var foundVINByVehicleID = args.Result.Vehicles.FirstOrDefault(x => x.VehicleId == vm.DatsVehicle.VehicleId);
					if (foundVINByVehicleID.VehicleStatus == "Loaded"
						&& (vm.VIN.StartsWith("PARTIAL") || vm.VIN.EndsWith("DUMMY"))
						&& (vm.DatsVehicle.VIN != foundVINByVehicleID.VIN))
					{
						ViewModel.Vehicles.Remove(vm);
					}

				}

				var exceptionalVehicles = (from x in ViewModel.Vehicles
					where (IsException (x, curToggleIndex))
					select x).ToList ();

				if (exceptionalVehicles.Count > 0) {
					System.Diagnostics.Debug.WriteLine ("Currentload: exception found");
					ViewModel.HasExceptions = true;
					MessagingCenter.Send(this, MessageTypes.Exception, ViewModel.HasExceptions);

					Device.BeginInvokeOnMainThread (async () => {
                        var manageExceptionsPage = new ManageExceptions (exceptionalVehicles, async (bypassEceptionCheck) =>
                        {
                            await Navigation.PopModalAsync();
                            switch (curToggleIndex)
                            {
                                case 0:
                                    System.Diagnostics.Debug.WriteLine("Submitting loading exception");
                                    OnSend(SubmitAction.LoadingException, bypassEceptionCheck);
                                    break;
                                case 1:
                                    System.Diagnostics.Debug.WriteLine("Submitting delivering exception");
                                    OnSend(SubmitAction.DeliveringException, bypassEceptionCheck);
                                    break;
                            }
                        },
                            async delegate {
                                System.Diagnostics.Debug.WriteLine("Poping Exception canceling");
                                await Navigation.PopModalAsync();
                            });
                        await Navigation.PushModalAsync(new CustomNavigationPage(manageExceptionsPage));
					});

					return;
				}
				ViewModel.HasExceptions = false;
				MessagingCenter.Send(this, MessageTypes.Exception, ViewModel.HasExceptions);
					
				if (curToggleIndex == 1) 
				{
					UploadDriverSignature ();
				} 
				else 
				{
					var assignedVehicles = (from x in ViewModel.Vehicles
											   where (x.DatsVehicle.VehicleStatus == "Assigned")
											   select x).ToList();
					if (assignedVehicles.Count > 0)
					{
						return;
					}
						System.Diagnostics.Debug.WriteLine ("Running damage upload");
					UploadDamagePhotos ();
				}
			}
		}

		private async void UploadDriverSignature(){
            actionId = "uploadSignature";
            if (await this.BeginCallToServerAsync ("Uploading Driver Signature...", actionId)) {
				ViewModel.SubmitDriverSignatureCompleted += OnUploadDriverSignatureCompleted;
				ViewModel.UploadDriverSignature ();
			}
		}

		private void OnUploadDriverSignatureCompleted(object sender, AsyncCompletedEventArgs args)
		{
			ViewModel.SubmitDriverSignatureCompleted -= OnUploadDriverSignatureCompleted;

			Device.BeginInvokeOnMainThread (async () => {
                this.EndCallToServerAsync (args, actionId);
			});
				
			if (sender == null && args == null) {
                RefreshVehicle();
			} else if (args.Error == null) {
				UploadDamagePhotos ();
			}
		}

		private async void UploadDamagePhotos(){

			if(ViewModel.HasDamagePhotos()){
				ViewModel.UploadDamagePhotosCompleted += ViewModelOnUploadPhotosCompleted;
                actionId = "UploadPhoto";
				Device.BeginInvokeOnMainThread (async () => {
                    if (await this.BeginCallToServerAsync ("Uploading Damage Photos...", actionId)) {
						ViewModel.UploadDamagePhotos ();
					}
				});
			}else if(curToggleIndex == 1){
				if (await ViewModel.HasOfflineDelivery()) {
					OnSend ();
				} else {
					RefreshLoad ();
				}
			}else{
				System.Diagnostics.Debug.WriteLine ("No damage photo refresing load");
				RefreshLoad ();
			}

			if (CheckForLocation)
			{
				System.Diagnostics.Debug.WriteLine("Reporting: loading location");
				CheckForLocation = false;
				Device.BeginInvokeOnMainThread(async () =>
				{
					LocationManager.Instance.SetCheckLocationTimer();
					await LocationManager.Instance.ReportLocation();
				});
			}
		}

		private void ViewModelOnUploadPhotosCompleted(object sender, AsyncCompletedEventArgs args){
			ViewModel.UploadDamagePhotosCompleted -= ViewModelOnUploadPhotosCompleted;

            this.EndCallToServerAsync (args, actionId);

			Device.BeginInvokeOnMainThread (async () => {
				if (args != null && args.Error == null) {
					if (await ViewModel.HasOfflineDelivery()) {
						OnSend ();
					} else {
						RefreshLoad ();
					}
				}
			});
		}
			
		private void RefreshLoad(){
			ToolItemButtonVsibility();
			int index = curToggleIndex == 1 ? 0 : 1;
			UpdateToggleSelection(index);
            RefreshVehicle();
			
		}
						
		private void ViewModelOnUpdateExceptionsCompleted(object sender, AsyncCompletedEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine ("Exception Upload Completed");
			ViewModel.UploadCurrentLoadCompleted -= ViewModelOnUpdateExceptionsCompleted;

            this.EndCallToServerAsync (args, actionId);

			if (args.Error == null) {
				MessagingCenter.Send<CurrentLoad>(this, "exception");
				System.Diagnostics.Debug.WriteLine ("Refreshing load after exception");
				RefreshCurrentLoad(true);
			}
		}

		private void ViewModelOnUploadCurrentLoadCompleted(object sender, AsyncCompletedEventArgs args)
		{
			ViewModel.UploadCurrentLoadCompleted -= ViewModelOnUploadCurrentLoadCompleted;

            this.EndCallToServerAsync (args, actionId);

			if(args.Error == null){
				RefreshCurrentLoad ();
			}
		}
			
		private static bool IsException(VehicleViewModel x, int index)
		{
			switch (index)
			{
				case 0:
				return x.DatsVehicle.ExceptionCode > 0 &&
					(x.DatsVehicle.ExceptionCode != 8 && x.DatsVehicle.ExceptionCode != 9);
				case 1:
				return (x.DatsVehicle.ExceptionCode == 8 || x.DatsVehicle.ExceptionCode == 9);
				default:
					return false;
			}
		}

		async void OnLoad(object sender, EventArgs e)
		{
			await Load(-1);
		}

		private async Task Load(int locID)
		{

            //if no location is selected they may be adding vehicles to an empty run, which is allowed

            if (locID == 0 && ViewModel.VehiclesByPickup.Count == 0)
            {
                return;
            }

            if (!ViewModel.VehiclesInTransitionState && currentLoadRepository.DisplayOdometerPrompt)
            {
                int? limit = App.Container.Resolve<IAppSettingsRepository>().Settings["MobileOdometerMileageLimit"].ParseInt();

                var popupAlert = new PopupAlertText("Odometer Reading", "Please Enter Starting Odometer",
                    async delegate (int? reading)
                    {
                        ViewModel.UpdatedOdometerReading = reading;
                        currentLoadRepository.DisplayOdometerPrompt = false;
                        await Navigation.PopModalAsync();
                        Device.BeginInvokeOnMainThread(async () => await Load(locID));
                    }, ViewModel.Mileage, limit, isUser);
                await Navigation.PushModalAsync(new CustomNavigationPage(popupAlert));
            }
            else
            {
                await Navigation.PushAsync(new ManageLoad(locID, this));
            }
        }

		async public void LoadForLocation(int locID)
		{
			if (locID == -1)
			{
				await DisplayAlert("Error", "Vehicles cannot be loaded for an unknown destination", "OK");
			}
			else
			{
				await Load(locID);
			}
		}

		public void Clear(){
			UpdateToggleSelection(0);
			VehiclePickupList.ItemsSource = null;
			VehicleDeliveryList.ItemsSource = null;
		}

		public async void DeliverForLocation(int locID)
		{
			string[] statuses = { "Loaded"};
			//List<DatsVehicle> vehicles = ViewModel.Vehicles.Where (v => v.DropoffLocationName == locationName).ToList();
			List<VehicleViewModel> vehicles = ViewModel.Vehicles.Where(v => statuses.Contains(v.DatsVehicle.VehicleStatus)).ToList();

			if (vehicles.Count == 0)
				await DisplayAlert ("Error", "There are no vehicles in the \"Loaded\" state to Unload.", "OK");
			else {
				ViewModel.SetDropoffLocation(locID);
				await Navigation.PushAsync(new ManageDelivery(locID));
			}

		}
			
		protected override bool OnBackButtonPressed ()
		{
			MessagingCenter.Send<CurrentLoad>(this, MessageTypes.RemoveMessageSubscription);
			return false;
		}

		public void RemoveMessageSubscription()
		{
			switch (Device.RuntimePlatform)
			{
				case Device.Android:
					MessagingCenter.Unsubscribe<Login>(this, MessageTypes.RefreshCurrentLoad);
					MessagingCenter.Unsubscribe<SelectLoad>(this, MessageTypes.RefreshCurrentLoad);
					break;
			}

            MessagingCenter.Unsubscribe<CompleteDelivery>(this, MessageTypes.DeliveryCompleted);
		}
	}
}


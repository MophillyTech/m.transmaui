using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Autofac;
using m.transport.Data;
using m.transport.Interfaces;
using m.transport.Models;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using System.Threading.Tasks;
using m.transport.Domain;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace m.transport
{
	public partial class MainPage : FlyoutPage
    {
		private Dictionary<CustomMenuItem, Func<object>> menuItems;
		private IServiceClientFactory<ITransportServiceClient> client;
		private ILoginRepository loginRepository;
		private ICurrentLoadRepository loadRepository;
		private CurrentLoad currentLoad;
		//keep track of the expectionItem for enable/disable
		private CustomMenuItem exceptionItem, createExpenseItem, viewExpenseItem, payHistoryItem, settingsItem;
		private List<CustomMenuItem> menuList;
		public bool LoginPage { get; set; }

		public MainPage()
		{
			loginRepository = App.Container.Resolve<ILoginRepository>();
			var settingsRepository = App.Container.Resolve<IAppSettingsRepository>();
			client = App.Container.Resolve<IServiceClientFactory<ITransportServiceClient>>();

			//client.Url = new Uri(new Uri(settingsRepository.Settings ["WebServiceBase"]), new Uri(settingsRepository.Settings ["WebServicePath"])).ToString();
			loadRepository = App.Container.Resolve<ICurrentLoadRepository>();
			ViewModel = new MainViewModel(loginRepository, settingsRepository, loadRepository);
            MasterBehavior = MasterBehavior.Popover;
			InitializeComponent();
			currentLoad = new CurrentLoad();
			exceptionItem = new CustomMenuItem("Manage Exceptions");
			createExpenseItem = new CustomMenuItem("New Expense");
			viewExpenseItem = new CustomMenuItem("Unpaid Expenses");
			payHistoryItem = new CustomMenuItem("Pay History");
			settingsItem = new CustomMenuItem("Setting");

			exceptionItem.Enabled = false;
			menuItems = new Dictionary<CustomMenuItem, Func<object>>()
			{
				//{"Load Management", null },
				{new CustomMenuItem("Refresh from Dispatch"), () => {
						TryRefresh();
						return null;
					}
				},
				{new CustomMenuItem("Shag Summary"), () => {
						PrintShag();
						return null;
					} 
				},
				{new CustomMenuItem("Reprint Recent Receipt"), () =>  new DeliveryHistory() },
				{new CustomMenuItem("Load Summary Report"), () => 
					new SelectLocation (loadRepository, loginRepository, currentLoad.ViewModel.VehiclesByPickup, 
						currentLoad.ViewModel.Locations.ToList(), "Load Summary")
				},
				{new CustomMenuItem("Gate Pass Report"), () => 
					new SelectLocation (loadRepository,loginRepository, currentLoad.ViewModel.VehiclesByPickup, 
						currentLoad.ViewModel.Locations.ToList(), "Gate Pass")
				},

				{new CustomMenuItem("Refresh App Settings"), () => {
						System.Diagnostics.Debug.WriteLine ("Syncing Setting now");
						RefreshAppSettings();
						return null;
					}
				},
				{createExpenseItem, () => new CreateExpense()},
				{viewExpenseItem, () => new UnpaidExpense()},
				{payHistoryItem, () => new PayHistory() },
					
				{exceptionItem, LoadException },
				{new CustomMenuItem("Test Printer"), () => {
						PrintSample();
						return null;
					} 
				},

				{new CustomMenuItem("Test Scanner"), () => new TestScanner()},

				//{new CustomMenuItem("View Log"), () => new ViewLog() },
				
				// temporarilty remove Settings until we have something to add to the page
				//{new CustomMenuItem("Settings"), () => new SettingsModal() },
				{new CustomMenuItem("About"), () => new About() },
				{settingsItem, () => new AppSettings() },
				{new CustomMenuItem("Logout"), () => {
						TryLogout();
						return null;
					}
				}
			};

			Menu.ItemsSource = menuItems.Keys;
			Detail = new CustomNavigationPage(currentLoad);

			MessagingCenter.Subscribe<CurrentLoad, bool>(this, MessageTypes.Exception, (sender, arg) => ExceptionRefresh(arg));
			MessagingCenter.Subscribe<SelectLoad>(this, MessageTypes.Exception, (sender) => ExceptionRefresh(false));
			MessagingCenter.Subscribe<CurrentLoad> (this, MessageTypes.RemoveMessageSubscription, (sender) => RemoveMessageSubscription());
			MessagingCenter.Subscribe<CurrentLoad>(this, MessageTypes.RefreshSetting, (sender) => refreshMenuList());
			MessagingCenter.Subscribe<Login>(this, MessageTypes.RefreshSetting, (sender) => refreshMenuList());
			MessagingCenter.Subscribe<AppSettings>(this, MessageTypes.LogOut, (sender) => TryLogout());
			MessagingCenter.Subscribe<Page>(this, MessageTypes.LogOut, (sender) => TryLogout());

		}
			
		async private void TryLogout() {
			int cont = await CanLogout();
			if (cont == 0) {
				
				exceptionItem.Enabled = false;
                currentLoad.ChangeVehicleListVisbility(false);
				ViewModel.Logout(); 
				LoginPage = true;
				await Navigation.PushModalAsync (new CustomNavigationPage (new Login (this)));
			} else {
				if (cont == 1) {
					// dirty
					bool c = await DisplayAlert ("Send to Dispatch", "Do you want to Send your current data to Dispatch?", "Send", "Cancel");
					if (c) {
						IsPresented = false;
						//TODO if you have both offline delivering and loading vehicle how to handle this case
						currentLoad.OnSend (SubmitAction.Loading);
					}
				} else {
					// exception
					bool c = await DisplayAlert ("Manage Exceptions", "Do you want to view outstanding exceptions?", "Exceptions", "Cancel");
					if (c) {
						LoadException ();
					}
				}
			}

		}

		private async void TestScanner()
		{
			Navigation.PushModalAsync (new m.transport.TestScanner ());
		}

		async private void TryRefresh() {
			int cont = await CanLogout();
			if (cont == 0) {
				await Navigation.PushModalAsync (new CustomNavigationPage (new SelectLoad (currentLoad)));
			} else {
				if (cont == 1) {
					// dirty
					bool c = await DisplayAlert ("Send to Dispatch", "Do you want to Send your current data to Dispatch?", "Send", "Cancel");
					if (c) {
						IsPresented = false;
						currentLoad.OnSend (SubmitAction.Loading);
					}
				} else {
					// exception
					bool c = await DisplayAlert ("Manage Exceptions", "Do you want to view outstanding exceptions?", "Exceptions", "Cancel");
					if (c) {
						LoadException ();
					}
				}
			}
		}

		async private Task<int> CanLogout() {
			bool ok = false;
			int ret = 0;

			if (currentLoad.ViewModel.HasExceptions) {
				ok = await DisplayExceptionDialog ();
				if (!ok)
					ret = 2;
			} else if (currentLoad.ViewModel.IsDirty ()) {
				ok = await DisplayDirtyDialog ();
				if (!ok)
					ret = 1;
			}

			return ret;
		}

		async private Task<bool> DisplayExceptionDialog() {

			bool cont = await DisplayAlert ("Error", "You have Exceptions pending that you have not resolved. If you continue you will lose this information.", "Continue", "Cancel");

			return cont;

		}

		async private Task<bool> DisplayDirtyDialog() {
			bool cont = await DisplayAlert ("Error", "You have loading information that was not sent to dispatch. If you continue you will lose this information.", "Continue", "Cancel");

			return cont;
		}

		private object LoadException()
		{

			Device.BeginInvokeOnMainThread(async () =>
			{
				var manageExceptionsPage = new ManageExceptions(currentLoad.ViewModel.Exceptions, async delegate
					{
						currentLoad.OnSend(SubmitAction.LoadingException);
						await Navigation.PopModalAsync();
					}, async delegate
					{
						await Navigation.PopModalAsync();
					});
				await Navigation.PushModalAsync(new CustomNavigationPage(manageExceptionsPage));
			});

			return null;
		}

		private async void PrintShag()
		{
            try
            {
                bool resp = await DisplayAlert("Print report", "Would you like to print a shag summary?", "Yes", "No");

                if (resp)
                {

                    string[] statuses = { "Loaded", "Assigned", "Loading" };

                    List<VehicleViewModel> vehicles = new List<VehicleViewModel>();

                    foreach (GroupedVehicles g in currentLoad.ViewModel.VehiclesByPickup)
                    {

                        vehicles.AddRange(
                            g.Vehicles.Where(v => statuses.Contains(v.DatsVehicle.VehicleStatus)).ToList()
                        );
                    }


                    if (vehicles.Count == 0)
                    {
                        await DisplayAlert("Error", "There are no vehicles to load.", "OK");
                    }
                    else
                    {
                        bool success = PaperWork.PrintShag(loginRepository, vehicles, currentLoad.ViewModel.Locations.ToList());

                        if (!success)
                            await DisplayAlert("Bluetooth is Turned Off", "To print a sample, bluetooth must be turned on in the Settings app.", "OK");
                    }
                }
            } catch (Exception ex) {
                //InsightsManager.Track(ex.StackTrace);
                //await DisplayAlert("Error", ex.Message, "OK");
                await DisplayAlert("Error", "An error occurred during printing", "OK");
            }
		}
		
		private async void PrintSample()
		{
			bool resp = await DisplayAlert("Test Printer", "Would you like to print a sample?", "Yes", "No");

				if (resp)
				{
				Boolean success = PaperWork.PrintSample(loginRepository, client, DependencyService.Get<IBuildInfo>());
	
					if(!success)
					await DisplayAlert ("Bluetooth is Turned Off", "To print a sample, bluetooth must be turned on in the Settings app.", "OK");

				}
		}

		private void GetCodeTable(){
			ViewModel.RefreshCodeTable (false);
		}
			
		private async void RefreshAppSettings()
		{
			if (await this.BeginCallToServerAsync ("Refreshing app settings...")) {
				ViewModel.RefreshAppSettingsComplete += ViewModelOnRefreshAppSettingsComplete;
				ViewModel.UpdateConfiguration (false);
			}
		}

		private void ViewModelOnRefreshAppSettingsComplete(object sender, AsyncCompletedEventArgs args)
		{
			ViewModel.RefreshAppSettingsComplete -= ViewModelOnRefreshAppSettingsComplete;

			this.EndCallToServerAsync(args);

			Device.BeginInvokeOnMainThread( () => {
				DependencyService.Get<ISound>().PlaySound(
					args.Error == null ? SoundType.SuccessLoad : SoundType.ErrorLoad);

				DisplayAlert("Success!", "Your App Settings have been refreshed!", "OK");
				refreshMenuList();
			});
		}

		private void refreshMenuList() {

			Device.BeginInvokeOnMainThread (async () => {
				Menu.ItemsSource = null;
				menuList = menuItems.Keys.ToList();
				if(!ExpenseEnableCheck ()){
					menuList.Remove(createExpenseItem);
					menuList.Remove(viewExpenseItem);
				}

				if(!PayHistoryEnableCheck()){
					menuList.Remove(payHistoryItem);
				}

				if (!SettingEnableCheck())
                {
					menuList.Remove(settingsItem);
				}

				Menu.ItemsSource = menuList;
			});

		}

		public MainViewModel ViewModel
		{
			get { return (MainViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		protected override async void OnAppearing()
		{
			if (!DependencyService.Get<INetworkAvailability>().IsNetworkAvailable() && loginRepository.LoginResult != null)
			{
                loginRepository.LoginResult.CompanyInfo = new CompanyInfo(loginRepository.LoginResult.CompanyInfoToString);
				loginRepository.SetIsLoggedIn(true);
				return;
			} else if (!loginRepository.IsLoggedIn)
			{
				LoginPage = true;
				loadRepository.RemoveAllDamagePhotos();
				await Navigation.PushModalAsync(new CustomNavigationPage(new Login(this)));
			}
		}
			
		private bool PayHistoryEnableCheck()
		{
			if (ViewModel.GetMobilePayHistoryInd() == 1 && 
					(ViewModel.GetDriverType () == "DM" || 
						(ViewModel.GetDriverType () == "DP" && ViewModel.GetOutsideCarrierCompany () == 1))) {
				return true;
			} else {
				return false;
			}
		}

		private bool SettingEnableCheck()
		{
			return loginRepository.LoginResult.GPSTrackingInd == 1;
		}

		private bool ExpenseEnableCheck()
		{
			if (ViewModel.GetMobileExpenseInd() == 1 && 
			    (ViewModel.GetDriverType () == "DM" || 
				(ViewModel.GetDriverType () == "DP" && ViewModel.GetOutsideCarrierCompany () == 1))) {
				return true;
			} else {
				return false;
			}
		}

		private async void ExceptionRefresh(bool hasException)
		{
			exceptionItem.Enabled = hasException;
			refreshMenuList ();
		}
			
		public async void MenuClicked(object sender, EventArgs ea)
		{
			var selectedItem = (CustomMenuItem)Menu.SelectedItem;

			if (!selectedItem.Enabled)
			{
				Menu.SelectedItem = null;
				return;
			}


			var func = menuItems[selectedItem];

			if (func != null)
			{
				IsPresented = false;

				var page = func() as Page;
				if (page != null)
				{
					await Navigation.PushModalAsync(new CustomNavigationPage(page));
				}
				Menu.SelectedItem = null;
			}
		}

		public void RemoveMessageSubscription()
		{
			//Device.OnPlatform (Android: () => {
			//	MessagingCenter.Unsubscribe<CurrentLoad>(this, MessageTypes.Exception);
			//	MessagingCenter.Unsubscribe<SelectLoad>(this, MessageTypes.Exception);
			//	MessagingCenter.Unsubscribe<CurrentLoad> (this, MessageTypes.RemoveMessageSubscription);
			//	MessagingCenter.Unsubscribe<CurrentLoad>(this, MessageTypes.RefreshSetting);
			//             MessagingCenter.Unsubscribe<Login>(this, MessageTypes.RefreshSetting);
			//	MessagingCenter.Unsubscribe<AppSettings>(this, MessageTypes.LogOut);
			//	MessagingCenter.Unsubscribe<Page>(this, MessageTypes.LogOut);
			//	currentLoad.RemoveMessageSubscription();
			//});
			if (DeviceInfo.Current.Platform == DevicePlatform.Android) { 
				MessagingCenter.Unsubscribe<CurrentLoad>(this, MessageTypes.Exception);
			MessagingCenter.Unsubscribe<SelectLoad>(this, MessageTypes.Exception);
			MessagingCenter.Unsubscribe<CurrentLoad>(this, MessageTypes.RemoveMessageSubscription);
			MessagingCenter.Unsubscribe<CurrentLoad>(this, MessageTypes.RefreshSetting);
			MessagingCenter.Unsubscribe<Login>(this, MessageTypes.RefreshSetting);
			MessagingCenter.Unsubscribe<AppSettings>(this, MessageTypes.LogOut);
			MessagingCenter.Unsubscribe<Page>(this, MessageTypes.LogOut);
			currentLoad.RemoveMessageSubscription();
		}
		}
    }
}

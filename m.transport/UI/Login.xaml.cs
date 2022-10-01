using System;
using System.ComponentModel;
using System.Diagnostics;
using Autofac;
using m.transport.Data;
using m.transport.Models;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using Xamarin;
using m.transport.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using m.transport.Domain;
using System.Net;
using System.Threading.Tasks;
using m.transport.Cache;
using Newtonsoft.Json;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class Login : ContentPage
	{
		private ILoginRepository loginRepository;
		private bool addDispatchMode = false;
		private MainPage PageParent;
		private string dispatchName = "";
		private IAppSettingsRepository settingsRepo;

		public Login(MainPage parent)
		{
			this.PageParent = parent;
			bool isIOS = true;

			settingsRepo = App.Container.Resolve<IAppSettingsRepository>();

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
					isIOS = true;
				} else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
					isIOS = false;
				}
			

			loginRepository = App.Container.Resolve<ILoginRepository>();
			ViewModel = new LoginViewModel(
				App.Container.Resolve<IServiceClientFactory<ITransportServiceClient>>(),
                App.Container.Resolve<IServiceClientFactory<IRestServiceClient>>(),
                settingsRepo,
				loginRepository,
				App.Container.Resolve<ICurrentLoadRepository>(),
				DependencyService.Get<IBuildInfo>(), isIOS
                );
			InitializeComponent();

			if (Microsoft.Maui.Storage.Preferences.ContainsKey ("DispatcherName")) {
				try {
					dispatchName = (string)Microsoft.Maui.Storage.Preferences ["DispatcherName"];
				} catch (System.Exception ex) {
					dispatchName = "";
				}
			}

			String dispatchCode = ViewModel.GetDispatchCodeByName (dispatchName);

			if (!string.IsNullOrEmpty(dispatchName) && !string.IsNullOrEmpty(dispatchCode))
			{
			    //ViewModel.DispatchCode = dispatchCode[0];
				Dispatch.IsVisible = true;
				DispatchCode.IsVisible = false;
				AddDispatch.IsVisible = true;
				addDispatchMode = false;
				Code.Text = ViewModel.GetCachedDispatch (dispatchName);
				ViewModel.DispatchCode = dispatchCode;
			} else {
				addDispatchMode = true;
				Dispatch.IsVisible = false;
				DispatchCode.IsVisible = true;
				AddDispatch.IsVisible = false;
			}
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			App.Container.Resolve<IGPSLocation>().StopLocationTracking();
			var network = DependencyService.Get<INetworkAvailability>();
			if (loginRepository.LoginResult == null || String.IsNullOrEmpty(Code.Text))
			{
				return;
			}

			AutoLogin();
		}

		private async void SettingsRepo_GetMobileSettingsCompleted (object sender, GetMobileSettingsCompletedEventArgs e)
		{
			settingsRepo.GetMobileSettingsCompleted -= SettingsRepo_GetMobileSettingsCompleted;

			Device.BeginInvokeOnMainThread (() => {
				IsBusy = false;
			});

			if (e.Error == null)
            {
                // this is a hack to temporarily bypass this call to test restful login
                // should be undone once GetMobileSettingsAsync is migrated to REST

                VersionStatus status = settingsRepo.CheckAppVersion ();

                #if DEBUG
                	status = VersionStatus.OK;
#endif

                if (status == VersionStatus.UpdateRequired)
                {

                    this.EndCallToServerAsync(e);

                    Device.BeginInvokeOnMainThread(async () =>
                    {

                        bool choice = await DisplayAlert("Update Required", settingsRepo.UpdateMessage, "Update", "Cancel");
                        if (choice)
                        {
                            Device.OpenUri(new Uri(settingsRepo.AppStoreUrl));
                        }
                        else
                        {
                            // allow login regardless when in debug mode
#if !DEBUG
                			return;
#endif

#if DEBUG
                            if (await this.BeginCallToServerAsync("Logging in..."))
                            {
                                ViewModel.LoginAsync();
                            }
#endif
                        }
                    });

                }
                else if (status == VersionStatus.UpdateAvailable)
                {

                    this.EndCallToServerAsync(e);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        bool choice = await DisplayAlert("Update Available", settingsRepo.UpdateMessage, "Update", "No");
                        if (choice)
                        {
                            Device.OpenUri(new Uri(settingsRepo.AppStoreUrl));
                        }
                        else
                        {
                            if (await this.BeginCallToServerAsync("Logging in..."))
                            {
                                ViewModel.LoginAsync();
                            }
                        }
                    });
                }
                else
                {
                    ViewModel.LoginAsync();
                }
			} else {
				this.EndCallToServerAsync(e);

				Device.BeginInvokeOnMainThread(() => {
					DisplayAlert("Error!",
					   "There was a problem fetching mobile setting. Please contact Dispatch to resolve.", "OK");
					ViewModel.LoginAsync();
				});
			}
		}


		private async void GetMobileSetting()
		{
            var network = DependencyService.Get<INetworkAvailability>();

            if (!network.IsNetworkAvailable())
		    {
                ViewModel.LoginAsync();
                return;
		    }

		    settingsRepo.GetMobileSettingsCompleted += SettingsRepo_GetMobileSettingsCompleted;

			Device.BeginInvokeOnMainThread (async () => {
				if (await this.BeginCallToServerAsync ("Logging in...")) {

                    settingsRepo.GetMobileSettingsAsync ();
 				}
			});
		}

	    private async void AutoLogin() {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    IHardwareInfo hardwareInfo = App.Container.Resolve<IHardwareInfo>();
                    if (hardwareInfo.GetClearDataCompleted())
                    {
                        DependencyService.Get<IAlert>().ShowAlert("Update Complete, please restart app manually!", "", new[] { "OK" }, HandleSelection);
                        return;
                    }
                    break;
            }

            var serverUrl = loginRepository.LoginResult.ServerUrl;
			ViewModel.FillCacheCredential();
			IsBusy = true;

			GetMobileSetting ();

		}

		private async void HandleSelection(string result)
		{
			if (result == "OK")
			{
				App.Container.Resolve<IHardwareInfo>().ClearData();
			}
		}

		public async void OnShowDispatchAction(object sender, EventArgs args)
		{

			string result = await DisplayActionSheet("Dispatcher","Cancel",null,ViewModel.GetDispatch());		
			string key = string.Empty;

            if (result == null) return;

            switch (result) {
				case "Cancel":
					break;
			default:
				if (result.Contains (":")) {
					key = result.Substring (0, result.IndexOf (":"));
				}
			    ViewModel.DispatchCode = key;
				Code.Text = result;
				break;
			}

		}
			
		public LoginViewModel ViewModel
		{
			get { return (LoginViewModel)BindingContext; }
			set
			{
				var oldViewModel = BindingContext as LoginViewModel;
				if (oldViewModel != null)
				{
					oldViewModel.LoginSucceeded -= OnLoginSucceeded;
					oldViewModel.LoginFailed -= OnLoginFailed;
				}
				BindingContext = value;
				if (value != null)
				{
					value.LoginSucceeded += OnLoginSucceeded;
					value.LoginFailed += OnLoginFailed;
				}
			}
		}


	    private void OnLoginFailed(AsyncCompletedEventArgs args)
		{

			this.EndCallToServerAsync(null);

			if (args == null)
			{
                var network = DependencyService.Get<INetworkAvailability>();
                if (!network.IsNetworkAvailable())
                {
                    DisplayAlert("Network Unavailable!", "Please try again when network is available", "OK");
                }
				return;
			}

			if (args.Error.Message.Contains("103"))
			{
				// the assigned truck number will be between "[truck]" in the error - we need to capture it
				string existingTruck;

				try {
					int s = args.Error.Message.IndexOf ('[');
					int e = args.Error.Message.IndexOf (']');
					int l = e - s - 1;
				
					existingTruck = args.Error.Message.Substring (s + 1, l);
				} catch (System.Exception ex) {
					existingTruck = string.Empty;
				}

                if (string.IsNullOrEmpty(existingTruck))
                {
                    string err = args.Error.Message;

                    var split = err.Split(';');

                    if (split.Length > 1)
                    {
                        existingTruck = split[1];
                    }
                   
                }

				Device.BeginInvokeOnMainThread(() => ReplaceExistingTruck(existingTruck));
			}
			
		}

		private async void ReplaceExistingTruck(string existingTruck)
		{
			bool resp = await DisplayAlert("No Matching Truck", "Would you like to report using a different truck?", "Yes", "No");

			if (resp)
			{
				ViewModel.UpdateTruckAsync (existingTruck);
			}

		}
			
		public void OnAddDispatch(object sender, EventArgs args) {
			addDispatchMode = true;
			Device.BeginInvokeOnMainThread (() => {
				Dispatch.IsVisible = false;
				DispatchCode.IsVisible = true;
				AddDispatch.IsVisible = false;
			});
		}

		public async void OnLogin(object sender, EventArgs args)
		{
            if (!ViewModel.IsValid) return; 
            if (!addDispatchMode)
            {
			
				string code = Code.Text;
				if (code.Contains (":")) 
                {
					code = code.Substring (0, code.IndexOf (":"));
				}

				Server server = ViewModel.GetServer (code.Trim());
				ViewModel.SetServer (server.Url);
				SetDispatcherName (server.Name);
			} 
            else 
            {
				Server server = await ValidateDispatcher (ViewModel.DispatchCode);

				if (server != null) 
                {
				
					ViewModel.SetServer (server.Url);
					SetDispatcherName (server.Name);
				} 
                else
                {
					return;
				}
			}

			IsBusy = true;
			GetMobileSetting ();
		}

		private async Task<Server> ValidateDispatcher(string code) {

			Server server = null;
			string json = "";

			// LOC is for local debugging against a local copy of the services
			if (code.ToUpper() == "LOC") {

				server = new Server ();
				server.Code = "LOC";
				server.Name = "LOC";

				// IP and path will vary per developer's VM setup
				//server.Url = "http://192.168.0.17:43001/Service.svc/soapssl";
				server.Url = "http://192.168.8.198/TransportWebApp/Service.svc/soapssl";


				return server;
			}

            if (code.Contains(":"))
            {
                var ndx = code.IndexOf(":",StringComparison.CurrentCultureIgnoreCase);

                code = code.Substring(0, ndx - 1);
            }

			var client = new System.Net.Http.HttpClient ();
			if (await this.BeginCallToServerAsync ("Connecting To Dispatch...")) {

				try {
					json = await client.GetStringAsync ("http://trans1.mophilly.com/TransportAdmin/ServerService.svc/GetServer/" + code);
					server = JsonConvert.DeserializeObject<Server> (json);
					ViewModel.SaveDispatchServer(server);
					this.EndCallToServerAsync ();
				} catch (System.Exception ex) {
					this.EndCallToServerAsync ();
					DisplayFailedLogin ("Dispatch connection error");
				}

			}
				
			return server;
		}

		private void DisplayFailedLogin(string err) {
			ViewModel.Error = err;
			ViewModel.IsLoginFailed = true;
		}

		public async void OnLoginDemo(object sender, EventArgs args)
		{
			// 6293 - always use DEV server for DEMO
			ViewModel.SetServer (LoginViewModel.DemoURL);
			SetDispatcherName("DEMO");

			if (await this.BeginCallToServerAsync("Logging in..."))
			{
				ViewModel.LoginAsync(new DemoLoginInfo());
			}
		}

		private async void SetDispatcherName(string name) {

            if (name == "DEMO") 
            {
                return;
            }

			if (Microsoft.Maui.Storage.Preferences.ContainsKey ("DispatcherName")) {
                Microsoft.Maui.Storage.Preferences["DispatcherName"] = name;
			} else {
                Microsoft.Maui.Storage.Preferences.Add ("DispatcherName", name);
			}

			App.Current.SavePropertiesAsync ();
		}

		private async void OnLoginSucceeded()
		{
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    IHardwareInfo hardwareInfo = App.Container.Resolve<IHardwareInfo>();
                    hardwareInfo.SetClearDataCompleted();
                    break;
            }
            //InsightsIdentify();

            this.EndCallToServerAsync();
				
			PageParent.LoginPage = false;

			if (loginRepository.LoginResult.GPSTrackingInd == 1)
            {
				if (App.Container.Resolve<IGPSLocation>().CheckLocationPermission())
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						await DisplayAlert("Not authorized", "GPS Location is turned off. Go To Setting > Vehicle Mobile > Location to turn on", "OK");
					});
					return;
				}


                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool resp = await DisplayAlert("GPS required", "Do you agree to share your location?", "Yes", "No");
                    if (resp)
                    {
                        bool success = await LocationManager.Instance.TurnOnGPSTrackingAsync();

                        if (success)
                        {
                            completeLogin();
                        }
                        else
                        {
                            bool agree = await DisplayAlert("Attention!", "By selecting no, your loads cannot be tracked and you will not be eligible to haul this load. Do you agree to share your location?", "Yes", "No");
                            if (agree)
                            {
                                bool trackingEnabled = await LocationManager.Instance.TurnOnGPSTrackingAsync();
                                if (trackingEnabled)
                                {
                                    completeLogin();

                                }
                                else
                                {
                                    await DisplayAlert("Error", "Unable to login", "Ok");
                                    LocationManager.Instance.SendEmailNotification();
                                }

                            }
                        }
                    }
                    else
                    {
                        bool agree = await DisplayAlert("Attention!", "By selecting no, your loads cannot be tracked and you will not be eligible to haul this load. Do you agree to share your location?", "Yes", "No");
                        if (agree)
                        {
                            bool trackingEnabled = await LocationManager.Instance.TurnOnGPSTrackingAsync();
                            if (trackingEnabled)
                            {
                                completeLogin();

                            }
                            else
                            {
                                await DisplayAlert("Error", "Unable to login", "Ok");
                                LocationManager.Instance.SendEmailNotification();
                            }

                        }
                        else
                        {
                            await DisplayAlert("Error", "Unable to login", "Ok");
                            LocationManager.Instance.SendEmailNotification();
                        }
                    }
                });
            } else
            {
				completeLogin();
			}
		}

		private void completeLogin()
        {
			App.Container.Resolve<IGPSLocation>().StartLocationTracking();

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
				MessagingCenter.Send<Login>(this, MessageTypes.RefreshCurrentLoad);
				MessagingCenter.Send<Login>(this, MessageTypes.RefreshSetting);
			}

			Device.BeginInvokeOnMainThread(async () =>
			{
				// 6715 - try to prevent an InvalidNavigationException
				try
				{
					await Navigation.PopModalAsync();
				}
				catch (System.Exception ex)
				{
					//InsightsManager.Track("Error popping login modal: " + ex.Message);
				}
			});
		}


#if DEBUG
		[Conditional("FALSE")]
#endif
		public void InsightsIdentify()
		{
			var traits = new Dictionary<string, string> {
				{ "Dispatcher", Microsoft.Maui.Storage.Preferences ["DispatcherName"].ToString() },
				{ Insights.Traits.Name, ViewModel.Username }
			};
			//Insights.Identify(ViewModel.Username, traits);
		}
	}
}

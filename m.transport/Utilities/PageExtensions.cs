using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using m.transport.Interfaces;
using Xamarin;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.Utilities
{
	public static class PageExtensions
	{
		private static bool enableFlag = false;
        private static HashSet<string> idSets = new HashSet<string>();

		public static async Task<bool> BeginCallToServerAsync(this Page page, string message, string actionId = null, Action callback = null, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, bool offline = false)
		{
			TrackInfo(callerFilePath, callerMemberName);
			bool cont = false;
			var settingsRepo = App.Container.Resolve<IAppSettingsRepository>();
            idSets.Add(actionId);
			if (settingsRepo.VersionStatus == VersionStatus.UpdateRequired) {

#if DEBUG
				System.Diagnostics.Debug.WriteLine ("App version is out of date");
#else
				//omit login page since login has it's own version check
				if(!callerFilePath.EndsWith("Login.xaml.cs")){
					bool choice = await page.DisplayAlert ("Update Required", "This version of the mobile app is unable to communicate with Dispatch, please install the latest version now.", "Update", "Cancel");

						if (choice) {
							Device.OpenUri (new Uri (settingsRepo.AppStoreUrl));
						} 

					return false;
				}
#endif

			} 
				
			// if this is an offline operation, don't check network status
			if (offline) {
				cont = true;
				DependencyService.Get<IHud> ().Show (message);
			} else {
				var network = DependencyService.Get<INetworkAvailability> ();
				if (network.IsNetworkAvailable ()) {
					cont = true;
					enableFlag = true;
					DependencyService.Get<IHud> ().Show (message);
                    //await page.DisplayAlert("Time Out", "No reponse from server, please try again!", "OK");
                    //StartTimer(page, actionId, callback);
                      
				} else {
					await page.DisplayAlert ("No Network", "Network access is required to use this feature.  Connect to Cellular or Wifi network and try again", "OK");
				}
			}

			return cont;
		}

        private static async Task StartTimer(Page p, string actionId, Action callback) {
            await Task.Delay(45000);
            if (!String.IsNullOrEmpty(actionId) && idSets.Contains(actionId)) {
                idSets.Remove(actionId);
                DependencyService.Get<IHud>().Dismiss();
                await p.DisplayAlert("Time Out", "No reponse from server, please try again!", "OK");
                if (callback != null)
                {
                    callback();
                }

            }
        }
			
        public static void EndCallToServerAsync(this Page page, AsyncCompletedEventArgs args = null, string actionId = null, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
		{
            idSets.Remove(actionId);
			TrackInfo(callerFilePath, callerMemberName);
			string msg = "";
			enableFlag = false;
			Device.BeginInvokeOnMainThread(() =>
			{
				DependencyService.Get<IHud>().Dismiss();

				if (args != null && args.Error != null)
				{

					Report(args.Error);
					if(args.Error.Message != null){
						msg = args.Error.Message;
					}

					if(msg.Contains("timed-out")){
						Report(callerMemberName + msg);
						msg = "Unable to communicate with dispatch, please try again";
					}
					page.DisplayAlert("Server Error", msg, "OK");
				}
			});
		}

#if DEBUG
		[Conditional("FALSE")]
#endif
		public static void TrackInfo(string callerFilePath, string callerMemberName, [CallerMemberName] string caller = null)
		{
            Analytics.TrackEvent(caller, new Dictionary<string, string>{
				{"CallerFilePath", callerFilePath},
				{"CallerMemberName", callerMemberName}
			});
		}

		#if DEBUG
		[Conditional("FALSE")]
		#endif
		public static void Report(System.Exception ex)
		{
            Crashes.TrackError(ex);
		}
			
		public static void Report(string errorMessage)
		{
			Crashes.TrackError (new System.Exception(errorMessage));
		}

	}
}
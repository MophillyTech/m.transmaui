using System;
using System.ComponentModel;
using System.Threading;
using m.transport.Data;
using m.transport.Svc;
using m.transport.Domain;
using System.Collections.Generic;
using m.transport.Interfaces;
using m.transport.Cache;
using Autofac;
using System.Threading.Tasks;
using System.Linq;
using m.transport.ServiceInterface;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private ILoginRepository loginRepository;
		private readonly IAppSettingsRepository appSettingsRepository;
		private readonly ICurrentLoadRepository loadRepository;
		private CountdownEvent countdownEvent;
		public event EventHandler<AsyncCompletedEventArgs> RefreshAppSettingsComplete = delegate { };
		private List<string> codeTypeList = CodeType.CodeTypeNameList;
		private bool codeTableCallBackEnabled;
		private double THREE_HOURS = 10800000;
		private readonly ICache _globalCache;

		public MainViewModel(ILoginRepository loginRepository, 
			IAppSettingsRepository appSettingsRepository,
			ICurrentLoadRepository loadRepository)
		{
			this.loginRepository = loginRepository;
			this.appSettingsRepository = appSettingsRepository;
			this.loadRepository = loadRepository;
			this._globalCache = App.Container.Resolve<ICache>();
		}

		public async Task Logout()
		{
            String settingString = string.Join(",", appSettingsRepository.Settings.Select(kv => kv.Key + "=" + kv.Value).ToArray());
            LoginResult r = loginRepository.GetUserById(loginRepository.LoginResult.Driver.GetHashCode());
            r.Setting = settingString;
            loginRepository.Save(r);

            r.Id = 2;
            //ID = 2 is previous user
            loginRepository.Save(r); 

			loginRepository.Clear();
			loginRepository.FirstTimeLogin = true;
			appSettingsRepository.InitSetting();
        }


		public void UpdateConfiguration(bool checkSyncPeriod = true) 
		{

			if (DependencyService.Get<INetworkAvailability>().IsNetworkAvailable ()) 
			{
				if (checkSyncPeriod) 
				{

					DateTime lastSync = appSettingsRepository.MobileSettingSyncTime;
					TimeSpan diff = DateTime.Now - lastSync;
					if (diff.TotalMilliseconds < THREE_HOURS) {
						System.Diagnostics.Debug.WriteLine ("Syncing Setting not yet");
						return;
					}
				}
				System.Diagnostics.Debug.WriteLine ("Syncing Setting call");
				appSettingsRepository.MobileSettingSyncTime = DateTime.Now;
				RefreshAppSettingsAsync ();
			}

		}
			
		public void RefreshAppSettingsAsync()
		{
            countdownEvent = new CountdownEvent(3);

			appSettingsRepository.GetDamageCodeListCompleted += AppSettingsRepositoryOnGetDamageCodeListCompleted;
			appSettingsRepository.GetDamageCodeListAsync();
			appSettingsRepository.GetMobileSettingsCompleted += AppSettingsRepositoryOnGetMobileSettingsCompleted;
			appSettingsRepository.GetMobileSettingsAsync();

			RefreshCodeTable ();
		}
			
		public void RefreshCodeTable(bool callBack = true)
		{

			codeTableCallBackEnabled = callBack;
			appSettingsRepository.GetCodeTableCompleted += AppSettingsRepositoryOnGetCodeTableCompleted;

			string result = String.Join(",", codeTypeList.ToArray());

			appSettingsRepository.RefreshCodes(result.ToString());
		}
			

		private void AppSettingsRepositoryOnGetCodeTableCompleted(object sender, GetCodesCompletedEventArgs args)
		{
			appSettingsRepository.GetCodeTableCompleted -= AppSettingsRepositoryOnGetCodeTableCompleted;
			if(codeTableCallBackEnabled)
				CallbackWhenComplete(sender, args);
		}
		
		private void AppSettingsRepositoryOnGetMobileSettingsCompleted(object sender, GetMobileSettingsCompletedEventArgs args)
		{
			appSettingsRepository.GetMobileSettingsCompleted -= AppSettingsRepositoryOnGetMobileSettingsCompleted;
			CallbackWhenComplete(sender, args);
		}

		private void AppSettingsRepositoryOnGetDamageCodeListCompleted(object sender, GetDamageCodeListCompletedEventArgs args)
		{
			appSettingsRepository.GetDamageCodeListCompleted -= AppSettingsRepositoryOnGetDamageCodeListCompleted;
			CallbackWhenComplete(sender, args);
		}
					
		private void CallbackWhenComplete(object sender, AsyncCompletedEventArgs args)
		{
			countdownEvent.Signal();
			if (countdownEvent.IsSet)
			{
				RefreshAppSettingsComplete(sender, args);
			}
		}

		public string GetDriverType(){
			return loginRepository.DriverType;
		}

		public int GetOutsideCarrierInd(){
			return (int) loginRepository.OutsideCarrierInd;
		}

		public int GetOutsideCarrierCompany(){
			return (int) loginRepository.OutsideCarrierCompany;
		}

		public int GetMobilePayHistoryInd(){
			return appSettingsRepository.MobilePayHistoryInd;
		}

		public int GetMobileExpenseInd(){
			return appSettingsRepository.MobileExpenseInd;
		}

	}
}
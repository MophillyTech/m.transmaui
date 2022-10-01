using System;
using m.transport.Data;
using m.transport.Models;
using m.transport.Svc;
using m.transport.Interfaces;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;
using Autofac;
using System.Collections.Generic;
using System.Threading.Tasks;
using m.transport.Cache;
using m.transport.Dto;
using m.transport.ServiceInterface;
using m.transport.Domain;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
//		public Dictionary<string, Tuple<string, string>> urls;
		private bool isLoginFailed;
		private string error;
		private readonly ILoginRepository loginRepo;
		private readonly ICurrentLoadRepository loadRepo;
		private readonly IBuildInfo buildInfo;
		private readonly IAppSettingsRepository settingRepo;
		private IServiceClientFactory<ITransportServiceClient> clientFactory;
        private IServiceClientFactory<IRestServiceClient> restClientFactory;
		private string imgSource = "disclosure_up.png";
		private bool isIOS;
		private const string CURRENT_VERSION = "v4";
		private const string PREVIOUS_VERSION = "v3";
	    private readonly ICache _globalCache;
	    public string DispatchCode {get; set;}
		private List<Server> dispatchServers;
		public const string DemoURL = "https://trans1.mophilly.com/transportws/Service.svc/soapssl";

		public event Action LoginSucceeded = delegate { };
		public event Action<AsyncCompletedEventArgs> LoginFailed = delegate { };

		public LoginViewModel(
			IServiceClientFactory<ITransportServiceClient> clientFactory,
            IServiceClientFactory<IRestServiceClient> restClientFactory,
            IAppSettingsRepository settingRepo,
			ILoginRepository loginRepo, 
			ICurrentLoadRepository loadRepo,
			IBuildInfo buildInfo,
			bool isIOS
			)
		{
			this.clientFactory = clientFactory;
            this.restClientFactory = restClientFactory;
			this.settingRepo = settingRepo;
			this.loginRepo = loginRepo;
			this.loadRepo = loadRepo;
			this.buildInfo = buildInfo;
			this.isIOS = isIOS;
		    this._globalCache = App.Container.Resolve<ICache>();

			dispatchServers = settingRepo.GetDispatchServers ();

//			urls = new Dictionary<string, Tuple<string, string>> ();
//#if DEBUG
//			urls.Add ("LOC", new Tuple<string,string> ("LOC", "http://192.168.2.222/transportwebapp/Service.svc/soapssl"));
//#endif
//			urls.Add ("MOPD", new Tuple<string,string> ("DEV", "https://trans1.mophilly.com/transportws/Service.svc/soapssl"));
//			urls.Add ("DAID", new Tuple<string,string> ("DAI Test", "http://alm.diversifiedauto.com:8083/dev/Service.svc/soap"));
//			urls.Add ("DAI", new Tuple<string,string> ("DAI Production", "http://alm.diversifiedauto.com:8083/prod/Service.svc/soap"));
//			urls.Add ("EXEL", new Tuple<string,string> ("Excel", "https://trans1.mophilly.com/transportexcel/Service.svc/soapssl"));
//			urls.Add ("OAKW", new Tuple<string,string> ("Oakwood", "https://trans1.mophilly.com/transportoakwood/Service.svc/soapssl"));
//			urls.Add ("TRIB", new Tuple<string,string> ("Tribeca", "https://trans1.mophilly.com/transporttribeca/Service.svc/soapssl"));

		}

		public void SetServer(string serverUrl)
		{
			var url = serverUrl;
            if (!url.Contains(CURRENT_VERSION) && !url.Contains(PREVIOUS_VERSION))
            {
				int index = url.IndexOf("/Service.svc");
				url = url.Insert(index, CURRENT_VERSION);
			} else if (url.Contains(PREVIOUS_VERSION))
            {
				url = url.Replace(PREVIOUS_VERSION, CURRENT_VERSION);
			}

			clientFactory.Url = url;
			restClientFactory.Url = url;
			System.Diagnostics.Debug.WriteLine (clientFactory.Url);
		}

		private string username;

		public string Username
		{
			get { return username; }
			set
			{
				if (username != value)
				{
					username = value.Trim();
					RaisePropertyChanged();
				}
			}
		}

		private string password;

		public string Password
		{
			get { return password; }
			set
			{
				if (password != value)
				{
					password = value.Trim();
					RaisePropertyChanged();
				}
			}
		}

		public string ImgSource
		{
			get { return imgSource; }
			set
			{
				if (imgSource != value)
				{
					imgSource = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool IsValid
		{
			get
			{
				if (String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(DispatchCode) || String.IsNullOrEmpty(Truck) || String.IsNullOrEmpty(Username))
				{
					Error = "Please fill in the login information";
					IsLoginFailed = true;
					return false;
				}

				Error = "";
				IsLoginFailed = false;
				return true;
			}
		}

		private string truck;

		public string Truck
		{
			get { return truck; }
			set
			{
				if (truck != value)
				{
					int index = value.IndexOf (' ');
					if(index != 0)
						truck = value;
					RaisePropertyChanged();
				}
			}
		}

		public string Error
		{
			get { return error; }
			set
			{
				if (error != value)
				{
					error = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool IsLoginFailed
		{
			get { return isLoginFailed; }
			set
			{
				if (isLoginFailed != value)
				{
					isLoginFailed = value;
					RaisePropertyChanged();
				}
			}
		}

		public void SaveDispatchServer(Server server)
		{
			settingRepo.AddDispatchServer (server);

		}

		public string GetCachedDispatch(string dispatchName) 
		{
			foreach (Server server in dispatchServers) 
			{
				if(server.Name.Equals(dispatchName)) {
					return server.Code + " : " + server.Name;
				}
			}

			return "";
		}

		public string GetDispatchCodeByName(string name) {
			Server server = (Server) dispatchServers.FirstOrDefault (u => u.Name == name);
			return server != null ? server.Code : "";
		}

		public Server GetServer(string code) {
			return dispatchServers.Where (u => u.Code == code).FirstOrDefault ();
		}

		public string[] GetDispatch()
		{
			List<string> dispatch = new List<string>();
		
			foreach (Server server in dispatchServers) 
			{
				dispatch.Add (server.Code + " : " + server.Name);
			}

			return dispatch.ToArray ();

		}

		public string VersionInfo 
		{
			get
			{

			//	if (isIOS) {
					return "Version: " + buildInfo.Version + " (" + buildInfo.BuildNumber + ")";
			//	}
					
			//	return "Version: " + buildInfo.BuildNumber + " (Build: " + buildInfo.Version  + ")";
			}
		}

		public void UpdateTruckAsync(string oldTruck)
		{
			loginRepo.UpdateTruckCompleted += OnUpdateTruckCompleted;
			IsLoginFailed = false;
			loginRepo.UpdateTruckAsync(new LoginInfo(Username, Password, oldTruck), Truck);
		}

		public async void LoginAsync(LoginInfo loginInfo = null)
		{
			loginRepo.LoginCompleted += OnLoginCompleted;

			if (loginInfo == null)
			{
				loginInfo = new LoginInfo(Username, Password, Truck);

			}
			else {
				Username = loginInfo.Username;
			}
			IsLoginFailed = false;

			if (!DependencyService.Get<INetworkAvailability>().IsNetworkAvailable())
			{
				loginRepo.LoginWithExistingCredential(loginInfo);
			}
			else 
			{
				loginRepo.LoginAsync(loginInfo);
			}
		}

	    private async void OnLoginCompleted(object sender, LoginCompletedEventArgs args)
		{
			loginRepo.LoginCompleted -= OnLoginCompleted;

			if (args == null)
			{
				Error = "Login Failed!";
				IsLoginFailed = true;

                LoginFailed(null);
			} 
			else if (args.Error != null)
			{
				IsLoginFailed = true;
				ParseError(args.Error.Message);
				LoginFailed(args);
			}
			else
			{
                LoginResult r = loginRepo.GetUserById(2);

                if (r == null || r.Driver.ToUpper() != Username.ToUpper())
				{
					App.Container.Resolve<IAppSettingsRepository>().ClearDeliveryHistory();
					loadRepo.ClearLoadInfo();
				}

                restoreSettingConfig(loginRepo.LoginResult.Setting);

				IsLoginFailed = false;

                System.Guid? installId = await AppCenter.GetInstallIdAsync();
                Analytics.TrackEvent("Login", new Dictionary<string, string>{
                    {"id", installId.ToString()},
                    {"username", username },
                    {"truck", truck },
                    {"dispatch", DispatchCode }
                });
                         
                Crashes.GetErrorAttachments = (ErrorReport report) =>
                {
                    // Your code goes here.
                    return new ErrorAttachmentLog[]
                    {
                        ErrorAttachmentLog.AttachmentWithText($"username={username},trucknum={truck},dispatch={DispatchCode}","userinfo.txt")
                    };
                };

				LoginSucceeded();
			}
		}

        private void restoreSettingConfig(string settings) 
        {
            if (String.IsNullOrEmpty(settings))
            {
                return;
            }

            Dictionary<string, string> config = new Dictionary<string, string>();
            List<string> values = settings.Split(',').ToList<string>();
            foreach (string setting in values) {
                List<string> pair = setting.Split('=').ToList<string>();
                config.Add(pair[0], pair[1]);
            }

            settingRepo.Settings = config;

        }

		private void ParseError(string error)
		{

			if (error.Contains("100"))
			{
				Error = "Login Failed!";
			}
			else if (error.Contains("103"))
			{
				Error = "The Current Truck Number Doesn't Match On File";
			}
			else if (error.Contains("200"))
			{
				Error = "Error Updating Truck Number For Driver";
			}
			else
			{
				Error = error;
			}

		}

		private void OnUpdateTruckCompleted(object sender, AsyncCompletedEventArgs args)
		{
			loginRepo.UpdateTruckCompleted -= OnUpdateTruckCompleted;
			if (args.Error != null)
			{
				IsLoginFailed = true;
				ParseError(args.Error.Message);
				LoginFailed(args);
			}
			else
			{
				loginRepo.LoginCompleted += OnLoginCompleted;
				IsLoginFailed = false;
				loginRepo.LoginAsync(new LoginInfo(Username, Password, Truck));
			}
		}

		public void FillCacheCredential() {
			Username = loginRepo.LoginResult.Driver;
			Password = loginRepo.LoginResult.Password;
			Truck = loginRepo.LoginResult.Truck;
		}
	}
}

using System;
using System.Linq;
using System.ServiceModel;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Models;
using m.transport.Svc;
using m.transport.ServiceInterface;
using System.ComponentModel;
using System.Collections.Generic;
using m.transport.Cache;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.Data
{
	public class LoginRepository : ILoginRepository
	{
		private readonly IServiceClientFactory<ITransportServiceClient> serviceClientFactory;
        private readonly IServiceClientFactory<IRestServiceClient> restClientFactory;
		readonly IRepository<LoginResult, int> repo;
	    private readonly ICache _globalCache;
		public bool FirstTimeLogin { get; set; }

	    public LoginRepository(
			IServiceClientFactory<ITransportServiceClient> serviceClientFactory,
            IServiceClientFactory<IRestServiceClient> restClientFactory,
            IRepository<LoginResult, int> repo,
            ICache globalCache)
		{
			this.serviceClientFactory = serviceClientFactory;
            this.restClientFactory = restClientFactory;
			this.repo = repo;
	        _globalCache = globalCache;
			FirstTimeLogin = true;

	        GetCachedCredentials();
		}

		private void ClientOnUpdateTruckCompleted (object sender, AsyncCompletedEventArgs args)
		{
			serviceClientFactory.Instance.UpdateTruckCompleted -= ClientOnUpdateTruckCompleted;
			UpdateTruckCompleted (sender, args);
		}

		public void LoginWithExistingCredential(ILoginInfo loginInfo)
		{
			LoginResult result = repo.GetById(loginInfo.Username.GetHashCode());
			if (result != null)
			{
				if (loginInfo.Username != result.Driver ||
				   loginInfo.Password != result.Password ||
				   loginInfo.Truck != result.Truck)
				{
					LoginCompleted(null, null);
				}
				else 
				{
					LoginResult = result;
					LoginResult.Id = 1;
					LoginResult.ServerUrl = result.ServerUrl;
                    LoginResult.CompanyInfo = new CompanyInfo(result.CompanyInfoToString);
					repo.Save(LoginResult);
					LoginCompleted(null, new LoginCompletedEventArgs(null, null, false, null));
                    IsLoggedIn = true;
				}
			}
			else 
			{
				LoginCompleted(null, null);
			}
		}

		private void ClientOnLoginCompleted(object sender, LoginCompletedEventArgs args)
		{
			serviceClientFactory.Instance.LoginCompleted -= ClientOnLoginCompleted;
			if (args.Error == null)
			{
				//ID = hashcode is existing driver that can be used for offline login
				LoginResult cachedResult = args.Result;
				cachedResult.Id = args.Result.Driver.GetHashCode();
                cachedResult.CompanyInfoToString = args.Result.CompanyInfo.ToString();
				cachedResult.ServerUrl = serviceClientFactory.Url;
				repo.Save(cachedResult);

                if (LoginResult != null) 
                {
					LoginResult.Id = 2;
					//ID = 2 is previous user
					repo.Save(LoginResult);    
                }

				LoginResult = args.Result;
				IsLoggedIn = true;
                Save();
			}
			else
			{
				IsLoggedIn = false;
                /*
				var fault = args.Error as FaultException<TruckInfo>;
				if (fault != null)
				{
					var truckOnFile = fault.Detail.TruckOnFile;
				}
				*/
			}

            //serviceClientFactory.Instance.LogMobileDeviceAsync (MobileDeviceBuilder.BuildMobileDevice());
            restClientFactory.Instance.LogMobileDeviceAsync(MobileDeviceBuilder.BuildMobileDevice());

			LoginCompleted(sender, args);
		}

		public void Save()
		{
            LoginResult.Id = 1;
			LoginResult.ServerUrl = serviceClientFactory.Url;
			repo.Save(LoginResult);
		}

        public void Save(LoginResult result)
		{
			repo.Save(result);
		}

	    public void SetLoginResult(LoginResult loginResult)
	    {
	        LoginResult = loginResult;
	    }

		public bool IsLoggedIn { get; set; }
		public string Username
		{
			get { return LoginResult != null ? LoginResult.Driver : string.Empty; }
		}
		public string FullName
		{
			get { return LoginResult != null ? LoginResult.FullName : string.Empty; }
		}
		public string Password
		{
			get { return LoginResult != null ? LoginResult.Password : string.Empty; }
		}
		public string Truck
		{
			get { return LoginResult != null ? LoginResult.Truck : string.Empty; }
		}
		public string AccountType
		{
			get { return (Username.ToUpper() == "ALDEMO") ? "DEMO" : "USER"; }
		}
		public string DriverType
		{
			get { return LoginResult != null ? LoginResult.DriverType : string.Empty; }
		}
		public int OutsideCarrierInd
		{
			get {
				if (LoginResult != null && LoginResult.OutsideCarrierInd != null) {
					return (int) LoginResult.OutsideCarrierInd;
				} else {
					return 0;
				}
			}
		}
		public int OutsideCarrierCompany
		{
			get {
				if (LoginResult != null && LoginResult.OutsideCarrierCompany != null) {
					return (int) LoginResult.OutsideCarrierCompany;
				} else {
					return 0;
				}
			}
		}
        public int TruckSleeperInd
        {
            get
            {
                if (LoginResult != null && LoginResult.SleeperCabInd != null)
                {
                    return (int)LoginResult.SleeperCabInd;
                }
                else
                {
                    return 0;
                }
            }
        }
		public LoginResult LoginResult { get; set; }

		public void Clear()
		{
			if (LoginResult != null)
			{
				LoginResult = null;
				repo.Delete(repo.GetById(1));
			}

			IsLoggedIn = false;
		}

		public async void LoginAsync(ILoginInfo loginInfo)
		{
            var network = DependencyService.Get<INetworkAvailability>();

		    if (!network.IsNetworkAvailable())
		    {
                await CachedLogin(loginInfo);
                return;
		    }

            try
            {
                var result = await restClientFactory.Instance.LoginAsync(loginInfo.Username, loginInfo.Password, loginInfo.Truck);

                Exception exc = null;

                if (result.Driver == null)
                {
                    exc = new Exception($"103;{result.Truck}");
                }

                ClientOnLoginCompleted(null, new LoginCompletedEventArgs(new object[] { result }, exc, false, null));
            }
			catch (Exception ex)
            {
               
				var result = new LoginResult() {  };
				if (ex.Message.Contains("111"))
                {
					String message = ex.Message;
					string[] stringSeparators = new string[] { "111," };
					string[] mesgs = message.Split(stringSeparators, StringSplitOptions.None);
					//string[] mesg = ex.Message.Split("111",",");
					String errMessage = "";
					foreach (string m in mesgs) {
						errMessage = m;
					}
					ex = new WASException(111, errMessage.ToString().Replace(").",""));
				}
                else
				{
					ex = new Exception("Login failed");
				}
                ClientOnLoginCompleted(null, new LoginCompletedEventArgs(new object[] { result }, ex, false, null));
            }

            //serviceClientFactory.Instance.LoginCompleted += ClientOnLoginCompleted;
            //serviceClientFactory.Instance.LoginAsync(loginInfo.Username, loginInfo.Password, loginInfo.Truck);
        }

        /// <summary>
        /// Login from cached credentials
        /// </summary>
        /// <returns></returns>
	    public async Task CachedLogin(ILoginInfo loginInfo)
        {
            var currentUserCredentials = await _globalCache.GetLastLoggedInUser();

            if (currentUserCredentials == null)
            {
                LoginCompleted(null, new LoginCompletedEventArgs(null, new Exception("100"), false, null));
                return;
            }

            //Validates user credentials before querying the local data store
			if (currentUserCredentials.UserName != loginInfo.Username.ToUpper() ||
                currentUserCredentials.Password != loginInfo.Password ||
                currentUserCredentials.Truck != loginInfo.Truck)
            {
                LoginCompleted(null, new LoginCompletedEventArgs(null, new Exception("100"), false, null));
                return;
			} 
				
            LoginResult = currentUserCredentials.LoginResult;

            //Success Login
            var loginCompleteArgs = new LoginCompletedEventArgs(new object[] { LoginResult }, null, false, null);

            ClientOnLoginCompleted(null, loginCompleteArgs);
        }

        public void UpdateTruckAsync(ILoginInfo loginInfo, string newTruck)
		{
            //serviceClientFactory.Instance.UpdateTruckCompleted += ClientOnUpdateTruckCompleted;
            //serviceClientFactory.Instance.UpdateTruckAsync(loginInfo.Username, loginInfo.Password, loginInfo.Truck, newTruck);

            restClientFactory.Instance.UpdateTruckAsync(loginInfo.Username, loginInfo.Password, loginInfo.Truck, newTruck);
            ClientOnUpdateTruckCompleted(null, new AsyncCompletedEventArgs(null, false, null));
        }

		public event EventHandler<LoginCompletedEventArgs> LoginCompleted = delegate { };

		public event EventHandler<AsyncCompletedEventArgs> UpdateTruckCompleted = delegate { };
        /// <summary>
        /// Get Logged User from local Db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
	    public LoginResult GetUserById(int id)
	    {
	        return repo.GetById(id);
	    }

	    public void SetIsLoggedIn(bool isLoggedIn)
	    {
	        IsLoggedIn = isLoggedIn;
	    }

	    public void GetCachedCredentials()
		{
			LoginResult = repo.GetById(1);
			if (LoginResult != null && !string.IsNullOrEmpty(LoginResult.ServerUrl))
			{
				serviceClientFactory.Url = LoginResult.ServerUrl;
                restClientFactory.Url = LoginResult.ServerUrl;
			}
			else
			{
				LoginResult = null;
			}
		}
	}
}

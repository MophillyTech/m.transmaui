using System;
using System.ComponentModel;
using m.transport.Models;
using m.transport.Svc;
using m.transport.ServiceInterface;

namespace m.transport.Data
{
	public interface ILoginRepository
	{
		bool IsLoggedIn { get; }
		bool FirstTimeLogin { get; set;}
		string Username { get; }
		string FullName { get; }
		string Password { get; }
		string Truck { get; }
		string AccountType { get; }
		string DriverType { get; }
		int OutsideCarrierInd { get; }
		int OutsideCarrierCompany { get; }
        int TruckSleeperInd { get; }
		LoginResult LoginResult { get; set; }
		void Clear();
		void Save();
        void Save(LoginResult loginResult);
	    void SetLoginResult(LoginResult loginResult);
		void LoginAsync(ILoginInfo loginInfo);
		void LoginWithExistingCredential(ILoginInfo loginInfo);
		event EventHandler<LoginCompletedEventArgs> LoginCompleted;
		void GetCachedCredentials();

		void UpdateTruckAsync (ILoginInfo loginInfo, string newTruck);
		event EventHandler<AsyncCompletedEventArgs> UpdateTruckCompleted;

        /// <summary>
        /// Get User By Id from Sql Lite LocalDb
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
	    LoginResult GetUserById(int id);

        /// <summary>
        /// Set value of IsLoggedIn private property
        /// </summary>
        /// <param name="isLoggedIn"></param>
	    void SetIsLoggedIn(bool isLoggedIn);
	}
}
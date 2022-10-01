namespace m.transport.Models
{
	public class LoginInfo : ILoginInfo
	{
		public LoginInfo()
		{
		}

		public LoginInfo(string username, string password, string truck)
		{
			Username = username;
			Password = password;
			Truck = truck;
		}

		public string Username { get; private set; }
		public string Password { get; private set; }
		public string Truck { get; private set; }
		public string AccountType
		{
			get { return (Username.ToUpper() == "ALDEMO") ? "DEMO" : "USER"; }
		}
	}
}
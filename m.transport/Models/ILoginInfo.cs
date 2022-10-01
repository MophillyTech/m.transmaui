namespace m.transport.Models
{
	public interface ILoginInfo
	{
		string Username { get; }
		string Password { get; }
		string Truck { get; }
		string AccountType { get; }
	}
}
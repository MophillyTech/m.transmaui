namespace m.transport.Interfaces
{
	public interface IBuildInfo
	{
		string Version { get; }
		string BuildNumber { get; }
		string BundleID { get; }
	}
}
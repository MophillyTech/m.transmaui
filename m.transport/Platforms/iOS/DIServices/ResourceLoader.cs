using m.transport.Interfaces;
using m.transport.iOS.DIServices;
using System.IO;
using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(ResourceLoader))]
namespace m.transport.iOS.DIServices
{
	public class ResourceLoader : ILoadResource
	{
		public string ResourcePrefix
		{
			get { return "m.transport.iOS.Resources."; }
		}
		public Stream LoadStream(string resourceName)
		{
			// note that the prefix includes the trailing period '.' that is required
			Assembly assembly = Assembly.GetExecutingAssembly();
			//var names = assembly.GetManifestResourceNames();
			return assembly.GetManifestResourceStream(ResourcePrefix + resourceName);
		}
		public byte[] LoadBytes(string resourceName)
		{
			using (var stream = LoadStream(resourceName))
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					return ms.ToArray();
				}
			}
		}
	}
}
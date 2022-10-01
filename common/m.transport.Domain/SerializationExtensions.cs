using System.IO;
using System.Runtime.Serialization;

namespace m.transport.Domain
{
	public static class SerializationExtensions
	{
		public static string Serialize<T>(this T obj)
		{
			string text;
			using (var memStream = new MemoryStream())
			{
				var dcSerializer = new DataContractSerializer(typeof(T));
				dcSerializer.WriteObject(memStream, obj);
				memStream.Flush();
				memStream.Position = 0;
				var streamReader = new StreamReader(memStream);
				text = streamReader.ReadToEnd();
			}
			return text;
		}
	}
}

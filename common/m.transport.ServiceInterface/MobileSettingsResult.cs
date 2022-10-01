using System.Runtime.Serialization;
using m.transport.Domain;

namespace m.transport.ServiceInterface
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class MobileSettingsResult
	{
		[DataMember(Order = 0)]
		public Setting[] Settings { get; set; }
	}
}
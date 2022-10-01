using System.Runtime.Serialization;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class TruckInfo
	{
		[DataMember]
		public string TruckOnFile { get; set; }
	}
}
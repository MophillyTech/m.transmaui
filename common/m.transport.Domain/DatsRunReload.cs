using System.Runtime.Serialization;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DatsRunReload
	{
		[DataMember]
		public int RunId { get; set; }
		[DataMember]
		public int LocationId { get; set; }
		[DataMember]
		public int LegsId { get; set; }
	}
}

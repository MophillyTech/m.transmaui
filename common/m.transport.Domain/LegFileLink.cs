using System.Runtime.Serialization;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class LegFileLink
	{
		[DataMember]
		public int LegsId { get; set; }
		[DataMember]
		public string FileName { get; set; }
	}
}

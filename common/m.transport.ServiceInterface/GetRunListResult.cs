using System.Runtime.Serialization;
using m.transport.Domain;

namespace DAI
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class GetRunListResult
	{
		[DataMember(Order=0)]
		public DatsRunSummary[] RunSummaries { get; set; }
		[DataMember(Order=1)]
		public DatsRunHistory[] RunLists { get; set; }
	}
}
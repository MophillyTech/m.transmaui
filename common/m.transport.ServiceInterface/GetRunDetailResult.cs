using System.Runtime.Serialization;
using m.transport.Domain;

namespace DAI
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class GetRunDetailResult
	{
		[DataMember(Order=0)]
		public DatsRunStop[] RunStops { get; set; }
	}
}
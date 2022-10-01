using System;
using System.Runtime.Serialization;

namespace m.transport.ServiceInterface
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class MileageResult
	{
		[DataMember]
		public int Mileage { get; set; }
		[DataMember]
		public DateTime MileageAsOfDate { get; set; }
	}
}

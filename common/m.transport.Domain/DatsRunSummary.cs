using System;
using System.Runtime.Serialization;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DatsRunSummary
    {
		[DataMember]
        public int? DriverId { get; set; }
		[DataMember]
		public string PayPeriod { get; set; }
		[DataMember]
		public string CurrOrPrev { get; set; }
		[DataMember]
		public int? NumberRuns { get; set; }
		[DataMember]
		public decimal? TotalPay { get; set; }
    }
}

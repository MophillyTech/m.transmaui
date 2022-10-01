using System.Runtime.Serialization;
using m.transport.Domain;

namespace m.transport.ServiceInterface
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class CurrentLoadUpdate
	{
		[DataMember]
		public DatsVehicleV4[] Vehicles { get; set; }
		[DataMember]
		public DatsRunStop[] RunStops { get; set; }
		[DataMember]
		public DatsRun[] Runs { get; set; }
		[DataMember]
		public DeliverySignature[] DeliverySignature { get; set; }
		[DataMember]
		public LegFileLink[] LegsFileLinks { get; set; }
		[DataMember]
		public DatsRunReload[] ReloadList { get; set; }
	}
}

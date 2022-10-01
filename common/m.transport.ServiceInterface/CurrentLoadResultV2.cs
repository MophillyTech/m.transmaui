using System.Runtime.Serialization;
using m.transport.Domain;

namespace m.transport.ServiceInterface
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class CurrentLoadResultV2
	{
		[DataMember]
		public DatsRun[] Runs { get; set; }
		[DataMember]
		public DatsRunStop[] RunStops { get; set; }

		[DataMember]
		public DatsVehicleV5[] Vehicles { get; set; }

		[DataMember(Order = 1)]
		public DatsLocation[] Locations { get; set; }
	}
}

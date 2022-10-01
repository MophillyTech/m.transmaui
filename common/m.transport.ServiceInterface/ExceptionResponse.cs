using System.Runtime.Serialization;
using m.transport.Domain;

namespace m.transport.ServiceInterface
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class ExceptionResponse
	{
		[DataMember]
		public DatsVehicleV5[] Vehicles { get; set; } 
	}
}
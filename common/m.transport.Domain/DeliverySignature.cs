using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DeliverySignature
	{
		[DataMember]
		public string FileName { get; set; }
		[DataMember]
		public byte[] SignatureData { get; set; }
		[DataMember(Order=1)]
		public int RunId { get; set; }
		[DataMember(Order = 2)]
		public int DropoffLocationId { get; set; }
		[DataMember(Order = 3)]
		public LegFileLink[] LegsFileLinks { get; set; }
	}
}

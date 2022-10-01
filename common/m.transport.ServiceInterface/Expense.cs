using System;
using System.Runtime.Serialization;

namespace m.transport.ServiceInterface
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
    public class Expense
    {
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public short Type { get; set; }
        [DataMember]
        public string Amount { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string TruckNum { get; set; }
    }

}

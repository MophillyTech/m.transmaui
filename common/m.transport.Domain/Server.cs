using System;
using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class Server : IHaveId<string>
	{

		[PrimaryKey]
		public string Id
		{
			get { return Code; }
			set { Code = value; }
		}

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}


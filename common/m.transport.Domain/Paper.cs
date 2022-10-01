using System.Runtime.Serialization;
using SQLite;
using System;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class Paper : IHaveId<string>
	{

		[PrimaryKey]
		public string Id
		{
			get { return Key; }
			set { Key = value; }
		}

		[DataMember]
		public string Key { get; set; }

		[DataMember]
		public DateTime Time { get; set; }

		[DataMember]
		public string Location { get; set; }

		[DataMember]
		public byte [] Data{ get; set; }

		[DataMember]
		public byte[] Offsets{ get; set; }

		public string Title
		{
			get
			{
				return Location + " :: " + Time.ToString("MM/dd/yyyy");
			}
		}
	}
}

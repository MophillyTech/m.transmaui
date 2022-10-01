using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class Setting : IHaveId<string>
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
		public string Value { get; set; }

		public override string ToString()
		{
			return Key + "=" + Value;
		}
	}
}

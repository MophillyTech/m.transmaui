using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DamageSeverityCode : IHaveId<string>
	{
		[PrimaryKey]
		public string Id
		{
			get { return Code; }
			set { Code = value; }
		}

		[MaxLength(255)]
		[DataMember]
		public string Code { get; set; }

		[MaxLength(255)]
		[DataMember]
		public string Description { get; set; }

		public override string ToString()
		{
			return Description;
		}
	}
}
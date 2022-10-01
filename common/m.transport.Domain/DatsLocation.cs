using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DatsLocation : IHaveId<int>
	{
		[DataMember]
		public int LocationId { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string AddressLine1 { get; set; }

		[DataMember]
		public string AddressLine2 { get; set; }

		[DataMember]
		public string City { get; set; }

		[DataMember]
		public string State { get; set; }

		[DataMember]
		public string Zip { get; set; }

		[DataMember]
		public string MainPhone { get; set; }

		[DataMember]
		public string LocationType { get; set; }

		[DataMember]
		public string LocationSubType { get; set; }

		[DataMember]
		public string LocationNotes { get; set; }

		[DataMember]
		public string DeliveryTimes { get; set; }

		[DataMember]
		public bool? NightDropAllowed { get; set; }
		[DataMember]
		public bool? STIAllowed { get; set; }

		[DataMember]
		public string DriverDirections { get; set; }

		[DataMember]
		public int? ShagPayAllowedInd { get; set; }

        [DataMember]
        public string LocationMessage { get; set; }


		[PrimaryKey]
		public int Id
		{
			get { return LocationId; }
			set { LocationId = value; }
		}

		public override string ToString()
		{
			return Name + "(" + LocationId + ")";
		}

		private bool show = true;

		public bool Show
		{
			get { return show; }
			set { show = value; }
		}

		public string DisplayName { get; set; }
	}
}

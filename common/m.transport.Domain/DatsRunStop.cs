//------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{

	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DatsRunStop : IHaveId<int>
	{
		[PrimaryKey]
		public int Id
		{
			get { return RunStopsId; }
			set { RunStopsId = value; }
		}

		[DataMember]
		public int RunStopsId { get; set; }
		[DataMember]
		public int? RunId { get; set; }
		[DataMember]
		public int? LocationId { get; set; }
		[DataMember]
		public string LocationName { get; set; }
		[DataMember]
		public string City { get; set; }
		[DataMember]
		public string State { get; set; }
		[DataMember]
		public string MainPhone { get; set; }
		[DataMember]
		public int? RunStopNumber { get; set; }
		[DataMember]
		public string StopType { get; set; }
		[DataMember]
		public int? UnitsLoaded { get; set; }
		[DataMember]
		public int? UnitsUnloaded { get; set; }
		[DataMember]
		public int? NumberOfReloads { get; set; }
		[DataMember]
		public DateTime? StopDate { get; set; }
		[DataMember]
		public decimal? Miles { get; set; }
		[DataMember]
		public decimal? AuctionPay { get; set; }
		[DataMember]
		public DateTime? CreationDate { get; set; }
		[DataMember]
		public string CreatedBy { get; set; }
		[DataMember]
		public DateTime? UpdatedDate { get; set; }
		[DataMember]
		public string UpdatedBy { get; set; }
		[DataMember]
		public int? OriginalCount { get; set; }
		[DataMember]
		public int? LoadedCount { get; set; }
		[DataMember]
		public int? DestinationCount { get; set; }
		[DataMember]
		public int? DeliveredCount { get; set; }
		[DataMember]
		public string AddressLine1 { get; set; }
		[DataMember]
		public string AddressLine2 { get; set; }
		[DataMember]
		public string ZipCode { get; set; }
		[DataMember]
		public string Notes { get; set; }
		[DataMember]
		public string Directions { get; set; }
		[DataMember]
		public bool? IsNightDropAllowed { get; set; }
		[DataMember]
		public bool? IsStiAllowed { get; set; }
		[DataMember]
		public string DeliveryTimes { get; set; }
		[IgnoreDataMember]
		public int? OriginalNumberOfReloads{ get; set; }

		public string CityStateAddress
		{
			get{
				return City + ", " + State;
			}

		}
	}
}

using System;
using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{

	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DatsRun : IHaveId<int>
	{
		[PrimaryKey]
		public int Id
		{
			get { return RunId; }
			set { RunId = value; }
		}

		[DataMember]
		public string UserCode { get; set; }
		[DataMember]
		public string TruckNumber { get; set; }
		[DataMember]
		public int RunId { get; set; }
		[DataMember]
		public int DriverId { get; set; }
		[DataMember]
		public int? TruckID { get; set; }
		[DataMember]
		public int? DriverRunNumber { get; set; }
		[DataMember]
		public int? RunPayPeriod { get; set; }
		[DataMember]
		public int? RunPayPeriodYear { get; set; }
		[DataMember]
		public DateTime? RunStartDate { get; set; }
		[DataMember]
		public DateTime? RunEndDate { get; set; }
		[DataMember]
		public int? StartedEmptyFromId { get; set; }
		[DataMember]
		public int? StartedLoadedFromId { get; set; }
		[DataMember]
		public int? UnitsOnTruck { get; set; }
		[DataMember]
		public int? MaxUnitsOnTruck { get; set; }
		[DataMember]
		public int? TotalStops { get; set; }
		[DataMember]
		public int? PaidInd { get; set; }
		[DataMember]
		public DateTime? PaidDate { get; set; }
		[DataMember]
		public string RunStatus { get; set; }
		[DataMember]
		public int IsOpen { get; set; }
		[DataMember]
		public int Status { get; set; }
		[DataMember]
		public DateTime? CreationDate { get; set; }
		[DataMember]
		public string CreatedBy { get; set; }
		[DataMember]
		public DateTime? UpdatedDate { get; set; }
		[DataMember]
		public string UpdatedBy { get; set; }
		[DataMember]
		public int DefaultLoadId { get; set; }
	}
}

using System;
using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
    [DataContract(Namespace = "http://www.mophilly.com/")]
	public class SystemInfo : IHaveId<int>
	{
		//TODO: This table didn't have a primary key so I added this. Need to ask Dan about this
		[PrimaryKey, AutoIncrement]
		[DataMember]
		public int Id { get; set; }

		[MaxLength(20)]
		[DataMember]
		public string AppVersionNo { get; set; }

		[MaxLength(255)]
		[DataMember]
		public string URLforWS { get; set; }

		[MaxLength(255)]
		[DataMember]
		public string CompanyLogoFile { get; set; }

		[MaxLength(128)]
		[DataMember]
		public string Licensee { get; set; }

		[MaxLength(20)]
		[DataMember]
		public string LisenseKey { get; set; }

		[MaxLength(32)]
		[DataMember]
		public string RegisteredUser { get; set; }

		[DataMember]
		public bool UpdateStatus { get; set; }

		[DataMember]
		public bool UpdateRequired { get; set; }

		[MaxLength(20)]
		[DataMember]
		public string UpdateVersionNo { get; set; }

		[DataMember]
		public DateTime LastSyncDate { get; set; }

		[DataMember]
		public int LastUserID { get; set; }

		[DataMember]
		public int LastDriverID { get; set; }

		[DataMember]
		public int LastTruckID { get; set; }

		[DataMember]
		public int TimeoutMinutes { get; set; }

		[DataMember]
		public int DefaultUserID { get; set; }

		[DataMember]
		public int DefaultDriverID { get; set; }

		[DataMember]
		public int DefaultTruckID { get; set; }

		[MaxLength(20)]
		[DataMember]
		public string DefaultDateFormat { get; set; }

		[MaxLength(32)]
		[DataMember]
		public string DefaultDateTimeFormat { get; set; }

		[DataMember]
		public string RegisteredPassword { get; set; }

		[DataMember]
		public string RegisteredTruck { get; set; }
	}
}

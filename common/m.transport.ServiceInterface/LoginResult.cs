using System;
using System.Runtime.Serialization;
using SQLite;
using m.transport.Domain;

namespace m.transport.ServiceInterface
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class LoginResult: IHaveId<int>
	{
		public LoginResult()
		{
			Id = 1;
		}

		public LoginResult(int currentDriverID)
		{
			Id = currentDriverID;
		}

		[PrimaryKey]
		public int Id { get; set; }

		[DataMember]
		public string Driver { get; set; }
		[DataMember]
		public string Password { get; set; }
		[DataMember]
		public string Truck { get; set; }
		[DataMember]
		public string FullName { get; set; }
		[DataMember]
		public bool? IsSignatureOnFile { get; set; }
		[DataMember]
		public DateTime? OdometerLastUpdateDate { get; set; }
		[DataMember]
		public int? LastOdometerValue { get; set; }

        [DataMember]
        public int? OutsideCarrierInd { get; set; }

        [DataMember]
        public int? SleeperCabInd { get; set; }

		[DataMember]
		public int? GPSTrackingInd { get; set; }

		[DataMember]
        public string DriverType { get; set; }

        [DataMember]
        public int? OutsideCarrierCompany { get; set; }

        [DataMember]
		[Ignore]
        public CompanyInfo CompanyInfo { get; set; }

		public string ServerUrl { get; set; }

        public string Setting { get; set; }

        public string CompanyInfoToString { get; set; }
	}
}

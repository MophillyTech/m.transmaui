using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using SQLite;
using System.Linq;
using m.transport.Utilities;


namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DamagePhoto : IHaveId<string>
	{
		[DataMember]
		public string FileName { get; set; }
		[DataMember]
		public byte[] ImageData { get; set; }
		[DataMember(Order = 1)]
		public int RunId { get; set; }
		[DataMember(Order = 2)]
		public int LocationId { get; set; }
		[DataMember(Order = 3)]
		public int VehicleID { get; set; }
		[DataMember(Order = 4)]
		public int VehicleDamageDetailID { get; set; }
		[DataMember(Order = 5)]
		public string DamageCode { get; set; }
		[DataMember(Order = 6)]
		public int Sequence { get; set; }
		[DataMember(Order = 7)]
		public int InspectionType { get; set; }
		[DataMember(Order = 8)]
		public string NoPhotoReasonCode { get; set; }
		[DataMember(Order = 9)]
		public int Order { get; set; }
		[DataMember(Order = 10)]
		public string VIN { get; set; }

		[PrimaryKey]
		public string Id
		{
			get { return VehicleID + "|" + VIN + "|" + DamageCode + "|" + Sequence + "|" + Order; }
			set
			{
				var parts = value.Split('|');
				if (!string.IsNullOrWhiteSpace(parts[0]))
				{
					VehicleID = Int32.Parse(parts[0]);
				}
				if (!string.IsNullOrWhiteSpace(parts[1]))
				{
					VIN = parts[1];
				}

				if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2]))
				{
					DamageCode = parts[2];
				}
				if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
				{
					Sequence = Convert.ToInt32(parts[3]);
				}
				if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
				{
					Order = Convert.ToInt32(parts[4]);
				}
			}
		}
	}
}

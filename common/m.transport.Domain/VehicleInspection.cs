using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
    [DataContract(Namespace = "http://www.mophilly.com/")]
    public class VehicleInspection : IHaveId<int>
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("VehicleInspectionID")]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int VehicleID { get; set; }
        [DataMember]
        public int InspectionType { get; set; }
        [DataMember]
        public DateTime? InspectionDate { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string InspectedBy { get; set; }
        [DataMember]
        public int DamageCodeCount { get; set; }
        [DataMember]
        public bool? AttendedInd { get; set; }
        [DataMember]
        public bool? SubjectToInspectionInd { get; set; }
        [DataMember]
        public bool? CleanVehicleInd { get; set; }

        [MaxLength(1000)]
        [DataMember]
        public string Notes { get; set; }
        [DataMember]
        public DateTime? CreationDate { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public DateTime? UpdatedDate { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string UpdatedBy { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string DeliveryInitials { get; set; }

        [DataMember]
        public List<string> DamageCodes { get; set; }

        [DataMember]
        public int? LegsID { get; set; }
    }
}
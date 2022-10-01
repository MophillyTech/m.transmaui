using System;
using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
    [DataContract(Namespace = "http://www.mophilly.com/")]
	public class Expense : IHaveId<int>
	{

		[PrimaryKey]
		[AutoIncrement]
		[Column("ExpenseID")]
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int DriverID { get; set; }
		[DataMember]
		public DateTime ItemDate { get; set; }
		[DataMember]
		public short Type { get; set; }

		[DataMember]
		public decimal Amount { get; set; }
        
        [MaxLength(50)]
		[DataMember]
		public string ItemDescription { get; set; }
        
        [MaxLength(20)]
		[DataMember]
		public string TruckNum { get; set; }

		[DataMember]
		public int TruckID { get; set; }
		[DataMember]
		public int? BackupReceivedInd { get; set; }
		[DataMember]
		public bool PaidInd { get; set; }
		[DataMember]
		public DateTime PaidDate { get; set; }
		[DataMember]
		public int ExportBatchID { get; set; }
        
        [MaxLength(20)]
		[DataMember]
		public string ExportedBy { get; set; }
		[DataMember]
		public DateTime CreationDate { get; set; }

		[MaxLength(20)]
		[DataMember]
		public string CreatedBy { get; set; }

		[DataMember]
		public DateTime UpdatedDate { get; set; }

		[MaxLength(20)]
		[DataMember]
		public string UpdatedBy { get; set; }

		[DataMember]
		public bool Local { get; set; }

		public string ItemDateToString { get; set; }

	}
}
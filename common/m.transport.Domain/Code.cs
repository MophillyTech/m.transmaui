using System.Runtime.Serialization;
using SQLite;

namespace m.transport.Domain
{
    [DataContract(Namespace = "http://www.mophilly.com/")]
    public class Code : IHaveId<int>
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("CodeID")]
        [DataMember]
        public int Id { get; set; }

        [MaxLength(30)]
        [DataMember]
        public string CodeType { get; set; }

        [MaxLength(255)]
        [DataMember]
        public string CodeName { get; set; }

        [MaxLength(255)]
        [DataMember]
        public string CodeDescription { get; set; }

        [MaxLength(255)]
        [DataMember]
        public string Value1 { get; set; }
        
        [MaxLength(255)]
        [DataMember]
        public string Value1Description { get; set; }

        [MaxLength(255)]
        [DataMember]
        public string Value2 { get; set; }

        [MaxLength(255)]
        [DataMember]
        public string Value2Description { get; set; }

        [MaxLength(15)]
        [DataMember]
        public string RecordStatus { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        public override string ToString()
        {
            return CodeDescription;
        }
    }
}
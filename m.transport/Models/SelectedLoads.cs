using m.transport.Domain;
using SQLite.Net.Attributes;

namespace m.transport.Models
{
	public class SelectedLoads : IHaveId<int>
	{
		public SelectedLoads()
		{
			Id = 1;
		}

		[PrimaryKey]
		public int Id { get; set; }

		[MaxLength(255)]
		public string LoadNumbers { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m.transport.Domain;
using SQLite.Net.Attributes;

namespace m.transport.Models
{
	public class SelectedVehicles : IHaveId<int>
	{
		public SelectedVehicles()
		{
			Id = 1;
		}

		[PrimaryKey]
		public int Id { get; set; }
	
		[MaxLength(255)]
		public string VehicleIds { get; set; }
	}
}

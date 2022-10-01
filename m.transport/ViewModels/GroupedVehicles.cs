using System.Collections;
using System.Collections.Generic;
using System.Text;
using m.transport.Domain;
using System.Linq;

namespace m.transport.ViewModels
{

	public class GroupedVehicles : IEnumerable
	{
		public DatsLocation Location { get; set; }

		//changing display name in case two location has same name
		public string LocationName { get; set; }

		public CustomObservableCollection<VehicleViewModel> Vehicles { get; set; }

		public bool IsDeliveryInProgress { get; set; }

		public IEnumerator GetEnumerator()
		{
			return Vehicles.GetEnumerator();
		}

		public GroupedVehicles()
		{
			Vehicles = new CustomObservableCollection<VehicleViewModel>();
		}

		public int VehiclesCount
		{
			get{
				return Vehicles.Count;
			}
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.AppendLine("Location: " + Location);
			builder.AppendLine("Vehicles:");
			foreach (var vehicle in Vehicles)
			{
				builder.AppendLine("\t"+vehicle.DatsVehicle.ToString());
			}
			return builder.ToString();
		}
	}
}

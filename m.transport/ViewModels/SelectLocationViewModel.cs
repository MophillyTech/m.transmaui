using System;
using m.transport.ViewModels;
using System.Collections.Generic;
using m.transport.Domain;
using System.Linq;

namespace m.transport
{
	public class SelectLocationViewModel : BaseViewModel
	{

		public CustomObservableCollection<GroupedVehicles> GroupedVehicles{ get; set; }

		public SelectLocationViewModel (CustomObservableCollection<GroupedVehicles> groupedVehicles)
		{

			GroupedVehicles = groupedVehicles;

		}

		public List<DatsLocation> Locations
		{
			get
			{
				List<DatsLocation> list = new List<DatsLocation> ();

				foreach (GroupedVehicles v in GroupedVehicles) {
				
					DatsLocation loc = v.Location;

					if (loc == null || loc.Name == null) {
						list.Add (new DatsLocation {
							Name = "Unexpected Pickup Location"
						});

					} else {
						list.Add (v.Location);
					}
				}

				if (list.Count > 1) {
					list.Add(new DatsLocation{
						Name = "All"
					});
				}

				return list;
			}
		}
	}
}


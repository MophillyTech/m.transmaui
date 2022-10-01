using System;
using System.Collections.Generic;
using System.Collections;

namespace m.transport.ViewModels
{
	public class DealerLoadViewModel : BaseViewModel, IEnumerable
	{
		public DealerLoadViewModel()
		{
			Vehicles = new List<VehicleViewModel> ();
		}

		//public Dealer Dealer { get; set; }
		public List<VehicleViewModel> Vehicles { get; set; }
		public DateTime DeliveryDue { get; set; }

		public IEnumerator GetEnumerator() {
			return Vehicles.GetEnumerator ();
		}
	}
}


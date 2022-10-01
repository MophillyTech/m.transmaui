using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m.transport.Domain;
using Autofac;
using m.transport.Data;

namespace m.transport.ViewModels
{
	public class SelectVehicleViewModel : LoadViewModel
	{
		List<string> status = new List<string> { "Available", "Assigned", "Loading" };
		public CustomObservableCollection<VehicleViewModel> VehicleList { get; set; }
		public InspectionType Type { get; set; }
		private int locationID;
		public SelectVehicleViewModel(int locID, InspectionType inspectionType)
			: base(App.Container.Resolve<ICurrentLoadRepository>())
		{
			locationID = locID;
			Type = inspectionType;
			VehicleList = new CustomObservableCollection<VehicleViewModel>();
			ListVehicle ();
		}

		private void ListVehicle()
		{
			VehicleList.Clear();
			List<VehicleViewModel> model;
			if (locationID > 0) {
				model = (from veh in Vehicles
					where status.Contains (veh.DatsVehicle.VehicleStatus) && veh.DatsVehicle.PickupLocationId == locationID
					select veh).ToList ();
			} else {
				model = (from veh in Vehicles
					where status.Contains (veh.DatsVehicle.VehicleStatus)
					select veh).ToList ();
			}

			foreach (VehicleViewModel v in model)
			{
				VehicleList.Add(v);
			}

		}

		public void RefreshVehicleList()
		{
			ListVehicle();
			foreach (VehicleViewModel v in VehicleList)
			{
				VehicleList.ReportItemChange(v);
			}
		}
	}
}

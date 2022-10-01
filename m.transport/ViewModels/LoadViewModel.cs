using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using m.transport.Data;
using m.transport.Domain;
using System.ComponentModel;
using System.Threading.Tasks;

namespace m.transport.ViewModels
{
	public class LoadViewModel : BaseViewModel
	{
		protected readonly ICurrentLoadRepository loadRepo;
		private bool isClosureOn = true;
		protected LoadManager manager = LoadManager.Instance;

		public LoadViewModel(ICurrentLoadRepository loadRepo)
		{
			this.loadRepo = loadRepo;
		}

		public void RefreshVehicles()
		{
			manager.OnSelectedLoadChanged ();

		}
			
		public List<VehicleViewModel> Vehicles
		{
			get
			{
				return manager.Vehicles;
			}
		}

		public ObservableCollection<DatsLocation> Locations
		{
			get
			{
				return manager.Locations;
			}
		}
			
		public bool ProcessPickupExceptionVehicle(VehicleViewModel v)
		{
            bool hasException = false;

			DatsVehicleV5 veh = null;
			int loadId = 0;

			if(loadRepo.SelectedLoad != null && loadRepo.SelectedLoad.Vehicles != null)
            {
				veh = loadRepo.SelectedLoad.Vehicles.FirstOrDefault(x => x.VIN == v.VIN);
				var tempVeh = loadRepo.SelectedLoad.Vehicles.FirstOrDefault(x => x.LoadId > 0);
				if (tempVeh != null)
                {
					loadId = tempVeh.LoadId;

				}
			}


			//if the selected load doesn't contain the VIN add it in
			if (veh == null) {
				veh = v.DatsVehicle;
				veh.ProcessInd = 3;
				veh.SetVehicleStatus (VehicleStatus.Loading);
				if (veh.LoadId == 0)
                {
                    veh.LoadId = loadId;
                }
                hasException = true;
                AddVehicle(veh);
			}

			if (veh.ExceptionFlag == 2 && veh.ExceptionCode == 12) {
				veh.SetVehicleStatus (VehicleStatus.Removing);
                hasException = true;
                //AddVehicle(veh);
			}

            if (veh.ExceptionFlag == 1) {
                //AddVehicle(veh);
                hasException = true;
            }


			//if (addVehicleToSelectedLoad) {
			//	AddVehicle (veh);
   //             hasException = true;
			//}

            return hasException;

			//if the selected load doesn't contain the VIN add it in
//			if (veh == null) {
//				veh = v.DatsVehicle;
//				addVehicleToSelectedLoad = true;
//			}
//
//			veh.ProcessInd = 3;
//			veh.ExceptionInd = veh.ExceptionFlag;
//
//			if (veh.ExceptionFlag == 2 && veh.ExceptionCode != 12) {
//				veh.SetVehicleStatus (VehicleStatus.Loading);
//			}
//				
//			if (addVehicleToSelectedLoad) {
//				AddVehicle (veh);
//			}
		}

		public void ProcessDeliveryExceptionVehicle(VehicleViewModel v)
		{
			DatsVehicleV5 veh = loadRepo.SelectedLoad.Vehicles.FirstOrDefault (x => x.VIN == v.VIN);
			bool addVehicleToSelectedLoad = false;

			//if the selected load doesn't contain the VIN add it in
			if (veh == null) {
				veh = v.DatsVehicle;
				addVehicleToSelectedLoad = true;
			}

			veh.ProcessInd = 0;
			veh.ExceptionInd = veh.ExceptionFlag;
			if (veh.ExceptionFlag == 2) {
				veh.SetVehicleStatus (VehicleStatus.Delivering);
			} else {
				veh.SetVehicleStatus (VehicleStatus.Loaded);
				loadRepo.RemoveDamagePhotos (veh.VehicleId, veh.VIN);
			}
				
			if (addVehicleToSelectedLoad) {
				AddVehicle (veh);
			}
		}


		public void AddVehicle(DatsVehicleV5 v)
		{
			// not assigned a run yet
			v.RunId = 0;
			v.VinKey = v.VIN;
			v.LoadNumber = "0";

//			if (v.ExceptionCode == 0)
//			{
//				v.ExceptionCode = 1;
//			}


			// need to do this in order for loadRepo to update
			// is there a (significant) performance issue to doing this vs Array.Resize?
			List<DatsVehicleV5> dv;

			if (this.loadRepo.SelectedLoad.Vehicles != null)
			{
				dv = this.loadRepo.SelectedLoad.Vehicles.ToList();
			}
			else
			{
				dv = new List<DatsVehicleV5>();
			}

			dv.Add(v);
			this.loadRepo.SelectedLoad.Vehicles = dv.ToArray();

			manager.OnSelectedLoadChanged ();

		}
			
		public ObservableCollection<DatsVehicleV5> SelectedVehicle()
		{
			return loadRepo.InitializeObservableCollection(r => r.SelectedLoad, r => r.SelectedLoad.Vehicles);
		}

	}
}


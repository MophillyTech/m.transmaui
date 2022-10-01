using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using m.transport.Data;
using m.transport.Domain;
using System.Linq;
using System.ComponentModel;
using Autofac;

namespace m.transport.ViewModels
{
	public class ManageLoadViewModel : LoadViewModel
	{
		public string Mode { get; set; }
		public DeliveryInfo DeliveryInfo { get; set; }

		public CustomObservableCollection<GroupedVehicles> VehiclesGrouped { get; set; }
		public List<VehicleViewModel> SelectedVehicles;
		public int SelectedLocationID { get; set; }
		private InspectionType type;

		public ManageLoadViewModel(int locID, InspectionType type)
			: base(App.Container.Resolve<ICurrentLoadRepository>())
		{
			this.DeliveryInfo = new DeliveryInfo();
			this.type = type;
			SelectedLocationID = locID;
			SelectedVehicles = Vehicles;

			VehiclesGrouped = new CustomObservableCollection<GroupedVehicles>();
			CreateList();
		}

		public void RefreshList()
		{
			foreach (GroupedVehicles v in VehiclesGrouped)
			{
				VehiclesGrouped.ReportItemChange(v);
			}

		}

		public void CreateList()
		{
			if (type == InspectionType.Loading) {				
                ProcessPickupVehicles();
			} else if (type == InspectionType.Delivery) {
				ProcessDeliveryVehicles();
			}

			foreach (GroupedVehicles v in VehiclesGrouped)
			{
				VehiclesGrouped.ReportItemChange(v);
			}
		}

		public void ProcessDeliveryVehicles(){
			VehiclesGrouped.Clear();
			int locationID = -2;
			GroupedVehicles gv = null;

			if (Vehicles != null)
			{
				foreach (var v in Vehicles.OrderBy(v => v.DatsVehicle.DropoffLocationId))
				{
					string[] statuses = new []{ "Loaded" };


					if (statuses.Contains(v.DatsVehicle.VehicleStatus))
					{
						v.InspectionType = InspectionType.Delivery;


						if (locationID != v.DatsVehicle.DropoffLocationId) {

							DatsLocation loc = Locations.FirstOrDefault (l => l.Id == v.DatsVehicle.DropoffLocationId);
							locationID = v.DatsVehicle.DropoffLocationId;
							gv = new GroupedVehicles ();

							//modified it so that location disclosure can be hidden
							if (loc == null) {
								loc = new DatsLocation {
									Show = false, Name = "Unknown", DisplayName = "Unknown"
								};
							} else {
								//for UI display purpose when there are two of the same location name
								if (loc.DisplayName == null) {
									if (Locations.Where (l => l.Name == v.DatsVehicle.DropoffLocationName).Distinct ().Count () > 1)
										loc.DisplayName = loc.Name + " (" + v.DatsVehicle.DropoffLocationId + ")";
									else
										loc.DisplayName = loc.Name;
								}
							}

							gv.Location = loc;

							VehiclesGrouped.Add (gv);
						}

						if (statuses.Contains (v.DatsVehicle.VehicleStatus) && gv != null) {
							gv.Vehicles.Add(v);
						}

					}
				}
			}

			if (SelectedLocationID >= 0)
			{
				var found = VehiclesGrouped.FirstOrDefault(v => v.Location != null && v.Location.LocationId == SelectedLocationID);

				if (found != null)
				{
					var ndx = VehiclesGrouped.IndexOf(found);

					if (ndx > 0)
					{
						VehiclesGrouped.Remove(found);
						VehiclesGrouped.Insert(0, found);
					}
				}
			}

			foreach (GroupedVehicles g in VehiclesGrouped)
			{
				VehicleViewModel tmp = g.Vehicles.FirstOrDefault(l => (l.DatsVehicle.VehicleStatus == "Loaded" ||
					l.DatsVehicle.VehicleStatus == "Delivering"));
				if (tmp != null)
					g.IsDeliveryInProgress = true;
				else
					g.IsDeliveryInProgress = false;
			}
		}

		public void ProcessPickupVehicles(){
			VehiclesGrouped.Clear();
			int locationID = -2;
			GroupedVehicles gv = null;


			string[] statuses= new []{ "Assigned", "Loaded", "Loading" };

			if (Vehicles != null)
			{
				foreach (var v in Vehicles.OrderBy(v => v.DatsVehicle.DropoffLocationId))
				{
					if (SelectedLocationID > 0 && v.DatsVehicle.PickupLocationId != SelectedLocationID)
						continue;

					//StatusType == 0 check is for delivery tab in currentload so that all destination prints
					if (statuses.Contains(v.DatsVehicle.VehicleStatus))
					{
						v.InspectionType = InspectionType.Loading;


						if (locationID != v.DatsVehicle.DropoffLocationId) {

							DatsLocation loc = Locations.FirstOrDefault (l => l.Id == v.DatsVehicle.DropoffLocationId);
							locationID = v.DatsVehicle.DropoffLocationId;
							gv = new GroupedVehicles ();

							//modified it so that location disclosure can be hidden
							if (loc == null) {
								loc = new DatsLocation {
									Show = false, Name = "Unknown", DisplayName = "Unknown"
								};
							} else {
								//for UI display purpose when there are two of the same location name
								if (loc.DisplayName == null) {
									if (Locations.Where (l => l.Name == v.DatsVehicle.DropoffLocationName).Distinct ().Count () > 1)
										loc.DisplayName = loc.Name + " (" + v.DatsVehicle.DropoffLocationId + ")";
									else
										loc.DisplayName = loc.Name;
								}
							}

							gv.Location = loc;

							VehiclesGrouped.Add (gv);
						}

						if (statuses.Contains (v.DatsVehicle.VehicleStatus) && gv != null) {
							gv.Vehicles.Add(v);
						}

					}
				}
			}
				
			foreach (GroupedVehicles g in VehiclesGrouped)
			{
				VehicleViewModel tmp = g.Vehicles.FirstOrDefault(l => (l.DatsVehicle.VehicleStatus == "Loaded" ||
					l.DatsVehicle.VehicleStatus == "Delivering"));
				if (tmp != null)
					g.IsDeliveryInProgress = true;
				else
					g.IsDeliveryInProgress = false;
			}
		}

		public void ProcessDeliverySelection()
		{
			foreach (GroupedVehicles x in VehiclesGrouped) {
				foreach (var v in x.Vehicles) {
					if (v.DatsVehicle.DropoffLocationId == SelectedLocationID) {
						v.Selected = true;
					} else {
						v.Selected = false;
					}
				}
			}
		}
						
		public void RemoveDamagePhotos(int VehicleID, string VIN)
		{
			loadRepo.RemoveDamagePhotos (VehicleID, VIN);
		}
	}

}


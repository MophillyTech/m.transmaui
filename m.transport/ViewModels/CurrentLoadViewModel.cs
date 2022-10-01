using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using m.transport.Data;
using m.transport.Interfaces;
using m.transport.Svc;
using m.transport.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using m.transport.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.ViewModels
{
	public class CurrentLoadViewModel : LoadViewModel
	{
		protected readonly ILoginRepository loginRepo;

		public string Mode { get; set; }
		public event EventHandler<AsyncCompletedEventArgs> SendCurrentLoadCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UpdateOdometerCompleted = delegate { };
		public event EventHandler<GetCurrentLoadCompletedEventArgs> GetCurrentLoadCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UploadCurrentLoadCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UploadDamagePhotosCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> SubmitDriverSignatureCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UploadLocationCompleted = delegate { };

		public CustomObservableCollection<GroupedVehicles> VehiclesByPickup { get; set; }
		public CustomObservableCollection<GroupedVehicles> VehiclesByDropoff { get; set; }

		private bool hasException;
		private bool hasShownGPSRequirement = false;


		public CurrentLoadViewModel(ICurrentLoadRepository repo, ILoginRepository loginRepo)
			: base(repo)
		{
			this.loginRepo = loginRepo;

			Mode = "Load";

			//VehiclesGroupedByDropoff = new CustomObservableCollection<GroupedVehicles>();
			//VehiclesGroupedByPickup = new CustomObservableCollection<GroupedVehicles>(); 
		}

		public int? Mileage
		{
			get { return loginRepo.LoginResult.LastOdometerValue; }
		}

		public bool HasSelectedLoadVehicles
		{
			get { return loadRepo.SelectedLoad != null && loadRepo.SelectedLoad.Vehicles != null && loadRepo.SelectedLoad.Vehicles.Any(); }
		}

		public void SendCurrentLoadAsync(UploadStatus stats)
		{
			loadRepo.UploadCurrentLoadCompleted += LoadRepoOnUploadCurrentLoadCompleted;
			loadRepo.UploadCurrentLoadAsync(stats);
		}

		public bool HasDamagePhotos(){

			return loadRepo.HasDamagePhoto ();
		}

		public async Task<bool> HasOfflineDelivery()
		{
			return loadRepo.OfflineDelivery ();

		}

		public bool IsDirty(int index = -1){
			
			bool dirty = false;

			if (Vehicles != null) {
				switch (index)
				{
				case 0:
					if (Vehicles.Count (v => v.DatsVehicle.VehicleStatus == "Loading" || v.DatsVehicle.VehicleStatus == "Removing") > 0) {
						dirty = true;
					}
					break;
				case 1:
					if (loadRepo.HasOfflineDelivery ()) {
						dirty = true;
					}
					break;
				default:
					if (Vehicles.Count (v => v.DatsVehicle.VehicleStatus == "Delivering" || v.DatsVehicle.VehicleStatus == "Loading") > 0) {
						dirty = true;
					}
					break;
				}
			}
			return dirty;
		}

		public bool IsDirtyWithDelivery()
		{
			if (Vehicles.Count (v => v.DatsVehicle.VehicleStatus == VehicleStatus.Delivering.ToString()) > 0) {
				return true;
			}

			return false;
		}


		private void LoadRepoOnUploadCurrentLoadCompleted(object sender, AsyncCompletedEventArgs args)
		{
			loadRepo.UploadCurrentLoadCompleted -= LoadRepoOnUploadCurrentLoadCompleted;
			UploadCurrentLoadCompleted(sender, args);

		}

		public void GetCurrentLoadAysnc(bool refresh, bool isDelivery)
		{
			loadRepo.GetCurrentLoadCompleted += LoadRepoOnGetCurrentLoadCompleted;
			loadRepo.GetCurrentLoadAsync (refresh, isDelivery);
		}

		private void LoadRepoOnGetCurrentLoadCompleted(object sender, GetCurrentLoadCompletedEventArgs args)
		{
			loadRepo.GetCurrentLoadCompleted -= LoadRepoOnGetCurrentLoadCompleted;
			GetCurrentLoadCompleted(sender, args);
		}
			
		public List<VehicleViewModel> Exceptions
		{
			get
			{
				return Vehicles.Where(v => (v.DatsVehicle.ExceptionCode > 0)).ToList();
			}
		}

		public bool VehiclesInTransitionState
		{
			get
			{
				return 
					(from v in Vehicles
					 where v.DatsVehicle.VehicleStatus != "Assigned" || v.DatsVehicle.VehicleStatus == "Delivered"
					 select v).Distinct().Any();
			}
		}

		public int? UpdatedOdometerReading {
			get { return loadRepo.UpdatedMileage; }
			set { loadRepo.UpdatedMileage = value; }
		}

		public void UploadDamagePhotos(){
			loadRepo.UploadDamagePhotosComplete += OnUploadPhotosCompleted;
			loadRepo.UploadDamagePhotos (null);
		}

		private void OnUploadPhotosCompleted(object sender, AsyncCompletedEventArgs args)
		{
			UploadDamagePhotosCompleted(sender, args);
		}

		public bool HasExceptions
		{

			get { return hasException; }
			set { hasException = value; }
		}

		public void SetDropoffLocation(int locID)
		{
			loadRepo.SetDropoffLocation(locID);
		}

		public void UploadDriverSignature()
		{
			loadRepo.UploadCurrentLoadCompleted += OnUploadDriverSignatureCompleted;
			loadRepo.UploadCurrentLoadAsync(UploadStatus.DriverSignature);
		}

		private void OnUploadDriverSignatureCompleted(object sender, AsyncCompletedEventArgs args)
		{
			loadRepo.UploadCurrentLoadCompleted -= OnUploadDriverSignatureCompleted;
			SubmitDriverSignatureCompleted (sender, args);
		}

		//public void RefreshCurrentLoad()
		//{
		//	foreach (GroupedVehicles g in VehiclesGroupedByPickup)
		//	{
		//		VehiclesGroupedByPickup.ReportItemChange(g);
		//	}

		//	foreach (GroupedVehicles g in VehiclesGroupedByDropoff)
		//	{
		//		VehiclesGroupedByDropoff.ReportItemChange(g);
		//	}
		//}

		//public void ReLoadCurrentLoad()
		//{
		//	GroupByPickup();
		//	GroupByDropoff(-1, true, 0, true);

		//	RefreshCurrentLoad();
		//}

		public Task<CustomObservableCollection<GroupedVehicles>> InitPickupList()
		{
			return Task.Run(() =>
			{
				return GroupByPickup();
			});
		}


		public Task<CustomObservableCollection<GroupedVehicles>> InitDropOffList()
		{
			return Task.Run(() =>
			{
				return GroupByDropoff(-1, true, 0, true);
			});
		}

		public CustomObservableCollection<GroupedVehicles> GroupByPickup()
		{
			CustomObservableCollection<GroupedVehicles> VehiclesGroupedByPickup = new CustomObservableCollection<GroupedVehicles>();
			int locationID = -1;
			GroupedVehicles gv = null;

			if (Vehicles != null)
			{
				foreach (var v in Vehicles.OrderBy(v => v.DatsVehicle.PickupLocationId))
				{
                    v.CheckDamage(false);

					if (string.IsNullOrEmpty(v.DatsVehicle.PickupLocationName)) v.DatsVehicle.PickupLocationName = "Unknown";

					string[] statuses = { "Loaded", "Loading", "Assigned" };

					if (statuses.Contains(v.DatsVehicle.VehicleStatus))
					{

						v.InspectionType = InspectionType.Loading;

						if (locationID != v.DatsVehicle.PickupLocationId)
						{
							locationID = v.DatsVehicle.PickupLocationId;
							DatsLocation loc = Locations.FirstOrDefault (l => l.Id == v.DatsVehicle.PickupLocationId);

							gv = new GroupedVehicles();
							if (loc == null) {
								loc = new DatsLocation {
									Show = false, Name = "Unknown", DisplayName = "Unknown", LocationId = locationID
								};
							} else {
								//for UI display purpose when there are two of the same location name
								if (loc.DisplayName == null) {
									if(Locations.Where(l => l.Name == v.DatsVehicle.PickupLocationName).Distinct().Count() > 1)
										loc.DisplayName = loc.Name  + " (" + v.DatsVehicle.PickupLocationId + ")";
									else
										loc.DisplayName = loc.Name;
								}
							}

							gv.Location = loc;
							VehiclesGroupedByPickup.Add(gv);
						}
						gv.Vehicles.Add(v);
					}

				}
			}

            VehiclesByPickup = VehiclesGroupedByPickup;

            return VehiclesGroupedByPickup;
		}

		public CustomObservableCollection<GroupedVehicles> GroupByDropoff(int selectedLocID = -1, bool Delivery = false, int StatusType = 0, bool unschedule = false)
		{
            CustomObservableCollection<GroupedVehicles> VehiclesGroupedByDropoff = new CustomObservableCollection<GroupedVehicles>();
			//VehiclesGroupedByDropoff.Clear();
			int locationID = -2;
			GroupedVehicles gv = null;

			if (Vehicles != null)
			{
				foreach (var v in Vehicles.OrderBy(v => v.DatsVehicle.DropoffLocationId))
				{
					string[] statuses;

					switch (StatusType) 
					{
					//"Delivered", 
					default:
					case 0:
						statuses = new []{"Loaded", "Delivering"};
						break;
						//"Pickup"
					case 1:
						statuses= new []{ "Assigned", "Loaded", "Loading" };
						break;
						//special case for manage delivery where only show Loaded
					case 2:
						statuses= new []{ "Loaded" };
						break;
					}

					//StatusType == 0 check is for delivery tab in currentload so that all destination prints
					if (StatusType == 0 || !Delivery || statuses.Contains(v.DatsVehicle.VehicleStatus))
					{
						v.InspectionType = Delivery ? InspectionType.Delivery : InspectionType.Loading;


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

							VehiclesGroupedByDropoff.Add (gv);
						}

						if (statuses.Contains (v.DatsVehicle.VehicleStatus) && gv != null) {
							gv.Vehicles.Add(v);
						}

					}
				}
			}

			if (selectedLocID >= 0)
			{
				var found = VehiclesGroupedByDropoff.FirstOrDefault(v => v.Location != null && v.Location.LocationId == selectedLocID);

				if (found != null)
				{
					var ndx = VehiclesGroupedByDropoff.IndexOf(found);

					if (ndx > 0)
					{
						VehiclesGroupedByDropoff.Remove(found);
						VehiclesGroupedByDropoff.Insert(0, found);
					}
				}
			}

			foreach (GroupedVehicles g in VehiclesGroupedByDropoff)
			{
				VehicleViewModel tmp = g.Vehicles.FirstOrDefault(l => (l.DatsVehicle.VehicleStatus == "Loaded" ||
					l.DatsVehicle.VehicleStatus == "Delivering"));
				if (tmp != null)
					g.IsDeliveryInProgress = true;
				else
					g.IsDeliveryInProgress = false;
			}

            GroupedVehicles unschedulegroup = new GroupedVehicles
            {
                //modified it so that location disclosure can be hidden
                Location = new DatsLocation
                {

                    Name = "Unscheduled Location",
                    DisplayName = "Unscheduled Location",
                    Show = false
                }
            };

            VehiclesGroupedByDropoff.Add(unschedulegroup);

            VehiclesByDropoff = VehiclesGroupedByDropoff;

            return VehiclesGroupedByDropoff;
		}
	}
}


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Models;
using m.transport.Svc;
using m.transport.ServiceInterface;
using m.transport.Utilities;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.Data
{
	public class CurrentLoadRepository : ICurrentLoadRepository
	{
		private readonly ILoginRepository loginRepository;
		private readonly IDriverSignatureRepository driverSignatureRepository;
		private readonly IRepository<DatsRun, int> runRepository;
		private readonly IRepository<DatsRunStop, int> runStopRepository;
		private readonly IRepository<DatsVehicleV5, string> vehicleRepository;
		private readonly IRepository<DatsLocation, int> locationRepository;
		private readonly IRepository<SelectedLoads, int> selectedLoadRepository;
		private readonly IServiceClientFactory<ITransportServiceClient> serviceClientFactory;
        private readonly IServiceClientFactory<IRestServiceClient> restClientFactory;
        private readonly IRepository<DamagePhoto,string> damagePhotoRepository;

		private Signature customerSignature;
		private int dropoffLocationId;
		private List<DamagePhoto> damagePhotoList;
		private DamagePhoto currPhoto;
		private AsyncCompletedEventArgs sendDamageEvent;
		private Queue<DeliveryLoad> deliveryQueue = new Queue<DeliveryLoad> ();
		private Queue<DeliverySignature> driverSginatureQueue = new Queue<DeliverySignature> ();
		private DeliveryLoad load;

		public CurrentLoadResultV2 CurrentLoad { get; set; }
		public bool DisplayOdometerPrompt { get; set; }
		public Load[] Loads { get; set; }
		public CurrentLoadUpdateV2 SelectedLoad { get; private set; }
		public IEnumerable<DatsLocation> SelectedLoadLocations { get; private set; }
		public event Action<string> LoadAction = delegate { };
		public List<string> TempVehiclesVIN = new List<string>();
		public List<DatsVehicleV5> deliveryList;
		private UploadStatus loadStatus;
		private List<DatsVehicleV5> cachedVehiclesInProgress = new List<DatsVehicleV5>();

		public CurrentLoadRepository(
			IServiceClientFactory<ITransportServiceClient> serviceClientFactory,
            IServiceClientFactory<IRestServiceClient> restClientFactory,
            ILoginRepository loginRepository,
			IDriverSignatureRepository driverSignatureRepository,
			IRepository<DatsRun, int> runRepository,
			IRepository<DatsRunStop, int> runStopRepository,
			IRepository<DatsVehicleV5, string> vehicleRepository,
			IRepository<DatsLocation, int> locationRepository,
			IRepository<SelectedLoads, int> selectedLoadRepository,
			IRepository<DamagePhoto, string> damagePhotoRepository)
		{
			this.serviceClientFactory = serviceClientFactory;
            this.restClientFactory = restClientFactory;
			this.loginRepository = loginRepository;
			this.driverSignatureRepository = driverSignatureRepository;
			this.runRepository = runRepository;
			this.runStopRepository = runStopRepository;
			this.vehicleRepository = vehicleRepository;
			this.locationRepository = locationRepository;
			this.selectedLoadRepository = selectedLoadRepository;
			this.damagePhotoRepository = damagePhotoRepository;

			GetCachedCurrentLoad();
			GetCachedSelectedLoads();

			DisplayOdometerPrompt = true;
		}

		public void RemoveVehicle(DatsVehicleV5 v){
			List<DatsVehicleV5> list = SelectedLoad.Vehicles.ToList ();
			list.Remove (v);
			SelectedLoad.Vehicles = list.ToArray ();
		}

		//For demo use to delete Vehicles that got delivered.
		public void UpdateLoadStatus(){

			LoadAction("SelectedLoadChanged");
		}

		public void ClearLoadInfo(bool removeSelected = true)
		{

			runRepository.DeleteAll();
			runStopRepository.DeleteAll();
			vehicleRepository.DeleteAll();
			locationRepository.DeleteAll();

			if (removeSelected) {
				CurrentLoad = new CurrentLoadResultV2();
				selectedLoadRepository.DeleteAll();
				SelectedLoad = new CurrentLoadUpdateV2();
				SelectedLoadLocations = new List<DatsLocation>();
				LoadAction("SelectedLoadChanged");
			}
		}

		public void CleanEvent()
		{
			LoadAction ("RemoveEvent");
		}

		public void SetSelectedLoads(string[] selectedLoadNumbers)
		{
			var vehicles = 
				(from v in CurrentLoad.Vehicles
				 from ln in selectedLoadNumbers
				where v.LoadNumber == ln
				select v).ToArray();
			var pickupLocationIds = vehicles.Select(v => v.PickupLocationId);
			var dropoffLocationIds = vehicles.Select(v => v.DropoffLocationId);
			var locationIds = pickupLocationIds.Union(dropoffLocationIds).Distinct();
			var locations =
				from l in CurrentLoad.Locations
				where locationIds.Contains(l.LocationId)
				select l;

			SelectedLoad.Runs = CurrentLoad.Runs;
			SelectedLoad.RunStops = CurrentLoad.RunStops;
			SelectedLoad.Vehicles = vehicles;
			var numbers = string.Join(",", selectedLoadNumbers);
			selectedLoadRepository.Save(new SelectedLoads { LoadNumbers = numbers });
			SelectedLoadLocations = locations;
			LoadAction("SelectedLoadChanged");
		}

		public void SetDropoffLocation(int locID)
		{
			dropoffLocationId = locID;
		}

		public Signature CustomerSignature
		{
			get { return customerSignature; }
			set
			{
				customerSignature = value;
			}
		}

		private void UpdateCachedLoadData(IEnumerable<DatsRun> runs, IEnumerable<DatsRunStop> runStops, IEnumerable<DatsVehicleV5> vehicles, IEnumerable<DatsLocation> locations)
		{
			runRepository.SaveAll(runs);
			runStopRepository.SaveAll(runStops);
			//workaround to preserve the original NumberOfReloads in case when
			//data go out of sync because of combination of offline and normal delivery to the same location
			foreach (DatsRunStop runstop in runStops) {
				runstop.OriginalNumberOfReloads = runstop.NumberOfReloads;
			}
				
			vehicleRepository.SaveAll(vehicles);
			locationRepository.SaveAll(locations);
		}

		private void ClientOnGetCurrentLoadCompleted(object sender, GetCurrentLoadCompletedEventArgs loadArgs)
		{
			serviceClientFactory.Instance.GetCurrentLoadCompleted -= ClientOnGetCurrentLoadCompleted;
			if (loadArgs.Error == null)
			{
				var result = loadArgs.Result;
				Debug.WriteLine("From the server:");

				ClearLoadInfo (false);

				foreach (var vehicle in result.Vehicles)
				{
					vehicle.FixStatusEnum();
					Debug.WriteLine(vehicle.ToDebugString(true, true, true));
				}

                CurrentLoad = result;

				if (TempVehiclesVIN.Count > 0) 
				{
					//loadnumber will only be > 0 after manage exception
					string[] loadnumbers =
						(from x in result.Vehicles where TempVehiclesVIN.Contains(x.VinKey) && x.LoadNumber != "0"
							select x.LoadNumber).Distinct().ToArray();

					var selectedLoads = selectedLoadRepository.GetAll().FirstOrDefault();
					if (selectedLoads == null && loadnumbers.Length > 0) 
					{
						SetSelectedLoads(loadnumbers);
					}

					TempVehiclesVIN.Clear ();
				}

				UpdateCachedLoadData(result.Runs, result.RunStops, result.Vehicles, result.Locations);
				Loads = BuildLoads();
				GetCachedSelectedLoads();
			}
			GetCurrentLoadCompleted(sender, loadArgs);
		}

		void ClientOnUploadCurrentLoadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			serviceClientFactory.Instance.UploadCurrentLoadCompleted -= ClientOnUploadCurrentLoadCompleted;
			UploadCurrentLoadCompleted(sender, e);
		}

		public void UploadDamagePhotos(List<DatsVehicleV5> list) {
			DatsVehicleV5 v;
			List<DamagePhoto> photos = null;
			List<DamagePhoto> uploadPhotos = null;
			List<DatsVehicleV5> vehicles = list;
			try {
				photos = this.damagePhotoRepository.GetAll ().ToList();
				uploadPhotos = new List<DamagePhoto> ();

				if(load != null)
					vehicles = load.Vehicles.ToList();

				if(vehicles == null)
					vehicles = vehicleRepository.GetAll().ToList();
				var files = Microsoft.Maui.Controls.DependencyService.Get<ILoadAndSaveFiles> ();

				foreach (DamagePhoto dp in photos) {

					if (dp.VehicleDamageDetailID == 0) {

						v = vehicles.FirstOrDefault(x => x.VehicleId == dp.VehicleID);

						if(v == null)
						{
							v = vehicles.FirstOrDefault(x => x.VIN == dp.VIN);
							dp.VehicleID = v.VehicleId;
							if(v == null)
								continue;

						}
						if(dp.InspectionType == (int) InspectionType.Loading) 
						{
								//processing pickup damagephotos
							if(v == null || v.VehicleStatus != VehicleStatus.Loaded.ToString()){
								continue;
							}
							//this is needed because sometimes loading a vehicle will initiate a new run
							try {
								dp.RunId = v.RunId;
							} catch (System.Exception ex) {
								dp.RunId = 0;
							}
						}
							

						try {
							dp.ImageData = files.LoadBinary (dp.FileName);
						} catch (System.Exception ex) {
							dp.ImageData = new byte[0];
						}
							
						uploadPhotos.Add (dp);

						dp.VehicleDamageDetailID = -1;
						this.damagePhotoRepository.Save (dp);
					} 
				}
			} catch(System.Exception ex) {
				//InsightsManager.Track ("Error in UploadPhotos: '" + ex.Message + "'");
			}
				
			damagePhotoList = uploadPhotos;

			SendSingleDamagePhoto ();

		}

		private void SendSingleDamagePhoto(){
			currPhoto = damagePhotoList.FirstOrDefault();
			if (currPhoto != null) {
				SendDamagePhotosAsync (currPhoto);
			} else {
				if(sendDamageEvent != null && sendDamageEvent.Error == null)
					//InsightsManager.Track ("Uploading photos complete successfully!");
				UploadDamagePhotosComplete (null, sendDamageEvent);
			}
		}

		private async void SendDamagePhotosAsync(DamagePhoto photo){
			DamagePhoto[] photos = new DamagePhoto[]{ photo};
            //serviceClientFactory.Instance.SendDamagePhotosCompleted += ClientOnSendDamagePhotosCompleted;
            //serviceClientFactory.Instance.SendDamagePhotosAsync (loginRepository.Username, loginRepository.Password, loginRepository.Truck, photos);

            await restClientFactory.Instance.SendDamagePhotosAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, photos);
            ClientOnSendDamagePhotosCompleted(this, new AsyncCompletedEventArgs(null,false,null));
        }

        void ClientOnSendDamagePhotosCompleted(object sender, AsyncCompletedEventArgs args) {

			serviceClientFactory.Instance.SendDamagePhotosCompleted -= ClientOnSendDamagePhotosCompleted;

			if (args.Error == null) {
				damagePhotoList.Remove (currPhoto);
				damagePhotoRepository.Delete (currPhoto);
			}
				
			sendDamageEvent = args;

			if (args.Error != null) {
				//InsightsManager.Track ("Uploading photos error: " + args.Error.Message);

				if (args.Error.InnerException != null) {
					//InsightsManager.Track ("Uploading photos inner error: " + args.Error.InnerException.Message);
				}
			} 

			SendSingleDamagePhoto ();
		}

		void ClientOnUpdateOdometerCompleted(object sender, AsyncCompletedEventArgs args)
		{
			serviceClientFactory.Instance.UpdateOdometerCompleted -= ClientOnUpdateOdometerCompleted;
			if (args.Error == null)
			{
				UpdatedMileage = null;
				SendCurrentLoad();
			}
			else
			{
				UploadCurrentLoadCompleted(sender, new GetCurrentLoadCompletedEventArgs(new object[0], args.Error, args.Cancelled, args.UserState));
			}

		}

		public void UpdateLocationAsync(string longitude, string lattidue)
		{
			var result = restClientFactory.Instance.UpdateGPSLocationAsync(loginRepository.Username,
				loginRepository.Password,
				loginRepository.Truck,
				longitude,
				lattidue);

			ClientOnUpdateLocationCompleted(null, new AsyncCompletedEventArgs(null, false, null));
		}

		public void SendGPSEmailNotifcation()
		{
			restClientFactory.Instance.SendGPSEmailNotification(loginRepository.Username,
				loginRepository.Password,
				loginRepository.Truck,
				loginRepository.LoginResult.FullName);

			ClientOnSendEmailCompleted(null, new AsyncCompletedEventArgs(null, false, null));
		}

		void ClientOnUpdateLocationCompleted(object sender, AsyncCompletedEventArgs args)
		{
			UpdateLocationCompleted(sender, args);
		}

		void ClientOnSendEmailCompleted(object sender, AsyncCompletedEventArgs args)
		{
			SendEmailCompleted(sender, args);
		}



		public int? UpdatedMileage { get; set; }

		public event EventHandler<GetCurrentLoadCompletedEventArgs> GetCurrentLoadCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UploadDamagePhotosComplete = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UploadCurrentLoadCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UpdateLocationCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> SendEmailCompleted = delegate { };

		/// <summary>
		/// Gets the current load from the server
		/// </summary>
		public async void GetCurrentLoadAsync(bool clearExceptions = true, bool isDelivery = false)
		{
			string returnload = "";
			//if it is delivery then add the selectedRunID
            try
            {
                if (isDelivery && deliveryList.Any())
                {
                    var vehicle = deliveryList.FirstOrDefault(v => v.RunId != 0);
                    if (vehicle != null)
                    {
                        returnload = vehicle.RunId.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                // log this in AppCenter
                Crashes.TrackError(ex);
            }


			//serviceClientFactory.Instance.GetCurrentLoadCompleted += ClientOnGetCurrentLoadCompleted;
			//serviceClientFactory.Instance.GetCurrentLoadAsync(
				//loginRepository.Username,
				//loginRepository.Password,
				//loginRepository.Truck,
				//clearExceptions,
				//returnload);

            try
            {
                var result = await restClientFactory.Instance.GetCurrentLoadAsync(loginRepository.Username,
                    loginRepository.Password,
                    loginRepository.Truck,
                    clearExceptions,
                    returnload);

                ClientOnGetCurrentLoadCompleted(null, new GetCurrentLoadCompletedEventArgs(new object[] { result }, null, false, null));
            } catch (Exception e)
            {
                ClientOnGetCurrentLoadCompleted(null, new GetCurrentLoadCompletedEventArgs(new object[] { null }, e, false, null));
            }
        }

        private void UpdateOdometerAsync(int newMileage)
		{
            /*
			serviceClientFactory.Instance.UpdateOdometerCompleted += ClientOnUpdateOdometerCompleted;
			serviceClientFactory.Instance.UpdateOdometerAsync(
				loginRepository.Username,
				loginRepository.Password,
				loginRepository.Truck,
				newMileage.ToString());
			*/

            restClientFactory.Instance.UpdateOdometerAsync(loginRepository.Username,
                loginRepository.Password,
                loginRepository.Truck,
                newMileage.ToString());

            ClientOnUpdateOdometerCompleted(null, new AsyncCompletedEventArgs(null,false,null));

            loginRepository.LoginResult.LastOdometerValue = newMileage;
			loginRepository.LoginResult.OdometerLastUpdateDate = TimeProvider.Current.Now;
		}

		public int GetRunCount(){
			return (CurrentLoad == null || CurrentLoad.Runs == null) ? 0 : CurrentLoad.Runs.Length;
		}
			
		public void StoreDeliveryInfo()
		{
			deliveryQueue.Enqueue(new DeliveryLoad(){
				Vehicles = deliveryList.ToArray(), 
				CustomerSignatureFileName = customerSignature.Filename,
				CustomerSignatureData = customerSignature.Bytes,
				DeliverySignatureFileName = driverSignatureRepository.DriverSignature.Filename,
				DeliverySignatureData = driverSignatureRepository.DriverSignature.Bytes,
				DropoffLocationId = dropoffLocationId,
				Reload = SelectedLoad.ReloadList});
		}

		public bool HasOfflineDelivery()
		{
			return deliveryQueue.Count > 0;
		}

		public bool OfflineDelivery()
		{

			if (load == null) 
			{
				return false;
			}
			else 
			{
				load = null;
				deliveryQueue.Dequeue ();
				if (deliveryQueue.Count > 0) {
					return true;
				} else 
				{
					return false;
				}
			}
		}

		public void UploadCurrentLoadAsync(UploadStatus status)
		{

			loadStatus = status;

			switch(loadStatus)
			{
				case UploadStatus.Delivery:
					SetDelivery ();
					break;
				case UploadStatus.DriverSignature:
					SetDriverSignature();
					break;
				case UploadStatus.Pickup:
					SendMileageAndLoad ();
					break;
				case UploadStatus.OfflineDelivery:
					SetOfflineDelivery ();
					break;
			}


		}

		private void SetPickupVehicles()
		{
			var selectedLoads = selectedLoadRepository.GetAll().FirstOrDefault();

			List<DatsVehicleV5> loadingVehicles = new List<DatsVehicleV5> ();

			foreach (var vehicle in SelectedLoad.Vehicles) 
			{
				
				if (vehicle.VehicleStatus == VehicleStatus.Delivering.ToString ()) {
					cachedVehiclesInProgress.Add (vehicle);
				}

				Debug.WriteLine (vehicle.ToDebugString (true, true, true));
				//not a selected load but a created one
				if (selectedLoads == null && vehicle.LoadNumber == "0") {
					TempVehiclesVIN.Add ((vehicle.VIN).Substring (vehicle.VIN.Length - 8));
				}

				loadingVehicles.Add (vehicle);
			}

			SelectedLoad.Vehicles = loadingVehicles.ToArray();
			SelectedLoad.DeliverySignature = new DeliverySignature[0];
			SelectedLoad.ReloadList = new DatsRunReload[0];
			SelectedLoad.LegsFileLinks = new LegFileLink[0];
			SendCurrentLoad ();

		}
			

		private void SetDelivery()
		{
			//set customer signature
			var fileName = Path.GetFileName(customerSignature.Filename);

			string deliveryVinList = "";

			if (deliveryList != null)
			{
				foreach (DatsVehicleV5 dv in deliveryList)
				{
					deliveryVinList += dv.VIN + ", ";
				}
			}
			else
			{
				Analytics.TrackEvent("Set Delivery Data is empty", new Dictionary<string, string> {
				{ "Driver", loginRepository.LoginResult.FullName },
				{ "Empty Delivery VIN List", deliveryVinList},
				});
				return;
			}

			Analytics.TrackEvent("Set Delivery Data", new Dictionary<string, string> {
				{ "Driver", loginRepository.LoginResult.FullName },
				{ "Customer signature path", customerSignature.Filename},
				{ "Customer file name", fileName},
				{ "Delivery VIN List", deliveryVinList},
			});

			SelectedLoad.DeliverySignature = new[]
			{
				new DeliverySignature
				{
					FileName = fileName,
					SignatureData = customerSignature.Bytes,
				}
			};

			SelectedLoad.LegsFileLinks = deliveryList
				.Select (v => 
					new LegFileLink {
						FileName = fileName,
						LegsId = v.LegId,
					}).ToArray ();


			foreach(DatsVehicleV5 v in SelectedLoad.Vehicles)
			{
				var deliveryVehicle = deliveryList.FirstOrDefault (x => x.VIN == v.VIN);


				if (v.VehicleStatus == VehicleStatus.Loading.ToString () || 
					(v.VehicleStatus == VehicleStatus.Delivering.ToString () && deliveryVehicle == null)) {
					cachedVehiclesInProgress.Add (v);
				}
			}
				
			SelectedLoad.Vehicles = deliveryList.ToArray();

			SendMileageAndLoad();
		}

		private void SetDriverSignature()
		{
			var vehicle = deliveryList.FirstOrDefault(v => v.RunId != 0);

			if (vehicle == null)
            {
				Analytics.TrackEvent("No vehicle found for run id > 0 in driver Signature", new Dictionary<string, string> {
					{ "Driver", loginRepository.LoginResult.FullName }
				});
			}
			else if (vehicle != null) {

				int[] deliveryLocationIDs = deliveryList.Select (v => v.OriginalDropoffLocation).Distinct ().ToArray ();

				foreach (int deliveryLocationID in deliveryLocationIDs) {
					driverSginatureQueue.Enqueue (new DeliverySignature (){FileName = CustomerSignature.Filename,  
						SignatureData = CustomerSignature.Bytes, RunId = vehicle.RunId, DropoffLocationId = deliveryLocationID });
				}

				if (driverSginatureQueue.Count == 0) {
					Analytics.TrackEvent("No delivery location for customer signature", new Dictionary<string, string> {
						{ "Driver", loginRepository.LoginResult.FullName },
						{ "Load number", vehicle.LoadNumber},
						{ "Vehicle Id", vehicle.VehicleId.ToString()},
					});
					UploadCurrentLoadCompleted (null, null);
					return;
				}
				var sig = driverSginatureQueue.Peek ();

				if (sig != null)
					SendDriverSignatureByLocation (sig);

			} else {
				//nothing to deliver so no signature needed
				UploadCurrentLoadCompleted(null, null);
			}
		}

		private void SetOfflineDelivery()
		{
			if (deliveryQueue.Count == 0)
				return;

			//bool offlineVehicles = false;
			if (load == null) {
				load = deliveryQueue.Peek ();
				//offlineVehicles = true;
			}

			bool hasException = false;
			List<DatsVehicleV5> exceptionVehicles = new List<DatsVehicleV5> ();
			foreach(DatsVehicleV5 v in SelectedLoad.Vehicles)
			{
				if (v.VehicleStatus == VehicleStatus.Loading.ToString ()) {
					cachedVehiclesInProgress.Add (v);
				}else if(v.VehicleStatus == VehicleStatus.Delivering.ToString ()) {
					
					if (!load.Vehicles.Any (x => x.VIN == v.VIN)) {
						cachedVehiclesInProgress.Add (v);
						//vehicleList.Remove (v);

					}
				}

				//we are dealing with delivery exception
				if (v.ExceptionCode == VehicleStatusCodes.EXCPTN_DELIVERING_DELIVERED ||
					v.ExceptionCode == VehicleStatusCodes.EXCPTN_UNEXPECTED_LOCATION) {
					hasException = true;
					exceptionVehicles.Add (v);

					if (v.ExceptionFlag == 1) {
						deliveryList.Remove (deliveryList.FirstOrDefault (z => z.VIN == v.VIN));
					}
				}
			}

			if (hasException) {
				SelectedLoad.Vehicles = exceptionVehicles.ToArray();
				SelectedLoad.ReloadList = new DatsRunReload[0];
			}else {
				deliveryList = load.Vehicles.ToList();
				SelectedLoad.Vehicles = load.Vehicles;

				SelectedLoad.ReloadList = load.Reload;

				int reloadSize = load.Reload.Length;

				var rs = CurrentLoad.RunStops.FirstOrDefault (r => (r.LocationId.HasValue && r.LocationId.Value == load.DropoffLocationId));
				if (rs != null) {
					if (rs.OriginalNumberOfReloads.HasValue) {
						rs.NumberOfReloads = rs.OriginalNumberOfReloads + reloadSize;
					} else {
						rs.NumberOfReloads = reloadSize;
					}
				}
			}

			var fileName = Path.GetFileName(load.CustomerSignatureFileName);
			SelectedLoad.DeliverySignature = new[]
			{
				new DeliverySignature
				{
					FileName = fileName,
					SignatureData = load.CustomerSignatureData,
				}
			};
				
			SelectedLoad.LegsFileLinks = deliveryList
				.Select (v => 
					new LegFileLink {
						FileName = fileName,
						LegsId = v.LegId,
					}).ToArray ();
						
			SendMileageAndLoad();
		}

		private void SendDriverSignatureByLocation(DeliverySignature sig, bool offlineDelivery = false)
		{
			driverSignatureRepository.SaveCompleted += OnSaveDriverSignatureCompleted;
			int[] legIds = null;
			legIds = deliveryList.Where(v => v.OriginalDropoffLocation == sig.DropoffLocationId).Select(v => v.LegId).ToArray();
			driverSignatureRepository.SaveAsync (sig.RunId, sig.DropoffLocationId, legIds);
		}

		private void OnSaveDriverSignatureCompleted(object sender, AsyncCompletedEventArgs args)
		{
			driverSignatureRepository.SaveCompleted -= OnSaveDriverSignatureCompleted; 
			if (args.Error == null)
			{
				if (driverSginatureQueue.Count > 0)
				{
					Analytics.TrackEvent("OnSaveDriverSignatureCompleted: dequeuing signature ", new Dictionary<string, string> {
						{ "Driver", loginRepository.LoginResult.FullName }
					});

					driverSginatureQueue.Dequeue();
				}

				if (driverSginatureQueue.Count > 0) {
					SendDriverSignatureByLocation (driverSginatureQueue.Peek ());
				} else {
					UploadCurrentLoadCompleted(sender, args);
				}
			}
			else
			{
				UploadCurrentLoadCompleted(sender, new GetCurrentLoadCompletedEventArgs(new object[0], args.Error, args.Cancelled, args.UserState));
			}
		}


		private void SendMileageAndLoad()
		{
			if (UpdatedMileage != null)
			{
				UpdateOdometerAsync(UpdatedMileage.Value);
			}
			else
			{
				switch(loadStatus)
				{
					case UploadStatus.Delivery:
						SendCurrentLoad ();
						break;
					case UploadStatus.Pickup:
						SetPickupVehicles ();
						break;
					case UploadStatus.OfflineDelivery:
						SendCurrentLoad ();
						break;
				}

			}
		}


		private async void SendCurrentLoad()
		{

			var selectedLoads = selectedLoadRepository.GetAll().FirstOrDefault();

			if (SelectedLoad.Runs == null)
			{
				SelectedLoad.Runs = CurrentLoad.Runs;
			}
			if (SelectedLoad.RunStops == null)
			{
				SelectedLoad.RunStops = CurrentLoad.RunStops;
			}

			Debug.WriteLine("Sending to the server:");

            try
            {
                await restClientFactory.Instance.UploadCurrentLoadAsync(
                        loginRepository.Username,
                        loginRepository.Password,
                        loginRepository.Truck,
                        SelectedLoad);

                ClientOnUploadCurrentLoadCompleted(null, new AsyncCompletedEventArgs(null, false, null));
            }
            catch (Exception e)
            {
                ClientOnUploadCurrentLoadCompleted(null, new AsyncCompletedEventArgs(e, false, null));
            }
        }
			
		public void GetCachedCurrentLoad()
		{
			CurrentLoad = new CurrentLoadResultV2();
			CurrentLoad.Runs = runRepository.GetAll().ToArray();
			CurrentLoad.RunStops = runStopRepository.GetAll().ToArray();
			CurrentLoad.Vehicles = vehicleRepository.GetAll().ToArray();
			CurrentLoad.Locations = locationRepository.GetAll().ToArray();
		}

		public void GetCachedSelectedLoads()
		{
			SelectedLoadLocations = new List<DatsLocation>();
			var selectedLoads = selectedLoadRepository.GetAll();
			if (!selectedLoads.Any())
			{
				selectedLoadRepository.DeleteAll();
				SelectedLoad = new CurrentLoadUpdateV2();
				return;
			}

			var selectedLoadNumbers = selectedLoads
				.SelectMany(l => l.LoadNumbers.SplitCorrectly(','))
				.Distinct();

			var vehicles =
				(from v in CurrentLoad.Vehicles
				from ln in selectedLoadNumbers
					where v.LoadNumber == ln && v.VehicleStatus != VehicleStatus.Delivered.ToString() 
				select v).ToArray();

			if (vehicles.Length == 0)
			{
				selectedLoadRepository.DeleteAll();
				SelectedLoad = new CurrentLoadUpdateV2();
				return;
			}

			List<DatsVehicleV5> selectedVehicles = new List<DatsVehicleV5> ();

			var vehicleNotInProgress = vehicles.Where(x => !cachedVehiclesInProgress.Any(y => y.VIN == x.VIN)).ToArray();

			selectedVehicles.AddRange (cachedVehiclesInProgress);
			selectedVehicles.AddRange (vehicleNotInProgress);
		
			cachedVehiclesInProgress.Clear ();
				
			var locationIds = selectedVehicles.Select(v => v.DropoffLocationId)
				.Union(selectedVehicles.Select(v => v.PickupLocationId));
			SelectedLoadLocations = CurrentLoad.Locations.Where(l => locationIds.Contains(l.Id));
			SelectedLoad = new CurrentLoadUpdateV2
			{
				Vehicles = selectedVehicles.ToArray(),
			};
		}

		private Load[] BuildLoads()
		{
			var loads = new List<Load>();
			List<string> loadnumbers =
				(from x in CurrentLoad.Vehicles
				 select x.LoadNumber).Distinct().ToList();

			foreach (string l in loadnumbers)
			{
				var load = new Load { LoadNumber = l };

				DatsVehicleV5 vehicle = CurrentLoad.Vehicles.FirstOrDefault(z => z.LoadNumber == l);
				if (vehicle != null)
				{
					if (CurrentLoad.Locations.Where (loc => loc.Name == vehicle.PickupLocationName).Distinct ().Count () > 1) {
						load.Origin = vehicle.PickupLocationName + " (" + vehicle.PickupLocationId + ")";
					} else {
						load.Origin = vehicle.PickupLocationName;
					}
					load.Type = vehicle.HotIndToString ();
				}

				List<string> dropoffLocationNames =
					(from v in CurrentLoad.Vehicles
					 where v.LoadNumber == l
					 select v.DropoffLocationName).Distinct().ToList();

				foreach (string place in dropoffLocationNames)
				{
					int vehicleCount = 
						(from v in CurrentLoad.Vehicles
						where v.LoadNumber == l && v.DropoffLocationName == place
						select v).Distinct().Count();
					load.StopList.Add(new Stop { Details = "(" + vehicleCount + ") " + place });
				}

				load.StopCount = dropoffLocationNames.Count();
				load.VehicleCount = CurrentLoad.Vehicles.Count(z => z.LoadNumber == l);

				loads.Add(load);
			}
			return loads.ToArray();
		}

		public DamagePhoto GetDamagePhoto(string key) {
			return damagePhotoRepository.GetById (key);
		}

		public void AddDamagePhoto(DamagePhoto photo) {
			damagePhotoRepository.Save (photo);
		}

		public List<DamagePhoto> GetDamagePhotos(int VehicleId, string VIN) {
			return damagePhotoRepository.GetAll ().Where (v => v.VehicleID == VehicleId && v.VIN == VIN).ToList ();
		}

		public void RemoveDamagePhotos(int VehicleId, string VIN) {
			foreach (DamagePhoto p in GetDamagePhotos(VehicleId, VIN)) {
				damagePhotoRepository.Delete (p);
			}
		}

		public bool HasDamagePhoto(){

			if (damagePhotoRepository.GetAll ().ToList ().Count > 0)
				return true;

			return false;

		}

		public void RemoveAllDamagePhotos(){
			List<DamagePhoto> photos = damagePhotoRepository.GetAll ()?.ToList ();
			foreach(DamagePhoto p in photos)
				damagePhotoRepository.Delete (p);
		}

		public void SetDeliveryVehicles (List<DatsVehicleV5> deliveryList){
			this.deliveryList = deliveryList;
		}
    }

	public static class CurrentLoadRepositoryExtensions
	{
		public static ObservableCollection<T> InitializeObservableCollection<T>(this ICurrentLoadRepository repo, Func<ICurrentLoadRepository, IEnumerable<T>> func2)
		{
			if (func2(repo) != null)
			{
				return new ObservableCollection<T>(func2(repo));
			}
			return new ObservableCollection<T>();
		}
		public static ObservableCollection<T> InitializeObservableCollection<T>(this ICurrentLoadRepository repo, Func<ICurrentLoadRepository, object> func1, Func<ICurrentLoadRepository, IEnumerable<T>> func2)
		{
			if (func1(repo) != null && func2(repo) != null)
			{
				return new ObservableCollection<T>(func2(repo));
			}
			return new ObservableCollection<T>();
		}

	}
}
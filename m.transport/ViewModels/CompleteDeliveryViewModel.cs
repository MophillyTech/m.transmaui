using System;
using System.ComponentModel;
using m.transport.Data;
using m.transport.Interfaces;
using m.transport.Svc;
using System.Text;	
using System.Text.RegularExpressions;
using System.Collections.Generic;
using m.transport.Domain;
using System.Linq;
using Autofac;
using System.Diagnostics;

namespace m.transport.ViewModels
{
	public class CompleteDeliveryViewModel : LoadViewModel
	{
		private readonly IDriverSignatureRepository driverSignatureRepo;
		private readonly IAppSettingsRepository settingRepo;
		protected readonly ILoginRepository loginRepo;
		private readonly ILoadAndSaveFiles fileRepo;
		private string driverFullName;
		private string customerFullName = "";
		private CustomObservableCollection<VehicleViewModel> reloadedVehicles;
		private int locationID;
		private string customerNameError = "";
		private bool driverSignatureSet = false;

		public event EventHandler<GetCurrentLoadCompletedEventArgs> GetCurrentLoadCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> SubmitDeliveryCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> UploadDamagePhotosCompleted = delegate { };
		public event EventHandler<AsyncCompletedEventArgs> SubmitDriverSignatureCompleted = delegate { };

		public CompleteDeliveryViewModel(
			int locationID,
			CustomObservableCollection<VehicleViewModel> reloadedVehicles)
			: base(App.Container.Resolve<ICurrentLoadRepository>())
		{
			this.locationID = locationID;
			this.reloadedVehicles = reloadedVehicles;
			this.driverSignatureRepo = App.Container.Resolve<IDriverSignatureRepository>();
			this.loginRepo = App.Container.Resolve<ILoginRepository>();
			this.fileRepo = App.Container.Resolve<ILoadAndSaveFiles>();
			this.settingRepo = App.Container.Resolve<IAppSettingsRepository> ();
			driverSignatureRepo.SaveCompleted += OnSaveCompleted;
			DriverFullName = loginRepo.LoginResult.FullName;
			DriverSigned = false;
			loadRepo.CustomerSignature = null;
		}

		public DatsLocation DestinationLocation
		{
			get{
				return Locations.FirstOrDefault (x => x.LocationId == locationID);
			}

		}
			
		public void UploadDriverSignature()
		{
			loadRepo.UploadCurrentLoadCompleted += LoadRepoOnUploadDriverSignatureCompleted;
			loadRepo.UploadCurrentLoadAsync(UploadStatus.DriverSignature);

		}

		public bool DriverSigned 
		{ 
			set 
			{
				driverSignatureSet = value;
				RaisePropertyChanged("MissingDriverSignature");
			}
			get 
			{
				return driverSignatureSet;
			}
		}
			
		public ICurrentLoadRepository LoadRepo
		{
			get{
				return loadRepo;
			}
		}


		public ILoginRepository LoginRepo
		{
			get{
				return loginRepo;
			}
		}

		public ILoadAndSaveFiles FileRepo
		{
			get{
				return fileRepo;
			}
		}

		public bool DriverSignatureExists
		{
			get { return fileRepo.FileExists(loginRepo.Username + ".png"); }
		}

		public string DriverSignatureFile
		{
			get { return fileRepo.GetFilePath(loginRepo.Username + ".png"); }
		}

		public string AccountType
		{
			get { return loginRepo.AccountType ;}
		}

		public int RunCount
		{
			get { return loadRepo.GetRunCount() ;}
		}

		public void RemovePhoto(int VehicleID, string VIN)
		{
			loadRepo.RemoveDamagePhotos (VehicleID, VIN);
		}

		private void OnSaveCompleted(object sender, AsyncCompletedEventArgs e)
		{
			RaisePropertyChanged(null);
		}

		public string DriverFullName
		{
			get { return driverFullName; }
			set
			{
				driverFullName = value;
				RaisePropertyChanged();
			}
		}

		public bool HasDamagePhoto(){
			return loadRepo.HasDamagePhoto ();
		}

		bool isValidated;

		public bool Validate()
		{
			Debug.WriteLine("Delivery Validion");
			isValidated = true;
			RaisePropertyChanged (null);
			return !MissingCustomerSignature && !MissingDriverSignature && !InvalidCustomerNameLength;
		}

		public void SubmitDeliveryAsync()
		{
			loadRepo.UploadCurrentLoadCompleted += LoadRepoOnUploadCurrentLoadCompleted;
			loadRepo.UploadCurrentLoadAsync(UploadStatus.Delivery);
		}

        public void GetCurrentLoadAsync(bool cleanException = false, bool isDelivery = true)
        {
            loadRepo.GetCurrentLoadCompleted += LoadRepoOnGetCurrentLoadCompleted;
            loadRepo.GetCurrentLoadAsync(cleanException, isDelivery);
        }

        private void LoadRepoOnUploadCurrentLoadCompleted(object sender, AsyncCompletedEventArgs args)
		{
			loadRepo.UploadCurrentLoadCompleted -= LoadRepoOnUploadCurrentLoadCompleted;
			SubmitDeliveryCompleted(sender, args);
		}
		private void LoadRepoOnGetCurrentLoadCompleted(object sender, GetCurrentLoadCompletedEventArgs args)
		{
			loadRepo.GetCurrentLoadCompleted -= LoadRepoOnGetCurrentLoadCompleted;
			if (args.Error == null) {





			}
			GetCurrentLoadCompleted(sender, args);
		}

		private void LoadRepoOnUploadDriverSignatureCompleted(object sender, AsyncCompletedEventArgs args)
		{
			loadRepo.UploadCurrentLoadCompleted -= LoadRepoOnUploadDriverSignatureCompleted;
			SubmitDriverSignatureCompleted (sender, args);
		}
			
		public bool MissingDriverSignature
		{
			get { return isValidated && !DriverSigned; }
		}

		public bool InvalidCustomerNameLength
		{
			get { 

				int len = customerFullName.Length;
				if (len == 0)
					CustomerError = "Please enter name";
				else if(len > 50)
					CustomerError = "Name can't exceed 50 charaters";

				return isValidated && (len == 0 || len > 50); 
			}
		}

		public string CustomerError { 


			get{
				return customerNameError;
			}
			set{
				customerNameError = value;
				RaisePropertyChanged();
			}
		
		}

		public bool MissingCustomerSignature
		{
			get { return isValidated && loadRepo.CustomerSignature == null; }
		}

		public string CustomerName
		{
			get { return customerFullName; }
			set
			{
				if (customerFullName != value)
				{
					customerFullName = value;
					RaisePropertyChanged(null);
				}
			}
		}

		public void SetCustomerSignature(Signature sig)
		{
			loadRepo.CustomerSignature = sig;
			RaisePropertyChanged("MissingCustomerSignature");
		}
		public void SetDriverSignature(Signature sig)
		{
			driverSignatureRepo.DriverSignature = sig;
			DriverSigned = true;
		}

		public bool HasDriverSignature()
        {
			return driverSignatureRepo.DriverSignature != null;
		}

		public void UploadDamagePhoto(CustomObservableCollection<VehicleViewModel> vehicles){

			List<DatsVehicleV5> list = new List<DatsVehicleV5> ();

			foreach (VehicleViewModel v in vehicles) {
				list.Add (v.DatsVehicle);
			}


			loadRepo.UploadDamagePhotosComplete += OnUploadPhotosCompleted;
			loadRepo.UploadDamagePhotos (list);
		}

		private void OnUploadPhotosCompleted(object sender, AsyncCompletedEventArgs args)
		{
			UploadDamagePhotosCompleted(sender, args);
		}

		public void CompleteDemoDelivery(CustomObservableCollection<VehicleViewModel> vehicles){
			foreach (VehicleViewModel v in vehicles) {
				v.DatsVehicle.SetVehicleStatus(VehicleStatus.Delivered);

				loadRepo.RemoveVehicle (v.DatsVehicle);
			}

			loadRepo.UpdateLoadStatus ();
		}
			
		public void SetDeliveryVehicles(List<DatsVehicleV5> deliveryList){
			loadRepo.SetDeliveryVehicles (deliveryList);
		}

		public void StoreDeliveryLoad()
		{
			loadRepo.StoreDeliveryInfo ();
		}

		public void ResetReloadVehicles() 
		{
			loadRepo.SelectedLoad.ReloadList = new DatsRunReload[0];
		}

		public void ProcessReloadVehicles()
		{
			if (reloadedVehicles.Count == 0) {
				ResetReloadVehicles ();
				return;
			}
				
			List<DatsRunReload> reloads = new List<DatsRunReload> ();
				
			foreach (VehicleViewModel v in reloadedVehicles) {
				v.DatsVehicle.ReloadInd = true;

				reloads.Add (new DatsRunReload () {
					RunId = v.DatsVehicle.RunId,
					LocationId = locationID,
					LegsId = v.DatsVehicle.LegId
				});
			}
		
			loadRepo.SelectedLoad.ReloadList = reloads.ToArray ();
			var rs = loadRepo.CurrentLoad.RunStops.FirstOrDefault (r => (r.LocationId.HasValue && r.LocationId.Value == locationID));
			if (rs != null) {
				if (rs.OriginalNumberOfReloads.HasValue) {
					rs.NumberOfReloads = rs.OriginalNumberOfReloads + reloadedVehicles.Count;
				} else {
					rs.NumberOfReloads = reloadedVehicles.Count;
				}
			}
		
		}

		public void RefreshSelectedLoad()
		{
			RefreshVehicles ();
		}

		public bool EnablePrintingDialogue 
		{
			get 
			{
				return settingRepo.PrintReceiptControlInd == 1 ? true : false;

			}
		}
	}
}

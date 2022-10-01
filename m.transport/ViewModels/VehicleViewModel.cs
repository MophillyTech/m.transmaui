using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using m.transport.Domain;
using m.transport.Models;
using m.transport.Interfaces;
using Autofac;
using m.transport.Data;

namespace m.transport.ViewModels
{
	public class Details
	{
		public string Attribute { get; set; }
		public string Value { get; set; }
		public bool IsDamage { get; set; }
		public bool HasPhoto { get; set; }
		public bool HasDamagePhoto { get; set; }
	}

	public class VehicleViewModel : BaseViewModel
	{
		public InspectionType InspectionType { get; set;}
		private readonly ICommand deleteDamageCommand;
		public DelegateCommand LocationSelectedCommand { get; set; }
		public event Action OnLocationSelected = delegate { };
		//private bool disclosure = true;
		private ICurrentLoadRepository currentLoadRepository;
		private bool hasLoadingDamagePhoto = false;
		private bool hasDeliveringDamagePhoto = false;

		private ObservableCollection<DamageViewModel> loadingDamageList = new ObservableCollection<DamageViewModel>();
		private ObservableCollection<DamageViewModel> deliveryDamageList = new ObservableCollection<DamageViewModel>();
		private DatsVehicleV5 datsVehicle;

		public VehicleViewModel(DatsVehicleV5 vehicle)
		{
			currentLoadRepository = App.Container.Resolve<ICurrentLoadRepository>();
			deleteDamageCommand = new DelegateCommand<DamageViewModel>(DeleteDamage);
			DatsVehicle = vehicle;
			DatsVehicle.FixStatusEnum();

			LocationSelectedCommand = new DelegateCommand(LocationSelectedExecuted);
		}

		private void RefreshPhotos() {

			List<DamagePhoto> photos = currentLoadRepository.GetDamagePhotos (this.DatsVehicle.VehicleId, this.DatsVehicle.VIN);

			loadingDamagePhotos = photos.Where (p => p.InspectionType == (int) InspectionType.Loading).ToList ();
			deliveryDamagePhotos = photos.Where (p => p.InspectionType == (int) InspectionType.Delivery).ToList ();


			if (loadingDamagePhotos == null || loadingDamagePhotos.Count == 0) {
				HasLoadingDamagePhoto = false;
			} else {
				HasLoadingDamagePhoto = loadingDamagePhotos.Any(s => (s.FileName != null && (s.FileName).Length > 0));
			}

			if (deliveryDamagePhotos == null || deliveryDamagePhotos.Count == 0) {
				HasDeliveringDamagePhoto = false;
			} else {
				HasDeliveringDamagePhoto = deliveryDamagePhotos.Any(s => (s.FileName != null && (s.FileName).Length > 0));
			}
		}

		public void RemovePhotos() 
		{


		}

		private void LocationSelectedExecuted(object obj)
		{
			OnLocationSelected();
		}

		public void RemoveEvent(string eventType)
		{
			RaisePropertyChanged(eventType);
		}
			
		public DatsVehicleV5 DatsVehicle
		{
			get { return datsVehicle; }
			set
			{	
				datsVehicle = value;
				CheckDamage (false);
			}
		}

		//TOOD: Figure out how this will be wired up
		//public string Damage { get; set; }

		public string Comment
		{
			get {
				if (DatsVehicle.AvailableForPickupDate.HasValue) {
					return DateTime.Now.Subtract (DatsVehicle.AvailableForPickupDate.Value).Days + " days old";
				} 

				return string.Empty;
			}
		}

		private bool _selected;

		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;
					RaisePropertyChanged();
				}
			}
		}

		public void CheckDamage(bool raisePoperty = true)
		{
			PopulateDamageList(InspectionType.Loading, LoadingDamageList);
			PopulateDamageList(InspectionType.Delivery, DeliveryDamageList);
			RefreshPhotos ();
			if(raisePoperty)
				RaisePropertyChanged ("RefreshLoad");
		}
			
		public ObservableCollection<DamageViewModel> LoadingDamageList
		{
			get { return loadingDamageList; }
		}

		public ObservableCollection<DamageViewModel> DeliveryDamageList
		{
			get { return deliveryDamageList; }
		}

		public void PopulateDamageList(InspectionType type, ObservableCollection<DamageViewModel> damageList)
		{
			damageList.Clear();

			var inspectionTypeToUse = type;
            if (type == InspectionType.Loading) {
                //if (DatsVehicle.VehicleStatus == "Loading") {
                //    var loadingCode = DatsVehicle.GetInspectionDamageCodes(InspectionType.Loading);
                //    foreach (string dc in loadingCode)
                //    {
                //        damageList.Add(new DamageViewModel(dc, type, DeleteDamageCommand));
                //    }
                //}

                var originCode = DatsVehicle.GetInspectionDamageCodes(InspectionType.Origin);
                foreach (string dc in originCode)
                {
                    damageList.Add(new DamageViewModel(dc, type, DeleteDamageCommand));
                }
            }

			List<DamagePhoto> damagePhotos = null;
			if(InspectionType == InspectionType.Loading && loadingDamagePhotos != null){
				damagePhotos = (loadingDamagePhotos.Where (p => p.VehicleID == DatsVehicle.VehicleId)).OrderBy(x=>x.DamageCode).ToList ();
			}else{
				if(deliveryDamagePhotos != null)
					damagePhotos = (deliveryDamagePhotos.Where (p => p.VehicleID == DatsVehicle.VehicleId)).OrderBy(x=>x.DamageCode).ToList ();
			}

			int index = 0;
			int sequenceCounter = -1;
			DamagePhoto photo;

			var tempCodes = DatsVehicle.GetInspectionDamageCodes(InspectionType.Temp);
            foreach (string dc in tempCodes)
			{

				if (damagePhotos != null && damagePhotos.Count > 0 && index < damagePhotos.Count &&  dc == damagePhotos [index].DamageCode ){
					sequenceCounter = damagePhotos [index].Sequence;
					ObservableCollection<string> photoList = new ObservableCollection<string> ();
					string reason = "";
					bool showPhoto = false;
					for (; index < damagePhotos.Count; index++) {
						if (sequenceCounter == damagePhotos [index].Sequence) {
							photo = damagePhotos [index];
							if (photo.FileName != null && photo.FileName.Length > 0) {
								photoList.Add (photo.FileName);
								showPhoto = true;
							}
							if (photo.NoPhotoReasonCode != null && photo.NoPhotoReasonCode.Length > 0)
								reason = photo.NoPhotoReasonCode;
						} else {
							break;
						}
					}
					DamageViewModel d = new DamageViewModel (dc, type, DeleteDamageCommand) {
						IsDeletable = true,
						//Photos = photoList,
						PhotoReason = reason,
					};
					d.SetPhotoVisbility (showPhoto);
					damageList.Add (d);

				} else {
					damageList.Add(new DamageViewModel(dc, type, DeleteDamageCommand) { IsDeletable = true});
				}
			}
		}

		public string Status { get { return DatsVehicle.VehicleStatus; } }

		public string SubDescription
		{
			get
			{
				return DatsVehicle.VehicleYear + "  |  " + DatsVehicle.Model + "  |  " + DatsVehicle.Color;
			}
		}

		public string Description
		{
			get
			{
				return VIN8 + "  |  " + DatsVehicle.BayLocation + "  |  " + DatsVehicle.Status;
			}
		}

		public string VIN8
		{
			get
			{
				if (DatsVehicle.VIN.Length <= 8)
					return DatsVehicle.VIN;

				return DatsVehicle.VIN.Substring(DatsVehicle.VIN.Length - 8, 8);
			}
		}

		public string VIN
		{
			get
			{
				return DatsVehicle.VIN;
			}
		}

		public bool HasLoadDamage
		{
			get
			{
				return !string.IsNullOrEmpty(DatsVehicle.LoadInspectionDamageCodes);
			}
		}

		public bool HasDeliveryDamage
		{
			get
			{
				return !string.IsNullOrEmpty(DatsVehicle.DeliveryInspectionDamageCodes);
			}
		}

		public List<Details> LoadingDetailList
		{
			get
			{
				var list = new List<Details>
				{
					new Details {Attribute = "VIN", Value = DatsVehicle.VIN, IsDamage = false},
					new Details {Attribute = "Make", Value = DatsVehicle.Make, IsDamage = false},
					new Details {Attribute = "Model", Value = DatsVehicle.Model, IsDamage = false},
					new Details {Attribute = "Year", Value = DatsVehicle.VehicleYear, IsDamage = false},
					new Details {Attribute = "Color", Value = DatsVehicle.Color, IsDamage = false},
					new Details {Attribute = "Load #", Value = DatsVehicle.LoadNumber, IsDamage = false},
					new Details {Attribute = "Customer Name", Value = DatsVehicle.CustomerName, IsDamage = false},
					new Details {Attribute = "Pickup Location", Value = DatsVehicle.PickupLocationName, IsDamage = false},
					new Details {Attribute = "Bay", Value = DatsVehicle.BayLocation, IsDamage = false},
					new Details {Attribute = "Delivery Location", Value = DatsVehicle.DropoffLocationName, IsDamage = false}
				};

				//RefreshPhotos ();
				//PopulateDamageList(InspectionType.Loading, LoadingDamageList);
				list.AddRange(LoadingDamageList.Select(d => new Details { Attribute = "Damage: " + d.DamageCode, Value = d.DamageInfo, IsDamage = true, HasDamagePhoto = (d.Photos).Count > 0 ? true : false}));

				return list;

			}
		}

		public List<Details> DeliveringDetailList
		{
			get
			{
				var list = new List<Details>
				{
					new Details {Attribute = "VIN", Value = DatsVehicle.VIN, IsDamage = false},
					new Details {Attribute = "Make", Value = DatsVehicle.Make, IsDamage = false},
					new Details {Attribute = "Model", Value = DatsVehicle.Model, IsDamage = false},
					new Details {Attribute = "Year", Value = DatsVehicle.VehicleYear, IsDamage = false},
					new Details {Attribute = "Color", Value = DatsVehicle.Color, IsDamage = false},
					new Details {Attribute = "Load #", Value = DatsVehicle.LoadNumber, IsDamage = false},
					new Details {Attribute = "Customer Name", Value = DatsVehicle.CustomerName, IsDamage = false},
					new Details {Attribute = "Pickup Location", Value = DatsVehicle.PickupLocationName, IsDamage = false},
					new Details {Attribute = "Bay", Value = DatsVehicle.BayLocation, IsDamage = false},
					new Details {Attribute = "Delivery Location", Value = DatsVehicle.DropoffLocationName, IsDamage = false}
				};

				//RefreshPhotos ();
				//PopulateDamageList(InspectionType.Delivery, DeliveryDamageList);
				list.AddRange(DeliveryDamageList.Select(d => new Details { Attribute = "Damage: " + d.DamageCode, Value = d.DamageInfo, IsDamage = true, HasDamagePhoto = (d.Photos).Count > 0 ? true : false}));

				return list;

			}
		}

		public bool PriorityVisible {
			get {
				return DatsVehicle.PriorityInd.HasValue;
			}
		}

		public string PriorityLabel {
			get {
				if (DatsVehicle.PriorityInd.HasValue) {
					if (datsVehicle.PriorityInd.Value == false) {
						return "!";
					}else if (datsVehicle.PriorityInd.Value == true) {
						return "H";
					} else {
						return string.Empty;
					}
				}

				return string.Empty;
			}
		}
			
		public bool HasLoadingDamagePhoto {
			get {
				return hasLoadingDamagePhoto;

				}
			set{
				hasLoadingDamagePhoto = value;
				RaisePropertyChanged ("HasDamagePhoto");

			}
		}

		public bool HasDeliveringDamagePhoto {
			get {
				return hasDeliveringDamagePhoto;

			}
			set{
				hasDeliveringDamagePhoto = value;
				RaisePropertyChanged ("HasDamagePhoto");

			}
		}

        public bool HasDeliveringNotes
        {
            get
            {
                return DatsVehicle.HasDropOffInspectionNotes;

            }
        }

        public ICommand DeleteDamageCommand
		{
			get { return deleteDamageCommand; }
		}

		public void AddDamage(DamageViewModel damageVm)
		{
			if (damageVm.InspectType == InspectionType.Loading)
				LoadingDamageList.Add (damageVm);
			else
				DeliveryDamageList.Add (damageVm);
			DatsVehicle.AddInspectionDamageCode(damageVm.DamageCode);
		}
		private void DeleteDamage(DamageViewModel damageVm)
		{
			if (damageVm.InspectType == InspectionType.Loading)
				LoadingDamageList.Remove (damageVm);
			else
				DeliveryDamageList.Remove (damageVm);
			DatsVehicle.RemoveInspectionDamageCode(damageVm.DamageCode);
		}

		public void SetVehicleStatus(VehicleStatus status)
		{
			DatsVehicle.SetVehicleStatus(status);
			RaisePropertyChanged("Status");
		}

		private List<DamagePhoto> deliveryDamagePhotos;
		public List<DamagePhoto> DeliveryDamagePhotos {
			get {
				return deliveryDamagePhotos;
			}
		}

		private List<DamagePhoto> loadingDamagePhotos;
		public List<DamagePhoto> LoadingDamagePhotos {
			get {
				return loadingDamagePhotos;
			}
		}
	}
}


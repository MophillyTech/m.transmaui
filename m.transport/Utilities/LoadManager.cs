using System;
using m.transport.Data;
using Autofac;
using System.Collections.ObjectModel;
using m.transport.Domain;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using m.transport.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace m.transport
{
	public sealed class LoadManager
	{
		public event Action<string> LoadChanged = delegate { };
		protected readonly ICurrentLoadRepository loadRepo;
		protected readonly IAppSettingsRepository settingRepo;
		private readonly DamageCodes codes;
		private List<VehicleViewModel> vms = new List<VehicleViewModel> ();
		public DamageCodes Codes { get; set; }

		private LoadManager()
		{
			loadRepo = App.Container.Resolve<ICurrentLoadRepository>();
			settingRepo = App.Container.Resolve<IAppSettingsRepository> ();
			Codes = settingRepo.Codes;
			settingRepo.SettingChanged += OnSettingChanged;
			loadRepo.LoadAction += OnLoadAction;
			OnSelectedLoadChanged ();
		}

		public static LoadManager Instance { get { return Nested.instance; } }

		private class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested()
			{
			}

			internal static readonly LoadManager instance = new LoadManager();
		}
			
		public void OnSettingChanged()
		{
			Codes = settingRepo.Codes;
		}
				
		public void OnLoadAction(string action)
		{
			if (action == "SelectedLoadChanged") {
				OnSelectedLoadChanged ();
			}else if (action == "RemoveEvent"){
				LoadChanged ("RemoveEvent");
			}
		}

		public void OnSelectedLoadChanged()
		{
			ObservableCollection<DatsVehicleV5> datsVehicles = loadRepo.InitializeObservableCollection(r => r.SelectedLoad, r => r.SelectedLoad.Vehicles);
			List<VehicleViewModel> vehicleList = new List<VehicleViewModel> ();
			foreach (DatsVehicleV5 v in datsVehicles) {
				vehicleList.Add(new VehicleViewModel(v));
			}

			foreach (VehicleViewModel v in vms) {
				VehicleViewModel vm = vehicleList.FirstOrDefault (vx => vx.DatsVehicle.VIN == v.DatsVehicle.VIN);
				if (vm == null) {
					if (v.InspectionType == InspectionType.Loading && v.DatsVehicle.ExceptionCode > 0)
						vehicleList.Add (v);
				}
			}

			vms = vehicleList;
			locations = loadRepo.InitializeObservableCollection(r => r.SelectedLoadLocations);
			LoadChanged ("SelectedLoadChanged");
		}

		public List<VehicleViewModel> Vehicles
		{
			get
			{
				return vms;
			}
			set 
			{
				vms = value;
			}
		}
			
		ObservableCollection<DatsLocation> locations;
		public ObservableCollection<DatsLocation> Locations
		{
			get
			{
				return locations;
			}
			set
			{
				if (locations != value)
				{
					locations = value;
				}
			}
		}
	}
}


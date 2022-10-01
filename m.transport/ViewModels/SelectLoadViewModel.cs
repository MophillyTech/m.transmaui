using System;
using System.Collections.ObjectModel;
using System.Linq;
using m.transport.Data;
using m.transport.Interfaces;
using m.transport.Models;
using m.transport.Svc;
using System.Collections.Generic;
using m.transport.Domain;
using Autofac;

namespace m.transport.ViewModels
{
	public class SelectLoadViewModel : BaseViewModel
	{
		private List<Load> loads;
		private readonly ICurrentLoadRepository repo;
		public event EventHandler<GetCurrentLoadCompletedEventArgs> GetCurrentLoadCompleted = delegate { };
		public int[] RunIds{ get; set;}
		public List<DatsVehicleV5>  Vehicles{ get; set;}
		private MainViewModel mainModel;

		public SelectLoadViewModel(ICurrentLoadRepository repo)
		{
			this.repo = repo;
			loads = new List<Load>();
			mainModel = new MainViewModel (App.Container.Resolve<ILoginRepository>(), App.Container.Resolve<IAppSettingsRepository>(), App.Container.Resolve<ICurrentLoadRepository>());
		}

		public List<Load> Loads
		{
			get
			{
				return loads;
			}
			set
			{
				loads = value;
				RaisePropertyChanged();
			}
		}

		public void GetCurrentLoadAsync()
		{
			repo.RemoveAllDamagePhotos ();
			repo.GetCurrentLoadCompleted += OnGetCurrentLoadCompleted;
			repo.GetCurrentLoadAsync(true, false);
		}

		private void OnGetCurrentLoadCompleted(object sender, GetCurrentLoadCompletedEventArgs e)
		{
			repo.GetCurrentLoadCompleted -= OnGetCurrentLoadCompleted;
			if (e != null && e.Error == null && repo.Loads != null)
			{
				List<Load> loadList = new List<Load> ();
				foreach (var l in repo.Loads)
				{
					if (repo.Loads.Length == 1)
						l.Selected = true;
					loadList.Add(l);
				}

				Loads = loadList.OrderBy (l => l.LoadNumber).ToList();

				RunIds = (from r in e.Result.Runs
				          select r.RunId).ToArray ();
				Vehicles = e.Result.Vehicles.ToList ();

			}
			GetCurrentLoadCompleted(sender, e);
		}

		public void BuildSelectedLoads()
		{
			var selectedLoads = Loads.Where(l => l.Selected).Select(l => l.LoadNumber).ToArray();
			repo.DisplayOdometerPrompt = true;
			repo.SetSelectedLoads(selectedLoads);
		}

		public void CreateEmptyLoad() {
			repo.DisplayOdometerPrompt = true;
			repo.ClearLoadInfo ();
		}

		public int LoadedVehicleCount(){
			return repo.CurrentLoad.Vehicles.Where(v => v.VehicleStatus == "Loaded").Count ();
		}

		public void UpdateConfiguration() {
			mainModel.UpdateConfiguration (true);
		}

	}
}


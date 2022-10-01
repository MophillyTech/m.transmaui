using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m.transport.Domain;
using m.transport.Data;
using Autofac;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Specialized;

namespace m.transport.ViewModels
{
	public class DeliveryDamageViewModel : LoadViewModel
	{
		public CustomObservableCollection<VehicleViewModel> SelectedVehicles { get; set; }
		public DeliveryInfo DeliveryInfo { get; set; }

		public DeliveryDamageViewModel(CustomObservableCollection<VehicleViewModel> vehicles, DeliveryInfo info)
			: base(App.Container.Resolve<ICurrentLoadRepository>())
		{
			SelectedVehicles = new CustomObservableCollection<VehicleViewModel>();
			DeliveryInfo = info;

			foreach (VehicleViewModel model in vehicles)
			{
				SelectedVehicles.Add(model);
			}

		}

		public void Refresh()
		{
			foreach (VehicleViewModel v in SelectedVehicles)
			{
				SelectedVehicles.ReportItemChange(v);
			}
		}
	}
}

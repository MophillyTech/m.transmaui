using System;
using m.transport.Data;
using m.transport.ViewModels;
using System.Collections.Generic;
using m.transport.Domain;

namespace m.transport
{
	public class DeliveryHistoryViewModel : BaseViewModel
	{

		private IAppSettingsRepository appRepo;

		public DeliveryHistoryViewModel (IAppSettingsRepository appRepo)
		{
			this.appRepo = appRepo;
		}

		public List<Paper> Papers
		{
			get
			{
				return appRepo.GetPaperListByType ();
			}
		}


	}
}


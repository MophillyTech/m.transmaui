using System;
using m.transport.Utilities;
using m.transport.Interfaces;
using m.transport.Data;
using System.IO;	
using Autofac;
using m.transport.Svc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace m.transport.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
		private DelegateCommand<string> openUrl;
		private readonly ILoginRepository loginRepo;
		private IServiceClientFactory<ITransportServiceClient> client; 

		private string webServiceUrl = string.Empty;
		private string coordinate = string.Empty;

		public AboutViewModel(ILoginRepository loginRepo)
        {
            this.loginRepo = loginRepo;

            client = App.Container.Resolve<IServiceClientFactory<ITransportServiceClient>>();
            
            webServiceUrl = client.Url;
            Mileage = loginRepo.LoginResult.LastOdometerValue;
        }

		public string Copyright
		{
			get { return string.Format("Copyright {0}. All Rights Reserved", CopyrightYear); }
		}

		public string GPS
        {
			get {
				return coordinate;
			}

			set {
				coordinate = value;
				RaisePropertyChanged();
			}
		}
		
		public int CopyrightYear
		{
			get { return TimeProvider.Current.UtcNow.Year; }
		}

		public string WebServiceUrl
		{
			get { return webServiceUrl; }
			set
			{
				if (webServiceUrl != value)
				{
					webServiceUrl = value;
					RaisePropertyChanged();
				}
			}
		}

		private string dispatcher = null;

		public string Dispatcher {
			get {
				if (dispatcher == null) {
					if (App.Current.Properties.ContainsKey ("DispatcherName")) {
						dispatcher = (string) App.Current.Properties ["DispatcherName"];
					}
				}

				return dispatcher;
			}
		}

		public string User
		{
			get { return loginRepo.Username; }
		}

		public string FullName
		{
			get { return loginRepo.FullName; }
		}

		public string Truck
		{
			get { return loginRepo.Truck; }
		}

		public int? Mileage { get; set; }

		public DelegateCommand<string> OpenUrl
		{
			get { return openUrl; }
			set { openUrl = value; }
		}
	}
}


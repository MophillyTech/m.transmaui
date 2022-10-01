using System;
using m.transport.Utilities;
using m.transport.Interfaces;
using m.transport.Data;
using Autofac;
using m.transport.Svc;
using m.transport.Services;
using m.transport.ServiceInterface;

namespace m.transport.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		public string WebServiceBase { get; set; }
		public string WebServicePath { get; set; }

		private IAppSettingsRepository repo;
		private ILoginRepository login;
		private ICurrentLoadRepository load;
		private IServiceClientFactory<ITransportServiceClient> client;

		public SettingsViewModel()
		{
			repo = App.Container.Resolve<IAppSettingsRepository> ();
			load = App.Container.Resolve<ICurrentLoadRepository> ();
			login = App.Container.Resolve<ILoginRepository> ();
			client = App.Container.Resolve<IServiceClientFactory<ITransportServiceClient>> ();

			WebServiceBase = repo.Settings ["WebServiceBase"];
			WebServicePath = repo.Settings ["WebServicePath"];
		}

		string oldUrl;
		Action success;
		Action error;

		public void Save(Action success, Action error) {

			this.success = success;
			this.error = error;

			oldUrl = client.Url;

			client.Url = WebServiceBase + WebServicePath;

			client.Instance.VersionCompleted += ClientOnVersionCompleted;

			client.Instance.VersionAsync ();
		}

		public void ClientOnVersionCompleted(object sender, VersionCompletedEventArgs e) {

			client.Instance.VersionCompleted -= ClientOnVersionCompleted;

			if (e.Error == null) {
				repo.Settings ["WebServiceBase"] = WebServiceBase;
				repo.Settings ["WebServicePath"] = WebServicePath;

				if (success != null) {
					success ();
				}

			} else {
				client.Url = oldUrl;

				if (error != null) {
					error ();
				}
			}
		}

		public void PrepareLogout() {

			load.ClearLoadInfo();
			login.Clear();

		}
	}
}

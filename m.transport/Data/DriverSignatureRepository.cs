using System;
using System.ComponentModel;
using System.Linq;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Svc;
using m.transport.Utilities;

namespace m.transport.Data
{
	public class DriverSignatureRepository: IDriverSignatureRepository
	{
		private readonly IServiceClientFactory<ITransportServiceClient> serviceClientFactory;
        private readonly IServiceClientFactory<IRestServiceClient> restClientFactory;
        private readonly ILoginRepository loginRepository;
		private readonly ILoadAndSaveFiles fileRepo;

		public DriverSignatureRepository(
			IServiceClientFactory<ITransportServiceClient> serviceClientFactory,
            IServiceClientFactory<IRestServiceClient> restClientFactory,
            ILoginRepository loginRepository,
			ILoadAndSaveFiles fileRepo)
		{
			this.serviceClientFactory = serviceClientFactory;
            this.restClientFactory = restClientFactory;
			this.loginRepository = loginRepository;
			this.fileRepo = fileRepo;

			GetCachedSignature();
		}

		private void ClientOnSendDriverSignatureCompleted(object sender, AsyncCompletedEventArgs args)
		{
			serviceClientFactory.Instance.SendDriverSignatureCompleted -= ClientOnSendDriverSignatureCompleted;
			if (args.Error == null)
			{
				loginRepository.LoginResult.IsSignatureOnFile = true;
				loginRepository.Save();
			}
			SaveCompleted(sender, args);
		}

		private void ClientOnGetDriverSignatureCompleted(object sender, GetDriverSignatureCompletedEventArgs args)
		{
			serviceClientFactory.Instance.GetDriverSignatureCompleted -= ClientOnGetDriverSignatureCompleted;
			if (args != null)
			{
				string filename = loginRepository.Username + ".png";
				DriverSignature = new Signature {Bytes = args.Result, Filename = filename};
				fileRepo.SaveBinary(filename, DriverSignature.Bytes);
			}
			LoadCompleted(sender, args);
		}

		public Signature DriverSignature { get; set; }

		public async void SaveAsync(int runId, int dropoffLocationId, int[] legIds)
		{
			var deliverySignature = DriverSignature.ToDeliverySignature();
			deliverySignature.RunId = runId;
			deliverySignature.DropoffLocationId = dropoffLocationId;
			deliverySignature.LegsFileLinks = legIds.Select(id => new LegFileLink {LegsId = id}).ToArray();
			
            //serviceClientFactory.Instance.SendDriverSignatureCompleted += ClientOnSendDriverSignatureCompleted;
			//serviceClientFactory.Instance.SendDriverSignatureAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, 
			//	deliverySignature);

            await restClientFactory.Instance.SendDriverSignatureAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, deliverySignature);
            ClientOnSendDriverSignatureCompleted(this, new AsyncCompletedEventArgs(null,false,null));

            fileRepo.SaveBinary(loginRepository.Username + ".png", DriverSignature.Bytes);
		}

		public event EventHandler<AsyncCompletedEventArgs> LoadCompleted = delegate { };

		public event EventHandler<AsyncCompletedEventArgs> SaveCompleted = delegate { };

		public void GetCachedSignature()
		{
			string filename = loginRepository.Username + ".png";
			if (fileRepo.FileExists(filename))
			{
				DriverSignature = new Signature
				{
					Filename = filename,
					Bytes = fileRepo.LoadBinary(filename),
				};
			}
		}

		public void ClearSignature()
		{
			string filename = loginRepository.Username + ".png";
			if (fileRepo.FileExists(filename))
			{
				fileRepo.Delete(filename);
			}
			filename = loginRepository.Username + ".points.bin";
			if (fileRepo.FileExists(filename))
			{
				fileRepo.Delete(filename);
			}
			DriverSignature = null;
		}

		public bool? IsSignatureOnFile
		{
			get { return loginRepository.LoginResult.IsSignatureOnFile; }
			set
			{
				loginRepository.LoginResult.IsSignatureOnFile = value;
				if (value == true && DriverSignature == null)
				{
					LoadAsync();
				}
			}
		}

		public async void LoadAsync()
		{
			//serviceClientFactory.Instance.GetDriverSignatureCompleted += ClientOnGetDriverSignatureCompleted;
		    //serviceClientFactory.Instance.GetDriverSignatureAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck);

            var result = await restClientFactory.Instance.GetDriverSignatureAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck);

            ClientOnGetDriverSignatureCompleted(this, new GetDriverSignatureCompletedEventArgs(new object[] { result }, null, false, null));
        }
    }
}

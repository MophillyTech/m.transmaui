using System;
using System.Collections.Generic;
using System.Linq;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Svc;
using m.transport.Models;
using mtd = m.transport.Domain;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport.Data
{
	public class AppSettingsRepository : IAppSettingsRepository
	{
		private readonly IServiceClientFactory<ITransportServiceClient> serviceClientFactory;
        private readonly IServiceClientFactory<IRestServiceClient> restClientFactory;
        private readonly ILoginRepository loginRepository;
		private readonly IRepository<mtd.DamageAreaCode, string> areaRepository;
		private readonly IRepository<mtd.DamageSeverityCode, string> severityRepository;
		private readonly IRepository<mtd.DamageTypeCode, string> typeRepository;
		private readonly IRepository<Setting, string> settingsRepository;
		private readonly IRepository<Paper, string> paperRepository;
		private readonly IRepository<Server, string> serverRepository;
		private readonly IRepository<Code, int> codeRepository;
		private VersionStatus versionStatus;
		private string updateMessage;
		public event Action SettingChanged = delegate { };

		public event EventHandler<GetCodesCompletedEventArgs> GetCodeTableCompleted = delegate { };

		public AppSettingsRepository(IServiceClientFactory<ITransportServiceClient> serviceClientFactory,
                                     IServiceClientFactory<IRestServiceClient> restClientFactory,
            ILoginRepository loginRepository,
			IRepository<mtd.DamageAreaCode, string> areaRepository,
			IRepository<mtd.DamageSeverityCode, string> severityRepository,
			IRepository<mtd.DamageTypeCode, string> typeRepository, 
			IRepository<Setting, string> settingsRepository,
			IRepository<Paper, string> paperRepository,
			IRepository<Code, int> codeRepository,
			IRepository<Server, string> serverRepository)
		
		{
			this.serviceClientFactory = serviceClientFactory;
            this.restClientFactory = restClientFactory;
			this.loginRepository = loginRepository;
			this.areaRepository = areaRepository;
			this.serverRepository = serverRepository;
			this.severityRepository = severityRepository;
			this.typeRepository = typeRepository;
			this.settingsRepository = settingsRepository;
			this.paperRepository = paperRepository;
			this.codeRepository = codeRepository;
			InitSetting ();
			Codes = new DamageCodes();
			Codes.Init();
			GetCachedDamageCodes();
			GetCachedMobileSettings();
		}

		public void InitSetting()
		{
			Settings = new Dictionary<string, string>
			{
				{ "MobileOdometerMileageLimit", "3000"},
				{ "MobileEnablePayHistory", "0"},
				{ "MobileSyncTime", DateTime.Now.ToString()}
			};

		}

		private void ClientOnGetMobileSettingsCompleted(object sender, GetMobileSettingsCompletedEventArgs args)
		{
			serviceClientFactory.Instance.GetMobileSettingsCompleted -= ClientOnGetMobileSettingsCompleted;
			if (args.Error == null)
			{
				InitSetting ();

				foreach (Setting s in args.Result.Settings) {
					if (Settings.ContainsKey (s.Key)) {
						// report dupe via Insights
						Settings[s.Key] = s.Value;
					} else {
						Settings.Add (s.Key, s.Value);
						settingsRepository.Save (s);
					}
				}

				//settingsRepository.SaveAll(Settings.ToList());

				// Settings = args.Result.Settings.ToDictionary(s => s.Key, s => s.Value);
				//settingsRepository.SaveAll(args.Result.Settings);
			}
			GetMobileSettingsCompleted(sender, args);
		}

		private void ClientOnGetDamageCodeListCompleted(object sender, GetDamageCodeListCompletedEventArgs args)
		{
			serviceClientFactory.Instance.GetDamageCodeListCompleted -= ClientOnGetDamageCodeListCompleted;
			if (args.Error == null)
			{
				ProcessDamageCode (args.Result);
				
			}
			GetDamageCodeListCompleted(sender, args);
		}

		private void ProcessDamageCode(DamageCodes dmgCodes) {

			Codes = dmgCodes;

			int index = 0;

			//format the damage area description
			foreach (DamageAreaCode d in Codes.Areas) {
				index = d.Description.IndexOf ('/');
				if(index > 0  && d.Description[index -1] != ' ')
					d.Description = d.Description.Replace ("/", " / ");
			}

			areaRepository.SaveAll(Codes.Areas);

			foreach (var d in Codes.Severities) {

				d.Description.Replace ("/", " / ");
			}

			severityRepository.SaveAll(Codes.Severities);

			index = 0;
			//format the damage area description
			foreach (mtd.DamageTypeCode d in Codes.Types) {
				index = d.Description.IndexOf ('/');
				if(index > 0 && d.Description[index -1] != ' ')
					d.Description = d.Description.Replace ("/", " / ");
			}
			typeRepository.SaveAll(Codes.Types);

			SettingChanged ();

		}


		public DamageCodes Codes { get; set; }

		public async void GetDamageCodeListAsync()
		{
            //serviceClientFactory.Instance.GetDamageCodeListCompleted += ClientOnGetDamageCodeListCompleted;
            //serviceClientFactory.Instance.GetDamageCodeListAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, "1");

            var result = await restClientFactory.Instance.GetDamageCodeListAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, "1");
            ClientOnGetDamageCodeListCompleted(null, new GetDamageCodeListCompletedEventArgs(new object[] { result }, null, false, null));

        }

		public event EventHandler<GetDamageCodeListCompletedEventArgs> GetDamageCodeListCompleted = delegate { };

		public void GetCachedDamageCodes()
		{
			var areas = areaRepository.GetAll().ToArray();
			if (areas.Length > 0)
			{
				Codes.Areas = areas;
			}
			var severities = severityRepository.GetAll().ToArray();
			if (areas.Length > 0)
			{
				Codes.Severities = severities;
			}
			var types = typeRepository.GetAll().ToArray();
			if (areas.Length > 0)
			{
				Codes.Types = types;
			}
		}
			
		public Dictionary<string, string> Settings { get; set; }

		public List<Paper> GetPaperListByType()
		{
			return paperRepository.GetAll().ToList();
		}

		public List<Server> GetDispatchServers()
		{
			return serverRepository.GetAll().ToList();
		}

		public void AddDispatchServer(Server server)
		{

			serverRepository.Save (server);
		}


		public void SavePaper(Paper p)
		{
			paperRepository.Save (p);

			List<Paper> papers = paperRepository.GetAll ().ToList();

			int count = papers.Count;

			if (count > 10) {
				int numToPurge = count - 10;

				// double check that this is the right order?
				papers = papers.OrderBy(pp => pp.Time).ToList();

				for (int ndx = 0; ndx < numToPurge; ndx++) {
					paperRepository.Delete (papers [ndx]);
				}
			}
		}

		public void UpdatePaper(String id, Paper newPaper) {

			DeletePaper (id);
			SavePaper (newPaper);
		}

		public void DeletePaper(String id) {

			foreach (Paper p in paperRepository.GetAll ().ToList()) {
				if (p.Id == id) {
					paperRepository.Delete (p);
					break;
				}
			}
		}

		public void ClearDeliveryHistory()
		{
			paperRepository.DeleteAll ();
		}


		public void GetCachedMobileSettings()
		{
			var newSettings = settingsRepository.GetAll().ToDictionary(s => s.Key, s => s.Value);
			foreach (var kvp in newSettings)
			{
				Settings[kvp.Key] = kvp.Value;
			}
		}

		public async void GetMobileSettingsAsync()
		{
            //serviceClientFactory.Instance.GetMobileSettingsCompleted += ClientOnGetMobileSettingsCompleted;
            //serviceClientFactory.Instance.GetMobileSettingsAsync();

            try
            {
				var result = await restClientFactory.Instance.GetMobileSettingsAsync();

				ClientOnGetMobileSettingsCompleted(null, new GetMobileSettingsCompletedEventArgs(new object[] { result }, null, false, null));
			} catch (Exception e)
            {
				ClientOnGetMobileSettingsCompleted(null, new GetMobileSettingsCompletedEventArgs(new object[] { null }, e, false, null));
			}

		}

		public event EventHandler<GetMobileSettingsCompletedEventArgs> GetMobileSettingsCompleted = delegate { };

		public VersionStatus CheckAppVersion() {

			IBuildInfo build = DependencyService.Get<IBuildInfo> ();

			versionStatus = VersionStatus.OK;

			int clientVersion = ConvertVersion(build.Version);
			int clientBuild = int.Parse(build.BuildNumber);

			int currentVersion = ConvertVersion (Settings ["MobileLatestVersion"]);
			int currentBuild = int.Parse (Settings ["MobileLatestBuild"]);

			int minVersion = ConvertVersion (Settings ["MobileEarliestVersion"]);
			int minBuild = int.Parse (Settings ["MobileEarliestBuild"]);

			// they SHOULD upgrade, but don't force!!
            if (currentVersion > clientVersion || (currentVersion == clientVersion && currentBuild > clientBuild)) {
				updateMessage = string.Format("This app is version {0} ({1}). There is a new version {2} ({3}) available, would you like to update now?", build.Version, clientBuild, Settings["MobileLatestVersion"], currentBuild);
				versionStatus = VersionStatus.UpdateAvailable;
			}

			// they MUST upgrade!!
			if (minVersion >= clientVersion && minBuild > clientBuild) {
				updateMessage = string.Format ("This app is version {0} ({1}), which is too old to work with the current dispatcher software. There is a new version {2} ({3}) available, would you like to update now?", build.Version, clientBuild, Settings["MobileLatestVersion"], currentBuild);
				versionStatus = VersionStatus.UpdateRequired;
			} 

			return versionStatus;
		}

		private int ConvertVersion(string version) {

			int retVal = 0;

			try {
				string[] split = version.Split (new char[] { '.' }, StringSplitOptions.None);

				if (split.Length == 3) {

					retVal = int.Parse (split [0]) * 1000000;
					retVal += int.Parse(split[1]) * 1000;
					retVal += int.Parse (split [2]);
				}
			} catch (System.Exception ex) {
			}

			return retVal;
		}

		public string AppStoreUrl {

			get {
				string url = string.Empty;

				//Device.OnPlatform(
				//	iOS: () => { url = "https://itunes.apple.com/app/id980753186"; }, 
				//	Android: () => { url = "https://play.google.com/store/apps/details?id=m.transport.Android"; });
				if (DeviceInfo.Current.Platform == DevicePlatform.Android)
				{
					url = "https://play.google.com/store/apps/details?id=m.transport.Android";
				}
				else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
				{
                    url = "https://itunes.apple.com/app/id980753186";
                }
                    return url;
			}
		}

		public VersionStatus VersionStatus {
			get { 
				return versionStatus;
			}
		}

		public string UpdateMessage { 
			get {
				return updateMessage;
			}
		}

		public int MobilePayHistoryInd {
			get{
				try {
					return Convert.ToInt32 (Settings ["MobileEnablePayHistory"]);
				} catch (System.Exception ex) {
					return 0;
				}
			}
		}

		public int MobileExpenseInd {
			get{
				try {
					return Convert.ToInt32 (Settings ["MobileEnableDriverExpenses"]);
				} catch (System.Exception ex) {
					return 0;
				}
			}
		}

		public int ExpenseDayLimit {
			get{
				try {
					return Convert.ToInt32 (Settings ["MobileExpenseAgeLimitDays"]);
				} catch (System.Exception ex) {
					return 14;
				}
			}
		}

		public int VehicleReloadInd {
			get{
				try {
					return Convert.ToInt32 (Settings ["MobileEnableReloads"]);
				} catch (System.Exception ex) {
					return 0;
				}
			}
		}

		public int DropLocationInd {
			get{
				try {
					return Convert.ToInt32 (Settings ["MobileEnableDropLocations"]);
				} catch (System.Exception ex) {
					return 0;
				}
			}
		}

		public int DeliveryTimeStamp {
			get{
				try {
					return Convert.ToInt32 (Settings ["DisplayTimeStampsMobile"]);
				} catch (System.Exception ex) {
					return 1;
				}
			}
		}

		public int PrintReceiptControlInd {
			get{
				try {
					return Convert.ToInt32 (Settings ["PrintReceiptControl"]);
				} catch (System.Exception ex) {
					return 0;
				}
			}
		}

		public DateTime MobileSettingSyncTime {
			get{
				DateTime val = DateTime.MinValue;
				DateTime.TryParse (Settings ["MobileSyncTime"], out val);
				return val;
			}
			set {
				Settings ["MobileSyncTime"] = value.ToString();
			}
		}

		public async void RefreshCodes(string codeType) {

            //serviceClientFactory.Instance.GetCodesCompleted += ServiceClientFactory_Instance_GetCodesCompleted;
            //serviceClientFactory.Instance.GetCodesAsync (loginRepository.Username, loginRepository.Password, loginRepository.Truck, codeType);

            var result = await restClientFactory.Instance.GetCodesAsync(loginRepository.Username, loginRepository.Password, loginRepository.Truck, codeType);
            ServiceClientFactory_Instance_GetCodesCompleted(null, new GetCodesCompletedEventArgs(new object[] { result }, null, false, null));
        }

		void ServiceClientFactory_Instance_GetCodesCompleted (object sender, GetCodesCompletedEventArgs e)
		{

			if (e.Error == null) {

				codeRepository.DeleteAll ();

				foreach (Code c in e.Result) {	
					codeRepository.Save (c);
				}
			}

			GetCodeTableCompleted (sender, e);
		}

		public List<Code> CodesByType(string codeType) {
			return codeRepository.GetAll ().Where(c => c.CodeType == codeType).ToList ();
		}
			
		public Dictionary<string, Code> CodesByTypeDictionary(string codeType) {

			Dictionary<string, Code> dict = new Dictionary<string, Code> ();

			foreach (Code c in CodesByType(codeType)) {

				dict.Add (c.CodeDescription, c);
			}

			return dict;
		}


		public void RemoveCodesByType(string codeType) {
			foreach (Code c in CodesByType(codeType)) {
				codeRepository.Delete (c);
			}
		}

	}
}

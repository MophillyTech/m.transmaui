using System;
using System.Collections.Generic;
using m.transport.Domain;
using m.transport.Svc;

namespace m.transport.Data
{
	public interface IAppSettingsRepository
	{
		DamageCodes Codes { get; set; }
		void GetCachedDamageCodes();
		void GetDamageCodeListAsync();
		event EventHandler<GetDamageCodeListCompletedEventArgs> GetDamageCodeListCompleted;

		Dictionary<string,string> Settings { get; set; }
		void GetCachedMobileSettings();
		void GetMobileSettingsAsync();
		void SavePaper(Paper p);
		void DeletePaper(String id);
		void UpdatePaper(String id, Paper p);
		event EventHandler<GetMobileSettingsCompletedEventArgs> GetMobileSettingsCompleted;
		event EventHandler<GetCodesCompletedEventArgs> GetCodeTableCompleted;

		List<Paper> GetPaperListByType();
		List<Server> GetDispatchServers();
		void AddDispatchServer(Server server);
		void ClearDeliveryHistory();

		VersionStatus CheckAppVersion();
		string AppStoreUrl { get; }
		VersionStatus VersionStatus { get; }
		string UpdateMessage { get; }

		int MobilePayHistoryInd { get; }
		int VehicleReloadInd {get;}
		int DropLocationInd {get;}
		int MobileExpenseInd {get;}
		int ExpenseDayLimit { get; }
		int DeliveryTimeStamp { get; }
		int PrintReceiptControlInd { get; }
		DateTime MobileSettingSyncTime { get; set;}

		void RefreshCodes (string codeType);
		List<Code> CodesByType(string codeType);
		Dictionary<string, Code> CodesByTypeDictionary(string codeType);
		void RemoveCodesByType(string codeType);

		event Action SettingChanged;
		void InitSetting ();
	}
}

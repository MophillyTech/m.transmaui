using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Utilities
{
	public static class MessageTypes
	{
		public static string VINScanned = "VINScanned";
		public static string DeleteDamagePhoto = "DeleteDamagePhoto";
		public static string NavigatePhotoView = "NavigateToPhotoView";
		public static string PhotoDamageReason = "PhotoDamageReason";
		public static string CreateDamage = "CreateDamage";
		public static string Exception = "Exception";
		public static string RefreshCurrentLoad = "RefreshCurrentLoad";
		public static string RemoveMessageSubscription = "RemoveMessageSubscription";
		public static string RefreshSetting = "RefreshSetting";
        public static string DeliveryCompleted = "DeliveryCompleted";
		public static string LogOut = "Logout";
	}
}

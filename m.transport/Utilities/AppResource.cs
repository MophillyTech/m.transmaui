using System;

namespace m.transport
{
	//Placeholder class that maintain the application's UI string
	//should we decide to localize the app all the values are stored
	//here so that it can easily migrated to resource file.
	//TODO slowly refactor and place the UI string here
	public static class AppResource
	{
		//Manage Delivery Screen
		public const string DELIVERY_DAMAGE_TITLE = "Delivery Damage/Comment Entered!";
		public const string DELIVERY_DISCARD_DAMAGE = "Discard Damages";
		public const string DELIVERY_DAMAGE_RETURN_ACTION = "Return To Delivery Damage";
		public const string DELIVERY_COMMENT_RETURN_ACTION = "Return To Delivery Condition";

		//Inspection Screen
		public const string PHOTO_REQUIRED = "Damage Photo Required!";
		public const string TAKE_PHOTO = "Take Photo";
		public const string UNABLE_TO_TAKE_PHOTO = "Unable To Take Photo";
		public const string RESTORE_CHANGE = "Restore The Change";
		public const string CANCEL_DAMAGE = "Cancel This Damage";
	}
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Models;
using m.transport.Svc;
using m.transport.ServiceInterface;

namespace m.transport.Data
{
	public interface ICurrentLoadRepository
	{
		Load[] Loads { get; set; }

		CurrentLoadResultV2 CurrentLoad { get; }
		CurrentLoadUpdateV2 SelectedLoad { get; }
		IEnumerable<DatsLocation> SelectedLoadLocations { get; }
		bool DisplayOdometerPrompt { get; set; }

		Signature CustomerSignature { get; set; }
		int? UpdatedMileage { get; set; }

		/// <summary>
		/// Gets the current load from the server
		/// </summary>
		/// <returns>whether online</returns>
		void GetCurrentLoadAsync(bool clearExceptions = true, bool isDelivery = false);
		event EventHandler<GetCurrentLoadCompletedEventArgs> GetCurrentLoadCompleted;

		void GetCachedCurrentLoad();
		void GetCachedSelectedLoads();

		void UploadCurrentLoadAsync(UploadStatus status);
		event EventHandler<AsyncCompletedEventArgs> UploadCurrentLoadCompleted;

		bool HasOfflineDelivery();
		void StoreDeliveryInfo ();
		bool OfflineDelivery();
		void ClearLoadInfo(bool removeSelected = true);
		event Action<string> LoadAction;
		void SetSelectedLoads(string[] selectedLoadNumbers);
		void SetDropoffLocation(int locID);
		void SetDeliveryVehicles (List<DatsVehicleV5> deliveryList);

		DamagePhoto GetDamagePhoto(string key);
		void AddDamagePhoto(DamagePhoto photo);
		List<DamagePhoto> GetDamagePhotos (int VehicleId, string VIN);
		void RemoveDamagePhotos (int VehicleId, string VIN);
		void RemoveAllDamagePhotos();
		void UploadDamagePhotos(List<DatsVehicleV5> vList);
		event EventHandler<AsyncCompletedEventArgs> UploadDamagePhotosComplete;
		void CleanEvent();
		void RemoveVehicle (DatsVehicleV5 v);
		//for demo use
		void UpdateLoadStatus();

		bool HasDamagePhoto();

		int GetRunCount();

		event EventHandler<AsyncCompletedEventArgs> UpdateLocationCompleted;
		event EventHandler<AsyncCompletedEventArgs> SendEmailCompleted;
		void UpdateLocationAsync(string longitude, string lattitude);
		void SendGPSEmailNotifcation();
	}
}
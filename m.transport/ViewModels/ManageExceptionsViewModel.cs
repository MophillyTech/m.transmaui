using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using m.transport.Domain;
using m.transport.Data;
using Autofac;
using System.Threading.Tasks;

namespace m.transport.ViewModels
{
	public class ManageExceptionsViewModel : LoadViewModel
	{
		private const int magNum = 1000;
		protected ICurrentLoadRepository currentLoadRepository;

		public ManageExceptionsViewModel(List<VehicleViewModel> vehicles, ICurrentLoadRepository repo) : base(repo)
		{
			currentLoadRepository = repo;

			ExceptionVehicles = new List<ExceptionViewModel>();
			bool isPickup = true;
			foreach (var v in vehicles)
			{
				isPickup = true;
				if (v.DatsVehicle.ExceptionCode == VehicleStatusCodes.EXCPTN_DELIVERING_DELIVERED ||
					v.DatsVehicle.ExceptionCode == VehicleStatusCodes.EXCPTN_UNEXPECTED_LOCATION) {

					isPickup = false;
				}

				ExceptionVehicles.Add(new ExceptionViewModel(v) { IsPickup = isPickup});
			}
		}

		public List<ExceptionViewModel> ExceptionVehicles { get; set; }

		public bool Validate()
		{
			foreach (ExceptionViewModel v in ExceptionVehicles) 
			{
				//has delivery vehicle
				//if(!v.IsPickup && v.Vehicle.DatsVehicle.ExceptionFlag < magNum)
					if (v.Vehicle.DatsVehicle.ExceptionFlag < magNum)
						return true;
			}

			return false;
		}

		public async Task<bool> ProcessException()
		{
            bool sendExceptionToDispatch = false;
			foreach (ExceptionViewModel v in ExceptionVehicles) {

				int flag = v.Vehicle.DatsVehicle.ExceptionFlag;

				//FOR LOADING only
				if (v.Vehicle.DatsVehicle.ExceptionFlag == 1 && v.IsPickup) {
					v.Vehicle.DatsVehicle.ExceptionFlag = 0;
				}else if (v.Vehicle.DatsVehicle.ExceptionFlag >= magNum) {
					//removing the magic number
					v.Vehicle.DatsVehicle.ExceptionFlag = v.Vehicle.DatsVehicle.ExceptionFlag - magNum;
				}

				//Pickup exception
				if (v.IsPickup) {
                    if (ProcessPickupExceptionVehicle(v.Vehicle)) {
                        sendExceptionToDispatch = true;
                    }
				} else {
					ProcessDeliveryExceptionVehicle (v.Vehicle);
                    sendExceptionToDispatch = true;
				}

			}

			System.Diagnostics.Debug.WriteLine ("Processing Exception");

            return sendExceptionToDispatch;
		}
	}

	public class ExceptionViewModel
	{
		public bool IsPickup { get; set; }

		public ExceptionViewModel(VehicleViewModel vehicle)
		{
			this.Vehicle = vehicle;
		}

		public string ExceptionPrompt
		{
			get
			{
				string prompt = string.Empty;

				switch (Vehicle.DatsVehicle.ExceptionCode)
				{
					case 1:
					case 3:
					case 5:
					case 6:
					case 7:
						prompt = "Add it to your current run?";
						break;
					case 8:
						prompt = "Vehicle Delivered already. Deliver again?";
						break;
					case 9:
						prompt = "Deliver it here instead?";
						break;
					case 12:
						prompt = "Leave it out of your current run?";
						break;
				}

				return prompt;
			}
		}

		public VehicleViewModel Vehicle { get; set; }

	}
}


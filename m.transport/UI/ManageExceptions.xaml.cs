using System;
using System.Collections.Generic;
using m.transport.Domain;
using m.transport.ViewModels;
using m.transport.Data;
using System.Linq;
using Autofac;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public partial class ManageExceptions : ContentPage
	{
		
		private Action cancel;
		private Action<bool> complete;
		private ICurrentLoadRepository repo;
		private bool isDelivery = false;
		private bool isPickup = false;

		public ManageExceptions(List<VehicleViewModel> vehicles, Action<bool> OnComplete, Action OnCancel)
		{
			repo = App.Container.Resolve<ICurrentLoadRepository>();
			complete = OnComplete;
			cancel = OnCancel;
			ViewModel = new ManageExceptionsViewModel(vehicles, repo);

			InitializeComponent();

			Title = "Manage Exceptions";

			var v = vehicles.FirstOrDefault (x => x.DatsVehicle.ExceptionCode == VehicleStatusCodes.EXCPTN_DELIVERING_DELIVERED ||
				x.DatsVehicle.ExceptionCode == VehicleStatusCodes.EXCPTN_UNEXPECTED_LOCATION);

			
			if (v == null) {
				//only show up when there is no delivery exception
				//this.ToolbarItems.Add(new ToolbarItem("Cancel", string.Empty,
				//	BuildExceptionResponse

				//	delegate 
				//	{
				//		if (cancel != null)
				//		{
				//			cancel();
				//		}
				//	}
				//		)
				//		);
				isPickup = true;
			} else {

				isDelivery = true;
			}
				
			this.ToolbarItems.Add(new ToolbarItem("Done", string.Empty, BuildExceptionResponse));

		}


		private async void BuildExceptionResponseForCancel()
        {
			//isCancel = true;
		}
		private async void BuildExceptionResponse()
		{

			if (ViewModel.Validate ()) {
				if (isDelivery)
				{
					DisplayAlert("Error!", "Please answer Yes or No for delivery exception", "OK");
				}
				else if (isPickup)
				{
					DisplayAlert("Error!", "Please answer Yes or No for VIN not loaded exception", "OK");
				}
					return;
			}

			bool hasException = await ViewModel.ProcessException ();

			if (complete != null)
			{
				System.Diagnostics.Debug.WriteLine ("Exception Complete CallBack");
                complete(hasException);
			}
		}

		public ManageExceptionsViewModel ViewModel
		{
			get { return (ManageExceptionsViewModel)BindingContext; }
			set { BindingContext = value; }
		}

		protected override bool OnBackButtonPressed (){

			if (isDelivery)
			{
				DisplayAlert("Error!", "Please answer Yes or No for delivery exception", "OK");
				return true;
			}
			else if (isPickup)
			{
				DisplayAlert("Error!", "Please answer Yes or No for VIN not loaded exception", "OK");
				return true;
			}

			return false;

		}
	}

}
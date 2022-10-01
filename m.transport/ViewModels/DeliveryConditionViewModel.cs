using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using m.transport.Domain;
using m.transport.Models;
using m.transport.Data;
using Autofac;
using System.Text.RegularExpressions;

namespace m.transport.ViewModels
{
	public class DeliveryConditionViewModel: LoadViewModel
	{
		public DeliveryInfo DeliveryInfo{ get; set;}
		private bool hasReason, showReason, showSurvey, hasSurvey, hasLocation;
		private string comment;
		private bool reloadable = false;
		private readonly IAppSettingsRepository appSettingsRepository = App.Container.Resolve<IAppSettingsRepository>();
		private CustomObservableCollection<VehicleViewModel> unselectedVehicles;
		private DatsLocation dest;
		private List<Code> dropLocations;
        private List<Code> deliveryResponses;
        private string locationErrorMessage;
		private string commentErrorMessage;

		public DeliveryConditionViewModel (CustomObservableCollection<VehicleViewModel> unselectedVehicles, int selectedLocID, DeliveryInfo info)
			: base(App.Container.Resolve<ICurrentLoadRepository>())
		{
			this.unselectedVehicles = unselectedVehicles;
			this.dest = Locations.FirstOrDefault (z => z.LocationId == selectedLocID);
			showReason = true;
            showSurvey = false;
			DeliveryInfo = info;
            DeliveryInfo.UnsafeDeliveryNotes = "";
            DeliveryInfo.UnsafeDeliveryResponse = "";

            dropLocations = appSettingsRepository.CodesByType (CodeType.MobileLocationCode.ToString ());
            deliveryResponses = appSettingsRepository.CodesByType(CodeType.SafeDeliveryPromptResponse.ToString());
        }
						
		public bool MissingReason
		{
			get 
			{ 
				return hasReason;
			}
			set
			{
				hasReason = value;
				RaisePropertyChanged();
			}
		}

        public List<String> DeliveryResponses()
        {
            List<String> responses = new List<string>();

            foreach (Code c in deliveryResponses)
            {
                responses.Add(c.CodeDescription);
            }

            return responses;
        }

        public bool MissingSurvey
        {
            get
            {
                return hasSurvey;
            }
            set
            {
                hasSurvey = value;
                RaisePropertyChanged();
            }
        }

        public string Comment
		{
			get
			{
				return comment;
			}
			set
			{
				comment = value;
				RaisePropertyChanged();
			}
		}

		public bool MissingLocation
		{
			get { 
				return hasLocation;
			}
			set
			{
				hasLocation = value;
				RaisePropertyChanged();
			}
		}

		public bool ReloadEnabled
		{
			get { 
				return unselectedVehicles.Count > 0;
			}
		}

		public bool DropLocationEnabled
		{
			get { 
				if (dest == null)
					return false;

				var location = dropLocations.FirstOrDefault (x => x.Value1 == dest.LocationId.ToString());

				return DropLocationVisible && location != null;
			}
		}

		public bool DropLocationVisible
		{
			get { 
				return appSettingsRepository.DropLocationInd == 1;
			}

		}

		public bool ShowReason
		{
			get { 
				return showReason;
			}
			set
			{
				showReason = value;
				RaisePropertyChanged();
			}
		}

        public bool ShowSurvey
        {
            get
            {
                return showSurvey;
            }
            set
            {
                showSurvey = value;
                RaisePropertyChanged();
            }
        }

        public bool Reload
		{
			get { 
				return reloadable;
			}
			set
			{
				reloadable = value;
				RaisePropertyChanged();
			}
		}

		public bool Validation()
		{

			MissingReason = (DeliveryInfo.Attended && DeliveryInfo.LoadInspection && DeliveryInfo.Reason == null) ? true : false;

            if (ShowSurvey)
            {
                if (DeliveryInfo.UnsafeDeliveryResponse == "Other")
                {
                    MissingSurvey = String.IsNullOrEmpty(DeliveryInfo.UnsafeDeliveryNotes) ? true : false;
                } else
                {
                    DeliveryInfo.UnsafeDeliveryNotes = null;
                    MissingSurvey = String.IsNullOrEmpty(DeliveryInfo.UnsafeDeliveryResponse) ? true : false;
                }
            } else
            {
                DeliveryInfo.UnsafeDeliveryNotes = null;
                DeliveryInfo.UnsafeDeliveryResponse = null;
            }

            MissingLocation = false;

			if (DropLocationEnabled) {
				if (string.IsNullOrEmpty (DeliveryInfo.DropLocation)) {
					locationErrorMessage = "Please input a location";
					MissingLocation = true;
				} else {
					DeliveryInfo.DropLocation = DeliveryInfo.DropLocation.Trim ();
					if (string.IsNullOrEmpty (DeliveryInfo.DropLocation)) {
						locationErrorMessage = "Please remove leading white space";
						MissingLocation = true;
					} else if (DeliveryInfo.DropLocation.Length > 20) {
						locationErrorMessage = "Location can't exceed 20 charcters";
						MissingLocation = true;
					} 
				}
			} 

			return MissingReason || MissingLocation || MissingSurvey;
		}

		public string LocationErrorMsg {
			get { return locationErrorMessage; }
		}

		public string CommentErrorMsg
		{
			get { return commentErrorMessage; }
		}
	}
}


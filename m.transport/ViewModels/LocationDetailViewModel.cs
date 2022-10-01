using m.transport.Domain;
using System;

namespace m.transport.ViewModels
{
	public class LocationDetailViewModel : BaseViewModel
	{
		private string name;
		private int locationID;
		private string type;
		private string subtype;
		private string address;
		private string contact;
		private string phone;
		private string contactphone;
		private string notes;
        private string nightDropAllowed;
        private string stiAllowed;
        private string driverDirections;
        private string deliveryTimes;
		private string message;

		public LocationDetailViewModel (DatsLocation loc)
		{
    		name = loc.Name;
			locationID = loc.LocationId;
			type = loc.LocationType;
			subtype = loc.LocationSubType;
			address = loc.AddressLine1;
            if (loc.STIAllowed.HasValue)
            {
                stiAllowed = loc.STIAllowed.Value ? "Yes" : "No";
            }
            if (loc.NightDropAllowed.HasValue)
            {
                nightDropAllowed = loc.NightDropAllowed.Value ? "Yes" : "No";
            }
            driverDirections = loc.DriverDirections;
            deliveryTimes = loc.DeliveryTimes;

            if (!string.IsNullOrEmpty(loc.AddressLine2)) {
				address += System.Environment.NewLine + loc.AddressLine2;
			}

			address += System.Environment.NewLine + loc.City + ", " + loc.State + " " + loc.Zip;

			contact = //loc.PrimaryContactFirstName + " " + loc.PrimaryContactLastName;
			phone = FormatPhone(loc.MainPhone);
            contactphone =  string.Empty;//loc.PrimaryContactPhone;
			notes = loc.LocationNotes;
			message = loc.LocationMessage;
		}

		public string Name {

			get { return name; }
			set {
				if (name != value) {
					name = value;
					RaisePropertyChanged ();
				}
			}
		}

		public string Type {

			get { return type; }
			set {
				if (type != value) {
					type = value;
					RaisePropertyChanged ();
				}
			}
		}

		public string SubType {

			get { return subtype; }
			set {
				if (subtype != value) {
					subtype = value;
					RaisePropertyChanged ();
				}
			}
		}

		public int LocationID {

			get { return locationID; }
			set {
				if (locationID != value) {
					locationID = value;
					RaisePropertyChanged ();
				}
			}
		}

		public string Address {

			get { return address; }
			set {
				if (address != value) {
					address = value;
					RaisePropertyChanged ();
				}
			}
		}

		public string Contact {

			get { return contact; }
			set {
				if (contact != value) {
					contact = value;
					RaisePropertyChanged ();
				}
			}
		}

		public string Phone {

			get { return phone; }
			set {
				if (phone != value) {
					phone = value;
					RaisePropertyChanged ();
				}
			}
		}

		private static string FormatPhone(string value)
		{ 
			value = new System.Text.RegularExpressions.Regex(@"\D")
				.Replace(value, string.Empty);
			value = value.TrimStart('1');
			if (value.Length == 7)
				return Convert.ToInt64(value).ToString("###-####");
			if (value.Length == 10)
				return Convert.ToInt64(value).ToString("(###) ###-####");
			if (value.Length > 10)
				return Convert.ToInt64(value)
					.ToString("(###) ###-#### " + new String('#', (value.Length - 10)));
			return value;
		}

		public string ContactPhone {

			get { return contactphone; }
			set {
				if (contactphone != value) {
					contactphone = value;
					RaisePropertyChanged ();
				}
			}
		}

		public string Notes {

			get { return notes; }
			set {
				if (notes != value) {
					notes = value;
					RaisePropertyChanged ();
				}
			}
		}

		public string Message {

			get { return message; }
			set {
				if (message != value) {
					message = value;
					RaisePropertyChanged ();
				}
			}
		}

        public string STIAllowed {

            get { return stiAllowed; }
            set {
                if (stiAllowed != value) {
                    stiAllowed = value;
                    RaisePropertyChanged ();
                }
            }
        }

        public string NightDropAllowed {
            get { return nightDropAllowed; }
            set {
                if (nightDropAllowed != value)
                {
                    nightDropAllowed = value;
                    RaisePropertyChanged();
                }
            }        
        }


        public string DeliveryTimes {
            get { return deliveryTimes; }
            set {
                if (deliveryTimes != value) {
                    deliveryTimes = value;
                    RaisePropertyChanged ();
                }
            }
        }

        public string DriverDirections {

            get { return driverDirections; }
            set {
                if (driverDirections != value) {
                    driverDirections = value;
                    RaisePropertyChanged ();
                }
            }
        }

	}
}


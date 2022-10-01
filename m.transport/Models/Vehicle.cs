using System;
using System.ComponentModel;
using m.transport.Domain;

namespace m.transport.Models
{
    public class zVehicle : INotifyPropertyChanged
    {
		private string _status;

        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Color { get; set; }
        
		public string Status { get { return _status; } set { 
				if (_status != value) {
					_status = value;
					NotifyPropertyChanged ("Status");
				}
			}
		}
        
		public string Location { get; set; }
        public string Damage { get; set; }
        public string VIN { get; set; }
        public string Comment { get; set; }

		public DatsLocation PickupLocation { get; set; }
		public DatsLocation DropoffLocation { get; set; }

		public string Description { 
			get {
				return Make + " " + Model + " (" + Color + ")";
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
    }
}
using System;
using m.transport.ViewModels;

namespace m.transport
{
	public class DeliveryInfo : BaseViewModel
	{

		public bool Attended { get; set; }
		public bool LoadInspection { get; set; }
		public string Reason{ get; set; }
		public string DropLocation { get; set;}
		public string Comment { get; set; }
        public string UnsafeDeliveryResponse { get; set; }
        public string UnsafeDeliveryNotes { get; set; }

        public DeliveryInfo ()
		{
		}
	}
}


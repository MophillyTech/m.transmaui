using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Domain
{
    public class MobileDevice
    {
		public MobileDevice() {
		}

        public int MobileDeviceID { get; set; }
        public int DriverID { get; set; }
        public string Platform { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string OSVersion { get; set; }
        public string PhoneNumber { get; set; }
        public string LastConnectionType { get; set; }
        public DateTime? LastConnectionDate { get; set; }
        public string AppVersion { get; set; }
        public int? AppBuild { get; set; }
        public string MobileName { get; set; }
        public string MobileCellCarrier { get; set; }
        public int MobileScreenHeight { get; set; }
        public int MobileScreenWidth { get; set; }
        public string RecordStatus { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
}

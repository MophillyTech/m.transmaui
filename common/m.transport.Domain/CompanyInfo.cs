using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Domain
{
    public class CompanyInfo
    {
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public byte[] ImageData { get; set; }

		public CompanyInfo() {
		}

		public CompanyInfo(string data) {

			string[] parsed = data.Split( new[] { "||" }, StringSplitOptions.None);

			try {
				CompanyName = parsed [0];
				AddressLine1 = parsed [1];
				AddressLine2 = parsed [2];
				City = parsed [3];
				State = parsed [4];
				Zip = parsed [5];
				Phone = parsed [6];
				Fax = parsed [7];
				EMail = parsed [8];
				ImageData = null;
			} catch (System.Exception ex) {
			}

		}

		public override string ToString ()
		{
			return string.Format ("{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}||{8}", CompanyName, AddressLine1, AddressLine2, City, State, Zip, Phone, Fax, EMail);
		}
    }
}

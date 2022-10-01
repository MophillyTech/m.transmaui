using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace m.transport.Domain
{
    
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DatsRunHistory
    {
		[DataMember]
		public int RunId { get; set; }
		[DataMember]
		public int? DriverId { get; set; }
		[DataMember]
		public DateTime? RunStartDate { get; set; }
		[DataMember]
		public string ShortStartDate { get; set; }
		[DataMember]
		public string LongStartDate { get; set; }
		[DataMember]
		public string LongEndDate { get; set; }
		[DataMember]
		public int? RunPayPeriodYear { get; set; }
		[DataMember]
		public int? RunPayPeriod { get; set; }
		[DataMember]
		public string PayPeriod { get; set; }
		[DataMember]
		public string CurrOrPrev { get; set; }
		[DataMember]
		public int? DriverRunNumber { get; set; }
		[DataMember]
		public string RunNumber { get; set; }
		[DataMember]
		public int? TotalStops { get; set; }
		[DataMember]
		public int TotalUnits { get; set; }
		[DataMember]
		public decimal Mileage { get; set; }
		[DataMember]
		public string HaulType { get; set; }
		[DataMember]
		public decimal TotalPay { get; set; }

		public DateTime EndDateTime
		{
			get {
				DateTime val = DateTime.MinValue;
				DateTime.TryParseExact (LongEndDate, dateFormats, 
					new CultureInfo ("en-US"), 
					DateTimeStyles.None, 
					out val);


//				DateTime val = DateTime.MinValue;
//				DateTime.TryParse (LongEndDate, out val);
				return val;
//				try{
//					
//					DateTime d = DateTime.Parse (LongEndDate);
//					return d.Ticks;
//				} catch(Exception e) {
//					return 0;
//				}
			}
		}



		public string PayPeriodLabel
		{
			get{
				return "Period:   " + RunPayPeriod;
			}
		}

		public string RunLabel
		{
			get{
				return "Run:      " + DriverRunNumber;
			}
		}

		public string UnitLabel
		{
			get{
				return "Units:    " + TotalUnits;
			}
		}

		public string PayLabel
		{
			get{
				return "$" + TotalPay;
			}
		}

		public string PayDetailLabel
		{
			get{
				return "Pay: $" + TotalPay;
			}
		}

		public string StartDateLabel
		{
			get{
				return "Start: " + LongStartDate;
			}
		}

		public string EndDateLabel
		{
			get{
				return "End:  " + LongEndDate;
			}
		}

		private string[] dateFormats= {"M/d/yyyy", 
						   "M/dd/yyyy",
		                   "MM/d/yyyy",
		                   "MM/dd/yyyy"};
    }
}

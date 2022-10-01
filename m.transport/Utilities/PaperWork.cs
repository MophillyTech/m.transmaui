using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using m.transport.Data;
using m.transport.Domain;
using m.transport.Interfaces;
using m.transport.Svc;
using m.transport.Utilities;
using m.transport.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace m.transport
{
	public static class PaperWork
	{

		private static IPrinter printer;
		private static readonly IAppSettingsRepository appSettingsRepository = App.Container.Resolve<IAppSettingsRepository>();


		public static bool PrintDeliveryHistory(byte[] data, byte[] offsets){
			printer = DependencyService.Get<IPrinter> ();
			if (printer.IsPrinterAvailable ()) {
				printer.PrintReceipt (data, offsets);
				return true;
			}
			return false;
		}

		public static Boolean PrintLoadSummary(ILoginRepository loginRepo, IEnumerable<GroupedVehicles> groupedVehicles, IList<DatsLocation> locations, string selectedLocation){
			printer = DependencyService.Get<IPrinter> ();

			int pickupID = -1;

			if (printer.IsPrinterAvailable ()) {
				printer.Clear ();
				printer.PrintText ("Load Summary", true);

				//printer.PrintLogo ();
				PrintHeader (printer, loginRepo);
				printer.SkipLines (2);

				printer.PrintText ("Driver Name:    " + loginRepo.LoginResult.FullName); 
				printer.PrintText ("Driver ID:      " + loginRepo.LoginResult.Driver); 
				printer.PrintText ("Truck Num:      " + loginRepo.Truck);

				printer.SkipLines (2);

				foreach (GroupedVehicles g in groupedVehicles) {

					DatsLocation loc = g.Location;
					if (selectedLocation != "All" && loc != null && loc.DisplayName != selectedLocation) {
						continue;
					}
						
					printer.PrintText ("Origin: ", false, true, true);

					if (loc != null && loc.Id > 0 && loc.DisplayName != null) {
						printer.PrintText (loc.DisplayName);  
						printer.PrintText (loc.AddressLine1);
						if (loc.AddressLine2.Length > 0)
							printer.PrintText (loc.AddressLine2);
						printer.PrintText (loc.City + ", " + loc.State + " " + loc.Zip);
						pickupID = loc.Id;
					} else {
						pickupID = -2;
						printer.SkipLines(4);
					}


					printer.SkipLines (2);

					if (!g.Vehicles.Any(s => (s.DatsVehicle.VehicleStatus == "Loading" || s.DatsVehicle.VehicleStatus == "Loaded") 
						&& s.DatsVehicle.PickupLocationId == pickupID)) {
						printer.PrintText ("There are no vehicles from this location", false, true, true); 
						printer.SkipLines (2);
						continue;
					}

					List<int> dropofflocationIdList = (from x in g.Vehicles select x.DatsVehicle.DropoffLocationId).Distinct ().ToList ();


					foreach (int pid in dropofflocationIdList)
					{
				
						List<VehicleViewModel> vehicleList =  (from y in g.Vehicles where y.DatsVehicle.DropoffLocationId == pid
							&& (y.DatsVehicle.VehicleStatus == "Loaded" || y.DatsVehicle.VehicleStatus == "Loading") select y).ToList();
							
						if (vehicleList.Count == 0)
							continue;

						DatsLocation dest = locations.FirstOrDefault(z => z.Id == pid);

						printer.PrintText ("Destination: ", false, true, true);
						if (dest != null) {
							printer.PrintText (dest.DisplayName);
							printer.PrintText (dest.AddressLine1);
							if (dest.AddressLine2.Length > 0)
								printer.PrintText (dest.AddressLine2);
							printer.PrintText (dest.City + ", " + dest.State + " " + dest.Zip);
						} else {
							printer.SkipLines(4);

						}
						printer.SkipLines(2);

						foreach (VehicleViewModel v in vehicleList)
						{
							printer.PrintText ("Load Number: " + v.DatsVehicle.LoadNumber);
							if (v.DatsVehicle.PickupDate != null)
							{
								printer.PrintText("Date Loaded: " + v.DatsVehicle.PickupDate.Value.ToString("d"));
							}

							printer.SkipLines (1);

							printer.PrintText (v.VIN, true);
							string desc = v.DatsVehicle.VehicleYear + " " + v.DatsVehicle.Make + " " + v.DatsVehicle.Model;

							printer.PrintText (desc, false, true, true, 0);

							printer.SkipLines (1);

						}

						printer.SkipLines (2);
			
					}
				}

				printer.SkipLines (5);
				printer.PrintData ();

				return true;
			}

			return false;
		}

		private static void PrintHeader(IPrinter printer, ILoginRepository loginRepo) {

            CompanyInfo info = loginRepo.LoginResult.CompanyInfo;

			/*
			var client = App.Container.Resolve<IServiceClientFactory<ITransportServiceClient>> ();

			var header = new Dictionary<string, string[]> ();

			// this data should come from the server and be stored on login instead of hardcoded
			header.Add ("DAI", new[] { "Diversified Automotive, Inc.", "100 Terminal Street", "Charlestown, MA 02119", "" });
			header.Add ("Mophilly", new [] { "Mophilly Technology, Inc.", "3843 Park Boulevard, Suite 7", "San Diego CA 92103", "" });
			header.Add ("Tribeca", new [] { "Tribeca Automotive, Inc.", "1179 Roosevelt Ave", "Carteret NJ 07008", "" });
			header.Add ("Excel", new [] { "DELUXE AUTO CARRIERS INC., dba EXCEL TRANSPORTING", "2353 S. Cactus Avenue", "Bloomington, CA 92316", "Email : dispatch@excelautotransport.com" });

			string key = string.Empty;

			// this will go away when we start downloading client info and logo from server
			if (client.Url.Contains ("transportws")) {
				key = "Mophilly";
			} else if (client.Url.Contains ("diversified")) {
				key = "DAI";
			} else if (client.Url.Contains ("excel")) {
				key = "Excel";
			} else if (client.Url.Contains ("tribeca")) {
				key = "Tribeca";
			}

			if (string.IsNullOrEmpty (key))
				return;

			printer.PrintText (header [key] [0]);
			printer.PrintText (header [key] [1]);
			printer.PrintText (header [key] [2]);
			if(header [key] [3].Length > 0)
				printer.PrintText (header [key] [3]);

			*/

			printer.PrintText (info.CompanyName);
			printer.PrintText (info.AddressLine1);
			printer.PrintText (info.AddressLine2);
			printer.PrintText (string.Format ("{0}, {1} {2}", info.City, info.State, info.Zip));
			printer.PrintText (FormatPhone(info.Phone));
			printer.PrintText (FormatPhone(info.Fax));
			printer.PrintText (info.EMail);

		}

		private static string FormatPhone(string value)
		{ 
			value = new System.Text.RegularExpressions.Regex(@"\D")
				.Replace(value, string.Empty);
			value = value.TrimStart('1');
			if (value.Length == 7)
				return Convert.ToInt64(value).ToString("###-####");
			if (value.Length == 10)
				return Convert.ToInt64(value).ToString("###-###-####");
			if (value.Length > 10)
				return Convert.ToInt64(value)
					.ToString("###-###-#### " + new String('#', (value.Length - 10)));
			return value;
		}


		public static Boolean PrintGatePass(ICurrentLoadRepository loadRepository, ILoginRepository loginRepo, IEnumerable<GroupedVehicles> groupedVehicles, IList<DatsLocation> locations, string selectedLocation){

			//List<DatsRunStop> runStopList = loadRepository.CurrentLoad.RunStops.ToList();

			printer = DependencyService.Get<IPrinter> ();
			int pickupID = -1;

			if (printer.IsPrinterAvailable ()) {
				printer.Clear ();
				printer.PrintText ("Gate Pass", true);

				//printer.PrintLogo ();
                PrintHeader (printer, loginRepo);

				printer.PrintText ("Driver Name:    " + loginRepo.LoginResult.FullName); 
				printer.PrintText ("Driver ID:      " + loginRepo.LoginResult.Driver); 
				printer.PrintText ("Truck Num:      " + loginRepo.Truck);

				printer.SkipLines (2);

				foreach (GroupedVehicles g in groupedVehicles) {

					DatsLocation loc = g.Location;
					if (selectedLocation != "All" && loc != null && loc.DisplayName != selectedLocation) {
						continue;
					}

					printer.PrintText ("Origin: ", false, true, true);

					if (loc != null && loc.Id > 0 && loc.DisplayName != null) {
						printer.PrintText (loc.DisplayName);  
						printer.PrintText (loc.AddressLine1);
						if (loc.AddressLine2.Length > 0)
							printer.PrintText (loc.AddressLine2);
						printer.PrintText (loc.City + ", " + loc.State + " " + loc.Zip);
						pickupID = loc.Id;
					} else {
						pickupID = 0;
						printer.SkipLines(4);
					}


					printer.SkipLines (2);

					if (!g.Vehicles.Any(s => (s.DatsVehicle.VehicleStatus == "Loading" || s.DatsVehicle.VehicleStatus == "Loaded") 
						&& (s.DatsVehicle.PickupLocationId == pickupID || s.DatsVehicle.PickupLocationId == -2))) {
						printer.PrintText ("There are no vehicles from this location", false, true, true); 
						printer.SkipLines (2);
						continue;
					}
						
					List<int> dropofflocationIdList = (from x in g.Vehicles select x.DatsVehicle.DropoffLocationId).Distinct ().ToList ();

					foreach (int pid in dropofflocationIdList)
					{

						List<VehicleViewModel> vehicleList =  (from y in g.Vehicles where (y.DatsVehicle.DropoffLocationId == pid || 
							y.DatsVehicle.DropoffLocationId == -1) && (y.DatsVehicle.PickupLocationId == pickupID || y.DatsVehicle.PickupLocationId == -2)
							&& (y.DatsVehicle.VehicleStatus == "Loading" || y.DatsVehicle.VehicleStatus == "Loaded") select y).ToList();

						if (vehicleList.Count == 0)
							continue;

						DatsLocation dest = locations.FirstOrDefault(z => z.Id == pid);

						printer.PrintText ("Destination: ", false, true, true);
						if (dest != null) {
							printer.PrintText (dest.DisplayName);
							printer.PrintText (dest.AddressLine1);
							if (dest.AddressLine2.Length > 0)
								printer.PrintText (dest.AddressLine2);
							printer.PrintText (dest.City + ", " + dest.State + " " + dest.Zip);
						} else {
							printer.SkipLines(4);

						}
							
						/*
						DatsRunStop stop = runStopList.Find(x => x.LocationId == localPid);

						if(stop!= null && !String.IsNullOrEmpty(stop.Directions)){
							printer.SkipLines (1);
							printer.PrintText ("Delivery Instruction: " + stop.Directions);
						}
						*/

						printer.SkipLines (2);
						
						foreach (VehicleViewModel v in vehicleList) {

							printer.PrintText ("Load Number: " + v.DatsVehicle.LoadNumber);
							if (v.DatsVehicle.PickupDate != null)
							{
								printer.PrintText("Date Loaded: " + v.DatsVehicle.PickupDate.Value.ToString("d"));
							}

							printer.SkipLines (1);

							printer.PrintText (v.VIN + "     BAY: " + v.DatsVehicle.BayLocation, true);
							string desc = v.DatsVehicle.VehicleYear + " " + v.DatsVehicle.Make + " " + v.DatsVehicle.Model;

							printer.PrintText (desc, false, true, true, 0);

							foreach(DamageViewModel d in v.LoadingDamageList){
								printer.PrintText (d.DamageCode + ": " + d.DamageInfo, false, true, false, 0);

							}

							printer.PrintBarcode (v.DatsVehicle.VIN);

							printer.SkipLines (1);

						}
						
					}
				}
			
				printer.SkipLines (5);
				printer.PrintData ();

				return true;
			}

			return false;
		}
		
		public static Boolean PrintSample(ILoginRepository loginRepo, IServiceClientFactory<ITransportServiceClient> clientFactory, IBuildInfo buildInfo){
			printer = DependencyService.Get<IPrinter> ();
			
			if (printer.IsPrinterAvailable ()) {
				printer.Clear ();
				
				printer.PrintText ("Printer Test Report", true);
                PrintHeader (printer, loginRepo);
								
				printer.PrintText ("Driver Name:    " + loginRepo.LoginResult.FullName); 
				printer.PrintText ("Driver ID:      " + loginRepo.LoginResult.Driver); 
				printer.PrintText ("Truck Num:      " + loginRepo.Truck);
				printer.PrintText ("Truck Mileage:  " + loginRepo.LoginResult.LastOdometerValue);
				printer.PrintText ("App Version:    " + buildInfo.Version + " (" + buildInfo.BuildNumber + ")");
				//printer.PrintText ("App Version:    " + buildInfo.BuildNumber + " (Build: " + buildInfo.Version + ")");
				printer.PrintText ("Webservice URL: " + clientFactory.Url);
				
				printer.SkipLines (5);
				printer.PrintData ();

				return true;
				
			}
			
			return false;
			
		}

		public static bool PrintShag(ILoginRepository loginRepo, List<VehicleViewModel> vehicles, List<DatsLocation> locations){
			printer = DependencyService.Get<IPrinter> ();

			if (printer.IsPrinterAvailable ()) {
				printer.Clear ();
				DatsLocation origin, destination;

				printer.PrintText ("Shag Summary", true);
                PrintHeader (printer, loginRepo);

				printer.SkipLines (1);

				printer.PrintText ("Truck Num:      " + loginRepo.Truck);

				List<VehicleViewModel> vehicleList;

				List<int> pickupID = 
					(from x in vehicles
					 select x.DatsVehicle.PickupLocationId).Distinct ().ToList ();

				foreach (int pickup in pickupID) {

					origin = locations.FirstOrDefault (z => z.Id == pickup);

					List<int> dropoffID = 
						(from x in vehicles
						 where x.DatsVehicle.PickupLocationId == pickup
						 select x.DatsVehicle.DropoffLocationId).Distinct ().ToList ();

					foreach (int dropoff in dropoffID) {

						destination = locations.FirstOrDefault (z => z.Id == dropoff);

						vehicleList = 
							(from x in vehicles
						  where x.DatsVehicle.PickupLocationId == pickup && x.DatsVehicle.DropoffLocationId == dropoff
						  select x).ToList ();

						if (vehicleList.Count == 0) {
							continue;
						}

						printer.SkipLines (2);
							
						PrintAddress (origin, destination);

						printer.SkipLines (1);

						printer.PrintText ("VIN                                                               BAY");
						foreach (VehicleViewModel v in vehicleList) {
							string bay = String.IsNullOrEmpty (v.DatsVehicle.BayLocation) ? "" : v.DatsVehicle.BayLocation;
							printer.PrintText (DetermineWhiteSpaceBetweenHeader (v.VIN, bay), true);
							printer.PrintText ("Load Number: " + v.DatsVehicle.LoadNumber + "     " + v.DatsVehicle.Model + "      " + v.DatsVehicle.Color);
							foreach (DamageViewModel d in v.LoadingDamageList) {
								printer.PrintText (d.DamageCode + ": " + d.DamageInfo, false, true, false, 0);
							}
							printer.SkipLines (1);
						}
					}
					printer.SkipLines (5);
				}

				printer.PrintData ();
				return true;
			} else {
				return false;
			}


		}
			
		public static Paper GenerateReceipt(ILoginRepository loginRepo, CustomObservableCollection<VehicleViewModel> vehicles, List<DatsLocation> locations, 
			string selectedLocation, string customername, string driverfilepath, string customerfilepath, DeliveryInfo info){
			printer = DependencyService.Get<IPrinter> ();

			printer.Clear ();

			List<int> receiptIndex = new List<int> ();

			DatsLocation origin, destination;

			List<VehicleViewModel> vehicleList;

			string loadnumber;

			List<int> runID = 
				(from x in vehicles 
					select x.DatsVehicle.LegRunId).Distinct ().ToList ();

			foreach (int id in runID) {

				List<int> customerID = 
					(from x in vehicles 
						where x.DatsVehicle.LegRunId == id
						select x.DatsVehicle.CustomerId).Distinct ().ToList ();

				foreach (int customer in customerID) {

					List<int> pickupID = 
						(from x in vehicles 
							where x.DatsVehicle.LegRunId == id && x.DatsVehicle.CustomerId == customer
							select x.DatsVehicle.PickupLocationId).Distinct ().ToList ();

					foreach (int pickup in pickupID) {

						List<int> dropoffID= 
							(from x in vehicles 
								where x.DatsVehicle.LegRunId == id && x.DatsVehicle.CustomerId == customer && 
								      x.DatsVehicle.PickupLocationId == pickup
								select x.DatsVehicle.OriginalDropoffLocation).Distinct ().ToList ();

						foreach (int dropOffLocationID in dropoffID) {

							origin = locations.Find (z => z.Id == pickup);
							destination = locations.Find (z => z.Id == dropOffLocationID);

							vehicleList = 
								(from x in vehicles 
									where x.DatsVehicle.LegRunId == id && x.DatsVehicle.CustomerId == customer && 
									x.DatsVehicle.PickupLocationId == pickup && x.DatsVehicle.OriginalDropoffLocation == dropOffLocationID &&
									(x.Status == "Delivered" || x.Status == "Delivering") orderby x.DatsVehicle.LoadNumber ascending
									select x).ToList();

							if (vehicleList.Count== 0) {
								continue;
							}

							loadnumber = "";
								
							printer.PrintText ("Delivery Receipt", true);
							PrintHeader (printer, loginRepo);

							printer.PrintText ("Driver Name:    " + loginRepo.LoginResult.FullName); 
							printer.PrintText ("Driver ID:      " + loginRepo.LoginResult.Driver); 
							printer.PrintText ("Truck Num:      " + loginRepo.Truck);

							PrintAddress (origin, destination);
															
							foreach (VehicleViewModel v in vehicleList) {

								if (loadnumber != v.DatsVehicle.LoadNumber) {

									loadnumber = v.DatsVehicle.LoadNumber;

									printer.SkipLines (1);
									printer.PrintText ("Load Number: " + loadnumber);
								}

								printer.SkipLines (1);
								if (v.DatsVehicle.PickupDate != null)
								{
									printer.PrintText("Date Loaded: " + v.DatsVehicle.PickupDate.Value.ToString("d"));
								}
								printer.SkipLines (1);

								printer.PrintText (v.VIN, true);
								string desc = v.DatsVehicle.VehicleYear + " " + v.DatsVehicle.Make + " " + v.DatsVehicle.Model;

								printer.PrintText (desc, false, true, true, 0);

								List<string> dmg = v.DatsVehicle.DeliveryInspectionDamageCodes.SplitCorrectly(',').ToList();

								foreach(string d in dmg){
									if (d.Length > 0) {
										var dm = new DamageViewModel (d, InspectionType.Delivery);
										printer.PrintText (dm.DamageCode + ": " + dm.DamageInfo, false, true, false, 0);
									}
								}

                                string notes = v.DatsVehicle.DropOffInspectionNotes;

								if (!string.IsNullOrEmpty(notes))
								{
									printer.SkipLines(1);
									printer.PrintText(notes, false, true, true);
								}
							}
								
							printer.SkipLines (2);

							printer.PrintText ("DELIVERY INFO:", false, true, true);
							printer.PrintText ("Delivery Attended:  "  + (info.Attended ? "Yes" : "No"));
							printer.PrintText ("Delivery STI:       "  + (info.LoadInspection ? "Yes" : "No"));
							if(info.Attended && info.LoadInspection)
								printer.PrintText ("Delayed Reason:     "  + info.Reason);

							if(appSettingsRepository.DeliveryTimeStamp == 0){
								printer.PrintText ("Delivery Date:      "  + DateTime.Now.ToString("d"));
							}
							else{
								printer.PrintText ("Delivery Date:      "  + DateTime.Now.ToString());
							}

							if (!string.IsNullOrEmpty(info.Comment))
							{
								printer.PrintText(info.Comment);
							}

							printer.SkipLines(2);
							printer.PrintText ("Driver:                           Dealer:", false, true, true);
							printer.PrintSignature (driverfilepath, true);
							printer.PrintSignature (customerfilepath, false);

							printer.SkipLines (1);

							printer.PrintText (loginRepo.LoginResult.FullName + DetermineWhiteSpaceBetweenName(loginRepo.LoginResult.FullName, 34) + customername, false, true, true);

							printer.SkipLines (2);

							printer.PrintText ("All damages/shortages discovered on vehicles dropped after hours or ");
							printer.PrintText ("Subject to Inspection (STI) must be reported within 48 hours of ");
							printer.PrintText ("delivery.  Pictures must be sent to support all claims.");

							printer.SkipLines (5);

							receiptIndex.Add (printer.Length() - 1);
					

						}
					}
				}
			}
				
			var p = new Paper {
				Id = DateTime.Now.GetHashCode().ToString(),
				Location = selectedLocation,
				Time = DateTime.Now,
				Data = printer.GetData (),
				Offsets = receiptIndex.SelectMany(BitConverter.GetBytes).ToArray()
			};

			return p;

		}
			
		private static void PrintAddress(DatsLocation origin, DatsLocation destination){

			string pName, pAddressLine1, pCity, pState, pZip, pAddressLine2;
			string dName, dAddressLine1, dCity, dState, dZip, dAddressLine2;

			string pAddressTop = "", pAddressBottom = "", dAddressTop = "", dAddressBottom = "";
			string pNameTop="", pNameBottom="", dNameTop="", dNameBottom="";

			int index = 0;

			if (origin == null) {
				pName = ""; pAddressLine1 = ""; pCity = ""; pState = ""; pZip = ""; pAddressLine2 = "";
			}else {
                pNameTop = pName = StringChecker(origin.DisplayName); 
                pAddressTop = pAddressLine1 = StringChecker(origin.AddressLine1); 
                pCity = StringChecker(origin.City); 
                pState = StringChecker(origin.State); 
                pZip = StringChecker(origin.Zip); 
                pAddressLine2 = StringChecker(origin.AddressLine2);

			}

			if (destination == null) {
				dName = ""; dAddressLine1 = ""; dCity = ""; dState = ""; dZip = ""; dAddressLine2 = "";
			}else {
                dNameTop = dName = StringChecker(destination.DisplayName); 
                dAddressTop = dAddressLine1 = StringChecker(destination.AddressLine1); 
                dCity = StringChecker(destination.City); 
                dState = StringChecker(destination.State); 
                dZip = StringChecker(destination.Zip); 
                dAddressLine2 = StringChecker(destination.AddressLine2);
			}

			if (pName.Length > 30) {
				index = TrimmingIndex (pName);
				pNameTop = pName.Substring (0, index);
				pNameBottom = pName.Substring (index);
			}

			if (dName.Length > 30) {
				index = TrimmingIndex (dName);
				dNameTop = dName.Substring (0, index);
				dNameBottom = dName.Substring (index);
			}

			if (pAddressLine1.Length > 30) {
				index = TrimmingIndex (pAddressLine1);
				pAddressTop = pAddressLine1.Substring (0, index);
				pAddressBottom = pAddressLine1.Substring (index);
			}

			if (dAddressLine1.Length > 30) {
				index = TrimmingIndex (dAddressLine1);
				dAddressTop = dAddressLine1.Substring (0, index);
				dAddressBottom = dAddressLine1.Substring (index);
			}

			printer.SkipLines (2);
			printer.PrintText ("Origin:" + DetermineWhiteSpaceBetweenName("Origin:") + "Destination:", false, true, true);

			if (pNameBottom.Length > 0 || dNameBottom.Length > 0) {
				printer.PrintText (pNameTop + DetermineWhiteSpaceBetweenName(pNameTop) + dNameTop);
				printer.PrintText (pNameBottom + DetermineWhiteSpaceBetweenName(pNameBottom) + dNameBottom);
			} else {
				printer.PrintText (pName + DetermineWhiteSpaceBetweenName(pName) + dName);
			}

			if (pAddressBottom.Length > 0 || dAddressBottom.Length > 0) {
				printer.PrintText (pAddressTop + DetermineWhiteSpaceBetweenName(pAddressTop) + dAddressTop);
				printer.PrintText (pAddressBottom + DetermineWhiteSpaceBetweenName(pNameBottom) + dAddressBottom);
			} else {
				printer.PrintText (pAddressLine1 + DetermineWhiteSpaceBetweenName(pAddressLine1) + dAddressLine1);
			}

			string originDetail = pCity + ", " + pState + " " + pZip;
			string destDetail   = dCity + ", " + dState + " " + dZip;

			if (pAddressLine2.Length > 0 && dAddressLine2.Length > 0) {
				printer.PrintText (pAddressLine2 + DetermineWhiteSpaceBetweenName (pAddressLine2) + dAddressLine2);
				printer.PrintText (originDetail + DetermineWhiteSpaceBetweenName (originDetail) + destDetail);
			} else if (pAddressLine2.Length > 0 && String.IsNullOrEmpty (dAddressLine2)) {
				printer.PrintText (pAddressLine2 + DetermineWhiteSpaceBetweenName (pAddressLine2) + destDetail);
				printer.PrintText (originDetail);
			} else if (String.IsNullOrEmpty (pAddressLine2) && dAddressLine2.Length > 0) {
				printer.PrintText (originDetail + DetermineWhiteSpaceBetweenName (originDetail) + dAddressLine2);
				printer.PrintText ("" + DetermineWhiteSpaceBetweenName ("") + destDetail);
			} else {
				printer.PrintText (originDetail + DetermineWhiteSpaceBetweenName (originDetail) + destDetail);
			}

		}

        private static string StringChecker(string str) {
            if (String.IsNullOrEmpty(str)) {
                return "";
            }

            return str;
        }

		private static int TrimmingIndex(string str, int GridLength = 30){

			int index = 0;
			string[] words = str.Split(' ');
			foreach (string word in words)
			{
				if (index + word.Length + 1  <= 30)
					index += word.Length + 1;
				else
					break;	
			}

			return index;

		}

		private static string DetermineWhiteSpaceBetweenHeader(string left, string right, int space = 17){

			int whiteSpaceCount = space - right.Length;

            if (whiteSpaceCount < 0) {
                whiteSpaceCount = 1;
            }

			return left + new String (' ', whiteSpaceCount) + right;
		}


		//determine the white space necessary to align names properly
		private static string DetermineWhiteSpaceBetweenName(string leftText, int midPoint = 35){
			if (leftText.Length > midPoint)
				leftText = leftText.Substring (0, midPoint - 1);

			int whiteSpaceCount = midPoint - leftText.Length;

			return  new String(' ', whiteSpaceCount);

		}

		public static void PrintCacheData(){
			printer.PrintData ();
		}

	}
}


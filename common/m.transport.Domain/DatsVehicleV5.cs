using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using SQLite;
using System.Linq;
using m.transport.Utilities;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DatsVehicleV5 : IHaveId<string>
	{
		[DataMember]
		public string VIN { get; set; }
		[DataMember]
		public string VinKey { get; set; }
		[DataMember]
		public string LegStatus { get; set; }
		[DataMember]
		public string LoadNumber { get; set; }
		[DataMember]
		public string LoadStatus { get; set; }
		[DataMember]
		public int VehicleId { get; set; }
		[DataMember]
		public string VehicleDescription { get; set; }
		[DataMember]
		public string BayLocation { get; set; }
		[DataMember]
		public int LegId { get; set; }
		[DataMember]
		public int RunId { get; set; }
		[DataMember]
		public int LoadId { get; set; }
		[DataMember]
		public int? ExceptionInd { get; set; }
		[DataMember]
		public int VehicleDelivered { get; set; }
		[DataMember]
		public int LegStatusCode { get; set; }
		[DataMember]
		public int LegRunId { get; set; }
		[DataMember]
		public string LegRunStatus { get; set; }
		[DataMember]
		public int PickupLocationId { get; set; }
		[DataMember]
		public string PickupLocationName { get; set; }
		[DataMember]
		public DateTime? PickupDate { get; set; }
		[DataMember]
		public int DropoffLocationId { get; set; }
		[DataMember]
		public string DropoffLocationName { get; set; }
		[DataMember]
		public DateTime? DropoffDate { get; set; }
		[DataMember]
		public bool? ShagUnitInd { get; set; }
		[DataMember]
		public int PoolId { get; set; }
		[DataMember]
		public string LocationType { get; set; }
		[DataMember]
		public string DamageCodeList { get; set; }
		[DataMember]
		public int LoadInspectionId { get; set; }
		[DataMember]
		public DateTime? LoadInspectionTimestamp { get; set; }
		[DataMember]
		public string LoadInspectionDamageCodes { get; set; }
		[DataMember]
		public int DeliveryInspectionId { get; set; }
		[DataMember]
		public string DeliveryInspectionDamageCodes { get; set; }
		[DataMember]
		public DateTime? DeliveryInspectionTimestamp { get; set; }
		[DataMember]
		public bool? AttendedInd { get; set; }
		[DataMember]
		public bool? SubjectToInspectionInd { get; set; }
		[DataMember]
		public int ExceptionId { get; set; }
		[DataMember]
		public int ExceptionFlag { get; set; }
		[DataMember]
		public int ExceptionCode { get; set; }
		[DataMember]
		public bool? ReviewedInd { get; set; }
		[DataMember]
		public string PhoneMessage { get; set; }
		[DataMember]
		public string ExceptionMessage { get; set; }
		[DataMember]
		public int LoadStatusCode { get; set; }
		[DataMember]
		public int LoadDriverId { get; set; }
		[DataMember]
		public string LoadDriverNumber { get; set; }
		[DataMember]
		public int LoadRunId { get; set; }
		[DataMember]
		public string LoadRunStatus { get; set; }
		[DataMember]
		public int LoadRunStatusCode { get; set; }
		[DataMember]
		public string VehicleStatus { get; set; }
		[DataMember]
		public int? VStatus { get; set; }
		[DataMember]
		public bool? ReservedByDriverInd { get; set; }
		[DataMember]
		public int ReservedByDriverId { get; set; }
		[DataMember]
		public int LegRunStatusCode { get; set; }
		[DataMember]
		public int CustomerId { get; set; }
		[DataMember]
		public int OrderId { get; set; }
		[DataMember]
		public string VehicleYear { get; set; }
		[DataMember]
		public string Make { get; set; }
		[DataMember]
		public string Model { get; set; }
		[DataMember]
		public string BodyStyle { get; set; }
		[DataMember]
		public string Color { get; set; }
		[DataMember]
		public string VehicleLength { get; set; }
		[DataMember]
		public string VehicleWidth { get; set; }
		[DataMember]
		public string VehicleHeight { get; set; }
		[DataMember]
		public string VehicleLocation { get; set; }
		[DataMember]
		public string CustomerIdentification { get; set; }
		[DataMember]
		public string SizeClass { get; set; }
		[DataMember]
		public string RailCarNumber { get; set; }
		[DataMember]
		public bool? PriorityInd { get; set; }
		[DataMember]
		public string HaulType { get; set; }
		[DataMember]
		public DateTime? AvailableForPickupDate { get; set; }
		[DataMember]
		public bool? VinDecodedInd { get; set; }
		[DataMember]
		public string RecordStatus { get; set; }
		[DataMember]
		public DateTime? CreationDate { get; set; }
		[DataMember]
		public string CreatedBy { get; set; }
		[DataMember]
		public DateTime? UpdatedDate { get; set; }
		[DataMember]
		public string UpdatedBy { get; set; }

		[DataMember]
		public string Status { get; set; }
		[DataMember]
		public int? ProcessInd { get; set; }

		[DataMember]
		public int? AssignedLoadId { get; set; }
		[DataMember]
		public int? ValidTRCode { get; set; }
		[DataMember]
		public string TCode { get; set; }
		[DataMember]
		public string RCode { get; set; }
		[DataMember]
		public bool? OldInd { get; set; }
		[DataMember]
		public string PickupCity { get; set; }
		[DataMember]
		public string PickupState { get; set; }
		[DataMember]
		public string PickupDeliveryTimes { get; set; }
		[DataMember]
		public string DropoffCity { get; set; }
		[DataMember]
		public string DropoffState { get; set; }
		[DataMember]
		public string DropoffDeliveryTimes { get; set; }
		[DataMember]
		public string PickupAddressLine1 { get; set; }
		[DataMember]
		public string PickupAddressLine2 { get; set; }
		[DataMember]
		public bool? ReloadInd { get; set; }
		[DataMember]
		public string YardLocation { get; set; }
		[DataMember]
		public string SignedBy { get; set; }
		[DataMember]
		public string RefusedSignCode { get; set; }
		[DataMember]
		public int DamagePhotoRequiredInd { get; set; }
		[DataMember]
		public int HotLoadInd { get; set; }
		[DataMember]
		public string DeliveryNotes { get; set; }
		[DataMember]
		public string PickupInspectionNotes { get; set; }
		[DataMember]
		public string DropOffInspectionNotes { get; set; }
		[DataMember]
		public int DropoffBillOfLadingID { get; set; }
		[DataMember]
		public int PickupBillOfLadingID { get; set; }
		[DataMember]
		public int SafeDeliveryPromptRequiredInd { get; set; }
		[DataMember]
		public string UnsafeDeliveryResponse { get; set; }
		[DataMember]
		public string UnsafeDeliveryNotes { get; set; }
		[DataMember]
		public string ExpectedPhotoCount { get; set; }
		[DataMember]
		public string LogData { get; set; }
		[DataMember]
		public int IsDriverTracked { get; set; }
		[DataMember]
		public int IsShipmentTrackedByVehicle { get; set; }
		[DataMember]
		public int IsShipmentTrackedByLoad { get; set; }
		[DataMember]
		public string CustomerName { get; set; }

		public bool HasDropOffInspectionNotes
		{
			get { return !string.IsNullOrEmpty(DropOffInspectionNotes); }
		}

		[PrimaryKey]
		public string Id
		{
			get { return VehicleId + "|" + LegId; }
			set
			{
				var parts = value.Split('|');
				if (!string.IsNullOrWhiteSpace(parts[0]))
				{
					VehicleId = Convert.ToInt32(parts[0]);
				}
				if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
				{
					LegId = Convert.ToInt32(parts[1]);
				}
			}
		}

		[IgnoreDataMember]
		public int OriginalDropoffLocation { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", VIN, Make, Model);
		}

		public string ToDebugString(bool includeDamages, bool includeLocations, bool includeExceptions)
		{
			var builder = new StringBuilder();
			builder.AppendFormat("{0} {1} {2}", VIN, Make, Model);

			builder.AppendFormat(", VehicleStatus={0},VStatus={1},Status={2}", VehicleStatus, this.GetVStatusString(), this.GetStatusString());
			if (includeDamages)
			{
				builder.AppendFormat(", Damages:[{0}] [{1}] [{2}]",
					DamageCodeList, LoadInspectionDamageCodes, DeliveryInspectionDamageCodes);
			}
			if (includeLocations)
			{
				builder.AppendFormat(", Locations: P={0}({1}) D={2}({3})",
					PickupLocationName, PickupLocationId, DropoffLocationName, DropoffLocationId);
			}
			if (includeExceptions)
			{
				builder.AppendFormat(", Exception? {0} C={1} F={2} Id={3}",
					ExceptionInd, ExceptionCode, ExceptionFlag, ExceptionId);
			}
			//builder.AppendLine(this.Serialize());
			return builder.ToString();
		}

		private string tempInspectionDamageCodes = string.Empty;

		public string TempInspectionDamageCodes
		{
			get
			{
				return tempInspectionDamageCodes;
			}
			set
			{
				tempInspectionDamageCodes = value;
			}
		}

		public string AddInspectionDamageCode(string damageCode)
		{
			return tempInspectionDamageCodes =
				CombineDamageCodes(tempInspectionDamageCodes, damageCode);
		}

		public void RemoveInspectionDamageCode(string damageCode)
		{
			var codes = tempInspectionDamageCodes.SplitCorrectly(',').ToList();
			codes.Remove(damageCode);
			tempInspectionDamageCodes = string.Join(",", codes);
		}

		public void SaveTempDamages(InspectionType type)
		{
			if (type == InspectionType.Delivery)
			{
				DeliveryInspectionDamageCodes =
					CombineDamageCodes(DeliveryInspectionDamageCodes, tempInspectionDamageCodes);
				DeliveryInspectionTimestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
			}
			else if (type == InspectionType.Loading)
			{
				LoadInspectionDamageCodes =
					CombineDamageCodes("", tempInspectionDamageCodes);
				LoadInspectionTimestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
			}
			//ClearTempDamages();
		}

		public void ClearDamage(InspectionType type)
		{
			if (type == InspectionType.Delivery)
			{
				DeliveryInspectionDamageCodes = String.Empty;
			}
			else if (type == InspectionType.Loading)
			{
				LoadInspectionDamageCodes = String.Empty;
			}

		}

		public void ClearTempDamages()
		{
			tempInspectionDamageCodes = string.Empty;
		}

		public string HotIndToString()
		{

			switch (HotLoadInd)
			{
				case 1:
					return "HOT";
				case 2:
				case 0:
				default:
					return "";

			}
		}

		public IEnumerable<string> GetInspectionDamageCodes(InspectionType inspectionType)
		{
			string result = string.Empty;

			switch (inspectionType)
			{
				case InspectionType.Loading:
					result = LoadInspectionDamageCodes;
					break;
				case InspectionType.Delivery:
					result = DeliveryInspectionDamageCodes;
					break;
				case InspectionType.Origin: // PreExisting
					result = DamageCodeList;
					break;
				case InspectionType.Temp:
					result = tempInspectionDamageCodes;
					break;
			}

			return result.SplitCorrectly(',');
		}


		//Returns true if this vehicle has damages prior to driver pickup...
		public bool HasPreExistingDamages
		{
			get
			{
				return DamageCodeList.SplitCorrectly(',')
					.Except(LoadInspectionDamageCodes.SplitCorrectly(','))
					.Except(DeliveryInspectionDamageCodes.SplitCorrectly(','))
					.Any();
			}
		}

		public string PreExisitingString
		{
			get
			{
				return HasPreExistingDamages ? "D" : " ";
			}

		}


		private string CombineDamageCodes(string currentDamageCodes, string codesToAdd)
		{
			if (string.IsNullOrEmpty(currentDamageCodes))
			{
				return codesToAdd;
			}
			if (string.IsNullOrEmpty(codesToAdd))
			{
				return currentDamageCodes;
			}
			return string.Join(",", currentDamageCodes, codesToAdd);
		}

	}
}

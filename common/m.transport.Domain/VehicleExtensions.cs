using System;

namespace m.transport.Domain
{
	public static class VehicleExtensions
	{
		public static void SetVehicleStatus(this DatsVehicleV4 v, VehicleStatus status)
		{
			v.VehicleStatus = status.ToString();
			v.Status = ((int)status).ToString();

			if (status == VehicleStatus.Delivering)
			{
				v.DropoffDate = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Unspecified);
				v.DeliveryInspectionTimestamp = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Unspecified);
			}

			if (status == VehicleStatus.Loading)
			{
				v.PickupDate = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Unspecified);
				v.LoadInspectionTimestamp = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Unspecified);
			}

			v.MapVehicleStatusToLegStatus(status);
		}

		public static void MapVehicleStatusToLegStatus(this DatsVehicleV4 v, VehicleStatus status)
		{

			if (status == VehicleStatus.Assigned)
			{
				v.LegStatus = LegStatus.Assigned.ToString();
				v.LegStatusCode = (int)LegStatus.Assigned;
			}

			if (status == VehicleStatus.Loading)
			{
				v.LegStatus = LegStatus.EnRoute.ToString();
				v.LegStatusCode = (int)LegStatus.EnRoute;
			}
		}

		public static VehicleStatus FixStatusEnum(this DatsVehicleV4 vehicle)
		{
			if (vehicle.VehicleStatus == "Available")
			{
				vehicle.SetVehicleStatus(VehicleStatus.Assigned);
			}
			if (vehicle.VehicleStatus == "Pending")
			{
				vehicle.SetVehicleStatus(VehicleStatus.Assigned);
			}
			if (vehicle.VehicleStatus == "EnRoute")
			{
				vehicle.SetVehicleStatus(VehicleStatus.Loaded);
			}

			// 5324 - LegStatus should override VehicleStatus in cases where they conflict
			if (vehicle.LegStatus == "Assigned")
			{
				vehicle.SetVehicleStatus(VehicleStatus.Assigned);
			}
			VehicleStatus vehicleStatusEnum;
			Enum.TryParse(vehicle.VehicleStatus, out vehicleStatusEnum);
			return vehicleStatusEnum;
		}

		public static string GetStatusString(this DatsVehicleV4 vehicle)
		{
			if (String.IsNullOrWhiteSpace(vehicle.Status))
				return vehicle.Status;

			int s;
			var parsed = int.TryParse(vehicle.Status, out s);

			return parsed ? ((VehicleStatus)s).ToString() : string.Empty;
		}

		public static string GetVStatusString(this DatsVehicleV4 vehicle)
		{
			if (vehicle.VStatus == null)
			{
				return "";
			}
			switch (vehicle.VStatus)
			{
				case 0:
					return "Available";
				case 1:
					return "Pending";
				case 2:
					return "EnRoute";
				case 3:
					return "Delivered";
				case 4:
					return "Damaged";
			}
			return vehicle.VStatus.ToString();
		}
	}
}
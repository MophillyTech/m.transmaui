using System;

namespace m.transport.Domain
{
	public enum VehicleStatus
	{
		New = 0,
		Inventorying = 1,
		Inventoried = 2,
		Locating = 3,
		Located = 4,
		Inspecting = 5,
		Inspected = 6,
		Assigned = 7,
		Reserved = 8,
		NotLoaded = 9,
		Loading = 10,
		Loaded = 11,
		Delivering = 12,
		Delivered = 13,
		Removing = 14,
		Removed = 15,
	}
}

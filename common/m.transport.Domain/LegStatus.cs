using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Domain
{
	public enum LegStatus
	{
		Assigned = 58,
		Available = 63,
		Complete = 62,
		Delivered = 424,
		EnRoute = 61, 
		InLoad = 64,
		OnHold = 1312,
		Pending = 55,
		PendingPrevLeg = 56,
		PendingRepair = 57,
		Scheduled = 59,
		ScheduledAssigned = 60
	}
}

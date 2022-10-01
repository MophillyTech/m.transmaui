using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Domain
{
	public enum InspectionType
	{
		Origin = 0,
		Yard = 1,
		Loading = 2,
		Delivery = 3,
		Other = 4,
		Port = 5,
		PostDelivery = 6,
		Marine = 7,
		AccessoryShopDamage = 8,
		ThirdParty = 9,
		Temp = 10, // Note this type does not exist on the server side
	}

}

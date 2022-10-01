using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m.transport.Domain;
using m.transport.Interfaces;

namespace m.transport.Utilities
{
	public static class SignatureExtensions
	{
		public static DeliverySignature ToDeliverySignature(this Signature signature)
		{
			return new DeliverySignature
			{
				FileName = signature.Filename,
				SignatureData = signature.Bytes,
			};
		}
	}
}

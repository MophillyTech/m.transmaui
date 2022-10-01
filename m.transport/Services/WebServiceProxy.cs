using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m.transport.Asmx;

namespace m.transport.Services
{
	public class WebServiceProxy
	{
		private ServiceSoapClient _client;

		public WebServiceProxy()
		{
			_client = CreateService();
		}

		private static ServiceSoapClient CreateService()
		{
			return new ServiceSoapClient();
		}
	}
}

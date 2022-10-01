using System;
using m.transport.Interfaces;

namespace m.transport.Services
{
	public class ServiceClientFactory<T> : IServiceClientFactory<T>
		where T : class
	{
		public ServiceClientFactory()
		{
		}

		public Func<string, T> CreateFunc { get; set; }

		public string Url
		{
			get { return url; }
			set
			{
				url = value;
				client = null;
			}
		}

		private T client;
		private string url;

		public T Instance
		{
			get
			{
				if (client == null)
				{
					client = CreateFunc(url);
				}
				return client;
			}
			set { client = value; }
		}
	}
}

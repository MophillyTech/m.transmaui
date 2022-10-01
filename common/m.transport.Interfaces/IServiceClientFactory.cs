using System;

namespace m.transport.Interfaces
{
	public interface IServiceClientFactory<T>
	{
		string Url { get; set; }
		Func<string, T> CreateFunc { get; set; }
		T Instance { get; set; }
	}
}
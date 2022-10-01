using System;
using System.Threading.Tasks;

namespace m.transport.Interfaces
{
	public interface IBarcode
	{
		void Scan(Action<string> onRead, Action<Exception> onError = null);
	}
}


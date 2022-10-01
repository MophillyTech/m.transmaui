using System;
using System.Threading.Tasks;

namespace m.transport.Interfaces
{
	public interface ICamera
	{
		void TakePhoto(Action<string> onRead, Action<Exception> onError = null, Action onCancel = null);
		bool CheckPermission();
	}
}


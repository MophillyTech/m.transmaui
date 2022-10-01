using System;
using System.Threading.Tasks;

namespace m.transport.Interfaces
{
	public interface IAlert
	{
        void ShowAlert(string title, string msg, string[] items, Action<string> onSelect);
		void ShowAlert(string title, string msg, Action onSelect);
	}
}


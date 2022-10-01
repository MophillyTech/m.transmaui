using System;

namespace m.transport.Interfaces
{
	public interface IHud
	{
		void Show();
		void Show(string message);
		void ShowToast(string message);
		void Dismiss();
	}
}


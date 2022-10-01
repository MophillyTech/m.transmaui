using System;

namespace m.transport.Interfaces
{
	public interface ILogger
	{
		void Log (string message);
		string ReadLog();
		void ClearLog();
		void CopyLog();
	}
}


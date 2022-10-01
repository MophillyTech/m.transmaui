using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m.transport.Interfaces
{
	public interface ILoadResource
	{
		string ResourcePrefix { get; }
		Stream LoadStream(string resourceName);
		byte[] LoadBytes(string resourceName);
	}
}

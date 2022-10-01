using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m.transport.Interfaces;

namespace m.transport.Data
{
	public interface IDriverSignatureRepository
	{
		Signature DriverSignature { get; set; }

		void LoadAsync();
		event EventHandler<AsyncCompletedEventArgs> LoadCompleted;
		void SaveAsync(int runId, int dropoffLocationId, int[] legIds);
		event EventHandler<AsyncCompletedEventArgs> SaveCompleted;
		void GetCachedSignature();
	}
}

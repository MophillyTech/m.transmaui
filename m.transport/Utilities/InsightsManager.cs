using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin;

namespace m.transport.Utilities
{
	public static class InsightsManager
	{
#if DEBUG
		[Conditional("FALSE")]
#endif
		public static void Track(string log)
		{
			//Insights.Track(log);
		}

		public static IDisposable TrackTime(string log)
		{
#if DEBUG
			return FakeTrackTime(log);
#else
			return RealTrackTime(log);
#endif
		}


		public static FakeDisposable FakeTrackTime(string log)
		{
			return new FakeDisposable(log);
		}

		public static ITrackHandle RealTrackTime(string log)
		{
			return Insights.TrackTime(log);
		}
	}

	public class FakeDisposable : IDisposable
	{
		private readonly string log;
		private DateTime startTime;

		public FakeDisposable(string log)
		{
			this.log = log;
			startTime = DateTime.Now;
			Debug.WriteLine(log + " starting " + startTime);
		}

		public void Dispose()
		{
			var endTime = DateTime.Now;
			Debug.WriteLine(log + " ending {0} took {1}", endTime, endTime-startTime);
		}
	}
}

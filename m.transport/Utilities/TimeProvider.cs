using System;

namespace m.transport.Utilities
{
	public abstract class TimeProvider
	{
		private static TimeProvider current = DefaultTimeProvider.Instance;

		public static TimeProvider Current
		{
			get { return current; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				current = value;
			}
		}

		public abstract DateTime UtcNow { get; }
		public abstract DateTime Now { get; }

		public static void ResetToDefault()
		{
			current = DefaultTimeProvider.Instance;
		}
	}

	public class DefaultTimeProvider : TimeProvider
	{
		private static TimeProvider instance;

		public static TimeProvider Instance
		{
			get { return instance ?? (instance = new DefaultTimeProvider()); }
		}

		private DefaultTimeProvider()
		{

		}

		public override DateTime UtcNow
		{
			get { return DateTime.UtcNow; }
		}
		public override DateTime Now
		{
			get { return DateTime.Now; }
		}
	}
}

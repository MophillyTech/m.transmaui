using System;
using Foundation;
using CoreFoundation;

namespace m.transport.iOS
{
	public static class DateExtensions
	{

		public static DateTime NSDateToDateTime(this NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime (
				                   new DateTime (2001, 1, 1, 0, 0, 0));
			
			return reference.AddSeconds (date.SecondsSinceReferenceDate);
		}

		public static NSDate DateTimeToNSDate(this DateTime date)
		{
			if (date.Kind == DateTimeKind.Unspecified) {
				// DateTimeKind.Local or DateTimeKind.Utc, this depends on each app 
				date = DateTime.SpecifyKind (date, DateTimeKind.Utc); 
			}

			return (NSDate)date;
		}
	}
}
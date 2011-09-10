using System;

namespace CrmDynamicsOnline.Sample
{
	public static class DateTimeExtensions
	{
		public static DateTime NSDateToDateTime(this MonoTouch.Foundation.NSDate date)
		{
			return (new DateTime(2001,1,1,0,0,0)).AddSeconds(date.SecondsSinceReferenceDate);
		}

		public static MonoTouch.Foundation.NSDate DateTimeToNSDate(this DateTime date)
		{
		    return MonoTouch.Foundation.NSDate.FromTimeIntervalSinceReferenceDate((date-(new DateTime(2001,1,1,0,0,0))).TotalSeconds);
		}
	}
}




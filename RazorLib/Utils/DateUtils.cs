using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLib.Utils
{
    public class DateUtils
    {
        public static int TimeInYears(DateTime start)
        {
            var end = DateTime.UtcNow;
            return end.Year - start.Year;
        }

        public static (int hours, int minutes) TimeInHoursMinutes(DateTime start, DateTime? end)
        {
            if (end == null)
            {
                end = DateTime.UtcNow;
            }
            var secondsStart = SecondsSinceBeginning(start);
            var secondsEnd = SecondsSinceBeginning(end.Value);
            var secondsDiff = secondsEnd - secondsStart;

            int hours = (int) Math.Floor(secondsDiff / 60 / 60); // seconds to hours
            var remaining = secondsDiff - (hours * 60 * 60);
            int minutes = (int) Math.Floor(remaining / 60);

            return (hours, minutes);
        }

        private static double SecondsSinceBeginning(DateTime time)
        {
            TimeSpan span = time.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return span.TotalSeconds;
        }
    }
}

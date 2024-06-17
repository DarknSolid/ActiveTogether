using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLib.Utils
{
    public static class TimeSpanExtensions
    {
        public static string ToQueryParameterString(this TimeSpan value)
        {
            return new DateTime(value.Ticks).ToString("HH:mm");
        }

        public static TimeSpan FromQueryParameterString(this string str)
        {
            DateTime dt = DateTime.ParseExact(str, "HH:mm", CultureInfo.InvariantCulture);
            return new TimeSpan(hours: dt.Hour, minutes: dt.Minute, seconds:0);
        }
    }
}

using System;
using System.Globalization;

namespace SamplesForm.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime SpecifyTime(this DateTime me, int hour, int minute, int second)
        {
            return new DateTime(me.Year, me.Month, me.Day, hour, minute, second);
        }

        public static DateTime ToDateTime(this string me, string format)
        {
            return DateTime.ParseExact(me, format, CultureInfo.InvariantCulture);
        }
    }
}
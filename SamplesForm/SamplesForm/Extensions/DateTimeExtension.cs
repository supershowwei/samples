using System;

namespace SamplesForm.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime SpecifyTime(this DateTime me, int hour, int minute, int second)
        {
            return new DateTime(me.Year, me.Month, me.Day, hour, minute, second);
        }
    }
}
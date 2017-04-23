using System;
using System.Linq;

namespace SamplesForm.Extensions
{
    public static class StringExtension
    {
        public static string Left(this string me, int length)
        {
            length = Math.Max(length, 0);

            return me.Length > length ? me.Substring(0, length) : me;
        }

        public static bool NotEquals(this string me, string value)
        {
            return !me.Equals(value);
        }

        public static string Right(this string me, int length)
        {
            length = Math.Max(length, 0);

            return me.Length > length ? me.Substring(me.Length - length, length) : me;
        }

        public static string ToHtml(this string me)
        {
            return string.Join(
                string.Empty,
                me.ToCharArray().Select(c => c > 127 ? string.Concat("&#", (int)c, ";") : c.ToString()));
        }
    }
}
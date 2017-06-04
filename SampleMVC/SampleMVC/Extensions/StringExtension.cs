using System;

namespace SampleMVC.Extensions
{
    internal static class StringExtension
    {
        public static bool EqualsIgnoreCase(
            this string me,
            string value,
            StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return me != null && me.Equals(value, comparisonType);
        }
    }
}
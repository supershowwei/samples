using System;

namespace SamplesForm.Extensions
{
    public static class ObjectExtension
    {
        public static bool ExtEquals<T>(this T me, T other) where T : IEquatable<T>
        {
            if ((me == null) ^ (other == null)) return false;

            return me == null || me.Equals(other);
        }
    }
}
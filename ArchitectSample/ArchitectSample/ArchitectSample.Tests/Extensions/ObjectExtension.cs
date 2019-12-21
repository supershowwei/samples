using System;
using System.Linq.Expressions;

namespace ArchitectSample.Tests.Extensions
{
    internal static class ObjectExtension
    {
        public static Expression<Func<T>> Set<T>(this T obj, Expression<Func<T>> setter)
        {
            return setter;
        }

        public static (Expression<Func<T>>, Expression<Func<T, bool>>) Where<T>(this Expression<Func<T>> me, Expression<Func<T, bool>> predicate)
        {
            return (me, predicate);
        }
    }
}
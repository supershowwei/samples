using System;
using System.Linq.Expressions;

namespace ArchitectSample.Tests.Extensions
{
    internal static class ObjectExtension
    {
        public static (Expression<Func<T, bool>>, Expression<Func<T>>) ToStatement<T>(
            this T me,
            Expression<Func<T, bool>> predicate,
            Expression<Func<T>> setter)
        {
            return (predicate, setter);
        }

        public static (Expression<Func<T, bool>>, Expression<Func<T, object>>) ToStatement<T>(
            this T me,
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> selector)
        {
            return (predicate, selector);
        }
    }
}
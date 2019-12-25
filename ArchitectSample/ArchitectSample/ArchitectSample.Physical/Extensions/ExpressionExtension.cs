using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Physical;
using Chef.Extensions.Dapper;
using Chef.Extensions.IEnumerable;

namespace ArchitectSample.Physical.Extensions
{
    internal static class ExpressionExtension
    {
        public static string ToOrderExpression<T>(this IEnumerable<(Expression<Func<T, object>>, Sortord)> me)
        {
            return ToOrderExpression(me, string.Empty);
        }

        public static string ToOrderExpression<T>(this IEnumerable<(Expression<Func<T, object>>, Sortord)> me, string alias)
        {
            if (me.IsNullOrEmpty()) return string.Empty;

            return string.Concat(
                @"
ORDER BY ",
                string.Join(
                    ", ",
                    me.Select(
                        o =>
                            {
                                var (expr, sortord) = o;

                                return sortord == Sortord.Descending ? expr.ToOrderDescending(alias) : expr.ToOrderAscending(alias);
                            })));
        }
    }
}
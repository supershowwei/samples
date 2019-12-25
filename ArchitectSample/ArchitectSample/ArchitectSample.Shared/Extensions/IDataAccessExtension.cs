using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Shared.Extensions
{
    public static class IDataAccessExtension
    {
        public static QueryObject<T> Where<T>(this IDataAccess<T> me, Expression<Func<T, bool>> predicate)
        {
            return new QueryObject<T>(me) { Predicate = predicate };
        }

        public static QueryObject<T> Select<T>(this IDataAccess<T> me, Expression<Func<T, object>> selector)
        {
            return new QueryObject<T>(me) { Selector = selector };
        }

        public static QueryObject<T> Set<T>(this IDataAccess<T> me, Expression<Func<T>> setter)
        {
            return new QueryObject<T>(me) { Setter = setter };
        }

        public static QueryObject<T> OrderBy<T>(this IDataAccess<T> me, Expression<Func<T, object>> ordering)
        {
            return new QueryObject<T>(me)
                   {
                       OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Ascending) }
                   };
        }

        public static QueryObject<T> OrderByDescending<T>(this IDataAccess<T> me, Expression<Func<T, object>> ordering)
        {
            return new QueryObject<T>(me)
                   {
                       OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Descending) }
                   };
        }

        public static QueryObject<T> Top<T>(this IDataAccess<T> me, int n)
        {
            return new QueryObject<T>(me) { Top = n };
        }
    }
}
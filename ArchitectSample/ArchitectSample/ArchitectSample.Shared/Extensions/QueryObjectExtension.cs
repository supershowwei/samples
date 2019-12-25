using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Shared.Extensions
{
    public static class QueryObjectExtension
    {
        public static QueryObject<T> Where<T>(this QueryObject<T> me, Expression<Func<T, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T> Select<T>(this QueryObject<T> me, Expression<Func<T, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T> Set<T>(this QueryObject<T> me, Expression<Func<T>> setter)
        {
            me.Setter = setter;

            return me;
        }

        public static QueryObject<T> OrderBy<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T> ThenBy<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T> OrderByDescending<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T> ThenByDescending<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static Task<T> QueryOneAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryOneAsync(me.Predicate, me.Selector, me.OrderExpressions);
        }

        public static Task<List<T>> QueryAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryAsync(me.Predicate, me.Selector, me.OrderExpressions);
        }

        public static Task UpdateAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.UpdateAsync(me.Predicate, me.Setter);
        }

        public static (Expression<Func<T, bool>>, Expression<Func<T>>) ToStatement<T>(
            this T me,
            Expression<Func<T, bool>> predicate,
            Expression<Func<T>> setter)
        {
            return (predicate, setter);
        }
    }
}
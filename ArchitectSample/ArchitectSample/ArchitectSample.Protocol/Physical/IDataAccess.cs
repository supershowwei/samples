using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArchitectSample.Protocol.Physical
{
    public interface IDataAccess<T>
    {
        Task<List<T>> QueryAllAsync(
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null);

        Task<T> QueryOneAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null);

        Task<List<T>> QueryAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null);

        Task InsertAsync(T value);

        Task InsertAsync(List<T> values);

        Task BulkInsertAsync(List<T> values);

        Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter, IEnumerable<T> values);

        Task UpsertAsync(T value);

        Task UpsertAsync(List<T> values);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}
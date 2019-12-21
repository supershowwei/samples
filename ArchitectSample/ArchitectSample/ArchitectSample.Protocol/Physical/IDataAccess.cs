using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArchitectSample.Protocol.Physical
{
    public interface IDataAccess<T>
    {
        Task<T> QueryOneAsync(Expression<Func<T, bool>> predicate);

        Task<T> QueryOneAsync(Expression<Func<T, object>> selector, Expression<Func<T, bool>> predicate);

        Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> QueryAsync(Expression<Func<T, object>> selector, Expression<Func<T, bool>> predicate);

        Task InsertAsync(T value);

        Task InsertAsync(List<T> values);

        Task BulkInsertAsync(List<T> values);

        Task UpdateAsync(Expression<Func<T>> setters, Expression<Func<T, bool>> predicate);

        Task UpdateAsync(IEnumerable<(Expression<Func<T>>, Expression<Func<T, bool>>)> statements);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}
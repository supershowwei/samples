using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArchitectSample.Protocol.Physical
{
    public interface IDataAccess<T>
    {
        Task<List<T>> QueryAllAsync();

        Task<List<T>> QueryAllAsync(Expression<Func<T, object>> selector);

        Task<T> QueryOneAsync(Expression<Func<T, bool>> predicate);

        Task<T> QueryOneAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector);

        Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector);

        Task InsertAsync(T value);

        Task InsertAsync(List<T> values);

        Task BulkInsertAsync(List<T> values);

        Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        Task UpdateAsync(IEnumerable<(Expression<Func<T, bool>>, Expression<Func<T>>)> statements);

        Task UpsertAsync(T value);

        Task UpsertAsync(List<T> values);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}
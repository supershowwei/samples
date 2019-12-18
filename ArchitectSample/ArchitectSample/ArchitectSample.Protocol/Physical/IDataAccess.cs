using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArchitectSample.Protocol.Physical
{
    public interface IDataAccess<T>
    {
        Task<T> QueryOneAsnyc(Expression<Func<T, bool>> predicate);

        Task<T> QueryOneAsnyc(Expression<Func<T, object>> selector, Expression<Func<T, bool>> predicate);

        Task<List<T>> QueryAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> QueryAsync(Expression<Func<T, object>> selector, Expression<Func<T, bool>> predicate);

        Task InsertAsync(T value);

        Task InsertAsync(Expression<Func<T>> setters);

        Task UpdateAsync(Expression<Func<T>> setters, Expression<Func<T, bool>> predicate);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}
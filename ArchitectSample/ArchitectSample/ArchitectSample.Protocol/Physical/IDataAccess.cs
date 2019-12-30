﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArchitectSample.Protocol.Physical
{
    public enum Sortord
    {
        Ascending,
        Descending
    }

    public interface IDataAccess<T>
    {
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

        Task InsertAsync(Expression<Func<T>> setter);

        Task InsertAsync(T value);

        Task InsertAsync(Expression<Func<T>> setter, IEnumerable<T> values);

        Task BulkInsertAsync(IEnumerable<T> values);

        Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter, IEnumerable<T> values);

        Task UpsertAsync(T value);

        Task UpsertAsync(IEnumerable<T> values);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}
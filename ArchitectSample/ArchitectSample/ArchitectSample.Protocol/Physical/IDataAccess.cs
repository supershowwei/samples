using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ArchitectSample.Protocol.Physical
{
    public interface IDataAccess<T>
    {
        T QueryOne(Expression<Func<T, bool>> predicate);

        T QueryOne(Expression<Func<T, object>> selector, Expression<Func<T, bool>> predicate);

        List<T> Query(Expression<Func<T, bool>> predicate);

        List<T> Query(Expression<Func<T, object>> selector, Expression<Func<T, bool>> predicate);

        void Insert(T value);

        void Insert(Expression<Func<T>> setters);

        void Update(Expression<Func<T>> setters, Expression<Func<T, bool>> predicate);

        void Delete(Expression<Func<T, bool>> predicate);
    }
}
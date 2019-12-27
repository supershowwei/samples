using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Shared
{
    public class QueryObject<T>
    {
        public QueryObject(IDataAccess<T> dataAccess)
        {
            this.DataAccess = dataAccess;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, object>> Selector { get; set; }

        public Expression<Func<T>> Setter { get; set; }

        public int? Top { get; set; }
    }
}
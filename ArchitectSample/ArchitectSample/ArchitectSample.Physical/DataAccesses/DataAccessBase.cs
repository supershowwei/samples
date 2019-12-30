using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ArchitectSample.Physical.Extensions;
using ArchitectSample.Protocol.Physical;
using Chef.Extensions.Dapper;
using Dapper;

namespace ArchitectSample.Physical.DataAccesses
{
    public abstract class DataAccessBase<T>
    {
        private readonly string connectionString;
        private readonly string tableName;

        protected DataAccessBase(string connectionString)
        {
            this.connectionString = connectionString;

            this.tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
        }

        protected abstract Expression<Func<T, object>> DefaultSelector { get; }

        public virtual async Task<T> QueryOneAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            if (selector == null && this.DefaultSelector == null)
            {
                throw new NullReferenceException($"At least one of '{nameof(selector)}' and '{nameof(this.DefaultSelector)}' is not null.");
            }

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;
            sql += (selector ?? this.DefaultSelector).ToSelectList(this.tableName);
            sql += $@"
FROM {this.tableName} WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.tableName, out var parameters);
            sql += orderings.ToOrderByStatement(this.tableName);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<T>(sql, parameters);

                return result;
            }
        }

        public virtual async Task<List<T>> QueryAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            if (selector == null && this.DefaultSelector == null)
            {
                throw new NullReferenceException($"At least one of '{nameof(selector)}' and '{nameof(this.DefaultSelector)}' is not null.");
            }

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;
            sql += (selector ?? this.DefaultSelector).ToSelectList(this.tableName);
            sql += $@"
FROM {this.tableName} WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.tableName, out var parameters);
            sql += orderings.ToOrderByStatement(this.tableName);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T>(sql, parameters);

                return result.ToList();
            }
        }

        public virtual Task InsertAsync(T value)
        {
            throw new NotImplementedException();
        }

        public virtual Task InsertAsync(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

        public virtual Task BulkInsertAsync(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

        public virtual async Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            SqlBuilder sql = $@"
UPDATE {this.tableName}
SET ";
            sql += setter.ToSetStatements(out var parameters);
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition(parameters);

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, parameters);
            }
        }

        public virtual async Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter, IEnumerable<T> values)
        {
            var sql = new SqlBuilder();

            sql += $@"
UPDATE {this.tableName}
SET ";
            sql += setter.ToSetStatements();
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition();

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, values);
            }
        }

        public virtual Task UpsertAsync(T value)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpsertAsync(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
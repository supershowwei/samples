using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Physical.Extensions;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;
using Chef.Extensions.Dapper;
using Chef.Extensions.IEnumerable;
using Dapper;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubDataAccess : IDataAccess<Club>
    {
        private static readonly string ConnectionString = File.ReadAllLines(@"D:\Labs\ConnectionStrings.txt").First();

        private static readonly Expression<Func<Club, object>> DefaultSelector = x => new { x.Id, x.Name };

        public async Task<Club> QueryOneAsync(
            Expression<Func<Club, bool>> predicate,
            IEnumerable<(Expression<Func<Club, object>>, Sortord)> orderings = null,
            Expression<Func<Club, object>> selector = null,
            int? top = null)
        {
            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;
            sql += (selector ?? DefaultSelector).ToSelectList("c");
            sql += @"
FROM Club c WITH (NOLOCK)";
            sql += predicate.ToWhereStatement("c", out var parameters);
            sql += orderings.ToOrderByStatement("c");

            using (var db = new SqlConnection(ConnectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<Club>(sql, parameters);

                return result;
            }
        }

        public async Task<List<Club>> QueryAsync(
            Expression<Func<Club, bool>> predicate,
            IEnumerable<(Expression<Func<Club, object>>, Sortord)> orderings = null,
            Expression<Func<Club, object>> selector = null,
            int? top = null)
        {
            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;
            sql += (selector ?? DefaultSelector).ToSelectList("c");
            sql += @"
FROM Club c WITH (NOLOCK)";
            sql += predicate.ToWhereStatement("c", out var parameters);
            sql += orderings.ToOrderByStatement("c");

            using (var db = new SqlConnection(ConnectionString))
            {
                var result = await db.QueryAsync<Club>(sql, parameters);

                return result.ToList();
            }
        }

        public Task InsertAsync(Club value)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(IEnumerable<Club> values)
        {
            throw new NotImplementedException();
        }

        public Task BulkInsertAsync(IEnumerable<Club> values)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Expression<Func<Club, bool>> predicate, Expression<Func<Club>> setter)
        {
            SqlBuilder sql = @"
UPDATE Club
SET ";
            sql += setter.ToSetStatements(out var parameters);
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition(parameters);
            sql += ";";

            using (var db = new SqlConnection(ConnectionString))
            {
                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateAsync(Expression<Func<Club, bool>> predicate, Expression<Func<Club>> setter, IEnumerable<Club> values)
        {
            var sql = new SqlBuilder();

            sql += @"
UPDATE Club
SET ";
            sql += setter.ToSetStatements();
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition();

            using (var db = new SqlConnection(ConnectionString))
            {
                await db.ExecuteAsync(sql, values);
            }
        }

        public Task UpsertAsync(Club value)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAsync(IEnumerable<Club> values)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
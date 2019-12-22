using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;
using Chef.Extensions.Dapper;
using Dapper;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubDataAccess : IDataAccess<Club>
    {
        private static readonly string ConnectionString = File.ReadAllLines(@"D:\Labs\ConnectionStrings.txt").First();

        public async Task<Club> QueryOneAsync(Expression<Func<Club, bool>> predicate)
        {
            SqlBuilder sql = @"
SELECT
    c.ClubID AS Id
   ,c.[Name]
   ,c.IsHide
   ,c.IsActive
FROM Club c WITH (NOLOCK)
WHERE ";
            sql += predicate.ToSearchCondition("c", out var parameters);

            using (var db = new SqlConnection(ConnectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<Club>(sql, parameters);

                return result;
            }
        }

        public async Task<Club> QueryOneAsync(Expression<Func<Club, bool>> predicate, Expression<Func<Club, object>> selector)
        {
            SqlBuilder sql = @"
SELECT ";
            sql += selector.ToSelectList("c");
            sql += @"
FROM Club c WITH (NOLOCK)
WHERE ";
            sql += predicate.ToSearchCondition("c", out var parameters);

            using (var db = new SqlConnection(ConnectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<Club>(sql, parameters);

                return result;
            }
        }

        public Task<List<Club>> QueryAsync(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Club>> QueryAsync(Expression<Func<Club, bool>> predicate, Expression<Func<Club, object>> selector)
        {
            SqlBuilder sql = @"
SELECT ";
            sql += selector.ToSelectList("c");
            sql += @"
FROM Club c WITH (NOLOCK)
WHERE ";
            sql += predicate.ToSearchCondition("c", out var parameters);

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

        public Task InsertAsync(List<Club> values)
        {
            throw new NotImplementedException();
        }

        public Task BulkInsertAsync(List<Club> values)
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

        public async Task UpdateAsync(IEnumerable<(Expression<Func<Club, bool>>, Expression<Func<Club>>)> statements)
        {
            var sql = new SqlBuilder();
            var parameters = new Dictionary<string, object>();

            foreach (var (predicate, setter) in statements)
            {
                sql += @"
UPDATE Club
SET ";
                sql += setter.ToSetStatements(parameters);
                sql += @"
WHERE ";
                sql += predicate.ToSearchCondition(parameters);
                sql += ";";
            }

            using (var db = new SqlConnection(ConnectionString))
            {
                await db.ExecuteAsync(sql, parameters);
            }
        }

        public Task DeleteAsync(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
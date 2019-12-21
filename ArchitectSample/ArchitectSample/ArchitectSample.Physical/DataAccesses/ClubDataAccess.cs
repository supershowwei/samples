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

        public Task<Club> QueryOneAsync(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<Club> QueryOneAsync(Expression<Func<Club, object>> selector, Expression<Func<Club, bool>> predicate)
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

        public async Task<List<Club>> QueryAsync(Expression<Func<Club, object>> selector, Expression<Func<Club, bool>> predicate)
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

        public Task UpdateAsync(Expression<Func<Club>> setters, Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IEnumerable<ValueTuple<Expression<Func<Club>>, Expression<Func<Club, bool>>>> statements)
        {
            var sql = new SqlBuilder();
            var parameters = new Dictionary<string, object>();

            foreach (var (setters, predicate) in statements)
            {
                sql += @"
UPDATE Club
SET ";
                sql += setters.ToSetStatements(parameters);
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
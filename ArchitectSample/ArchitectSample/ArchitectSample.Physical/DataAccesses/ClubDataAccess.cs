using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public async Task<Club> QueryOneAsync(Expression<Func<Club, bool>> predicate)
        {
            SqlBuilder sql = @"
SELECT
    *
FROM Club c WITH (NOLOCK)
WHERE ";

            sql += predicate.ToSearchCondition(out var parameters);

            using (var db = new SqlConnection(""))
            {
                var result = await db.QuerySingleOrDefaultAsync<Club>(sql, parameters);

                return result;
            }
        }

        public Task<Club> QueryOneAsync(Expression<Func<Club, object>> selector, Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<Club>> QueryAsync(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<Club>> QueryAsync(Expression<Func<Club, object>> selector, Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Club value)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Expression<Func<Club>> setters)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expression<Func<Club>> setters, Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
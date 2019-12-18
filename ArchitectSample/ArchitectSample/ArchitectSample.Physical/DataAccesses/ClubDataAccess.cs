using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubDataAccess : IDataAccess<Club>
    {
        public Task<Club> QueryOneAsync(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
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
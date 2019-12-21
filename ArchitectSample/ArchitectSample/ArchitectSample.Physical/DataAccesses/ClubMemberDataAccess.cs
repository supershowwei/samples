using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubMemberDataAccess : IDataAccess<ClubMember>
    {
        public Task<ClubMember> QueryOneAsync(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<ClubMember> QueryOneAsync(Expression<Func<ClubMember, object>> selector, Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClubMember>> QueryAsync(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClubMember>> QueryAsync(Expression<Func<ClubMember, object>> selector, Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(ClubMember value)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(List<ClubMember> values)
        {
            throw new NotImplementedException();
        }

        public Task BulkInsertAsync(List<ClubMember> values)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expression<Func<ClubMember>> setters, Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEnumerable<(Expression<Func<ClubMember>>, Expression<Func<ClubMember, bool>>)> statements)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
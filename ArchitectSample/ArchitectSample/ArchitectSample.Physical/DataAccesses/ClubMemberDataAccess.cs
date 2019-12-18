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
        public Task<ClubMember> QueryOneAsnyc(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<ClubMember> QueryOneAsnyc(Expression<Func<ClubMember, object>> selector, Expression<Func<ClubMember, bool>> predicate)
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

        public Task InsertAsync(Expression<Func<ClubMember>> setters)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expression<Func<ClubMember>> setters, Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
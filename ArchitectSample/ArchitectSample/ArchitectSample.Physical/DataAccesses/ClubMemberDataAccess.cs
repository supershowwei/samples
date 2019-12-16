using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubMemberDataAccess : IDataAccess<ClubMember>
    {
        public ClubMember QueryOne(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ClubMember QueryOne(Expression<Func<ClubMember, object>> selector, Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<ClubMember> Query(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<ClubMember> Query(Expression<Func<ClubMember, object>> selector, Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Insert(ClubMember value)
        {
            throw new NotImplementedException();
        }

        public void Insert(Expression<Func<ClubMember>> setters)
        {
            throw new NotImplementedException();
        }

        public void Update(Expression<Func<ClubMember>> setters, Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<ClubMember, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
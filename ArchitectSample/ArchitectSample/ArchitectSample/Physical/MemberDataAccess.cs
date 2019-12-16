using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArchitectSample.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical
{
    public class MemberDataAccess : IDataAccess<Member>
    {
        public Member QueryOne(Expression<Func<Member, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Member QueryOne(Expression<Func<Member, object>> selector, Expression<Func<Member, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<Member> Query(Expression<Func<Member, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<Member> Query(Expression<Func<Member, object>> selector, Expression<Func<Member, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Insert(Member value)
        {
            throw new NotImplementedException();
        }

        public void Insert(Expression<Func<Member>> setters)
        {
            throw new NotImplementedException();
        }

        public void Update(Expression<Func<Member>> setters, Expression<Func<Member, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<Member, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
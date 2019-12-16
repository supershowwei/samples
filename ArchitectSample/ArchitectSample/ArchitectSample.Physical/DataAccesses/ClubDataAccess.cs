using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubDataAccess : IDataAccess<Club>
    {
        public Club QueryOne(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Club QueryOne(Expression<Func<Club, object>> selector, Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<Club> Query(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<Club> Query(Expression<Func<Club, object>> selector, Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Insert(Club value)
        {
            throw new NotImplementedException();
        }

        public void Insert(Expression<Func<Club>> setters)
        {
            throw new NotImplementedException();
        }

        public void Update(Expression<Func<Club>> setters, Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<Club, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
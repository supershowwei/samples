using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubArticleDataAccess : IDataAccess<ClubArticle>
    {
        public ClubArticle QueryOne(Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ClubArticle QueryOne(Expression<Func<ClubArticle, object>> selector, Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<ClubArticle> Query(Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<ClubArticle> Query(Expression<Func<ClubArticle, object>> selector, Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Insert(ClubArticle value)
        {
            throw new NotImplementedException();
        }

        public void Insert(Expression<Func<ClubArticle>> setters)
        {
            throw new NotImplementedException();
        }

        public void Update(Expression<Func<ClubArticle>> setters, Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
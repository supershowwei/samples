using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubArticleDataAccess : IDataAccess<ClubArticle>
    {
        public Task<ClubArticle> QueryOneAsync(Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<ClubArticle> QueryOneAsync(
            Expression<Func<ClubArticle, object>> selector,
            Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClubArticle>> QueryAsync(Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClubArticle>> QueryAsync(Expression<Func<ClubArticle, object>> selector, Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(ClubArticle value)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(List<ClubArticle> values)
        {
            throw new NotImplementedException();
        }

        public Task BulkInsertAsync(List<ClubArticle> values)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expression<Func<ClubArticle>> setters, Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEnumerable<(Expression<Func<ClubArticle>>, Expression<Func<ClubArticle, bool>>)> statements)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
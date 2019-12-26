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
        public Task<List<ClubArticle>> QueryAllAsync(
            IEnumerable<(Expression<Func<ClubArticle, object>>, Sortord)> orderings = null,
            Expression<Func<ClubArticle, object>> selector = null,
            int? top = null)
        {
            throw new NotImplementedException();
        }

        public Task<ClubArticle> QueryOneAsync(
            Expression<Func<ClubArticle, bool>> predicate,
            IEnumerable<(Expression<Func<ClubArticle, object>>, Sortord)> orderings = null,
            Expression<Func<ClubArticle, object>> selector = null,
            int? top = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClubArticle>> QueryAsync(
            Expression<Func<ClubArticle, bool>> predicate,
            IEnumerable<(Expression<Func<ClubArticle, object>>, Sortord)> orderings = null,
            Expression<Func<ClubArticle, object>> selector = null,
            int? top = null)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(ClubArticle value)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(IEnumerable<ClubArticle> values)
        {
            throw new NotImplementedException();
        }

        public Task BulkInsertAsync(IEnumerable<ClubArticle> values)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expression<Func<ClubArticle, bool>> predicate, Expression<Func<ClubArticle>> setter)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expression<Func<ClubArticle, bool>> predicate, Expression<Func<ClubArticle>> setter, IEnumerable<ClubArticle> values)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAsync(ClubArticle value)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAsync(IEnumerable<ClubArticle> values)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<ClubArticle, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
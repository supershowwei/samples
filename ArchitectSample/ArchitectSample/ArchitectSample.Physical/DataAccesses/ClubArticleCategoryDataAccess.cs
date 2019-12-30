using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubArticleCategoryDataAccess : DataAccessBase<ClubArticleCategory>, IDataAccess<ClubArticleCategory>
    {
        public ClubArticleCategoryDataAccess()
            : base(File.ReadAllLines(@"D:\Labs\ConnectionStrings.txt").First())
        {
        }

        protected override Expression<Func<ClubArticleCategory, object>> DefaultSelector { get; } =
            x => new { x.Id, x.Name, x.RequiredReadingVideos };

        protected override Expression<Func<ClubArticleCategory>> DefaultColumns { get; } = () =>
            new ClubArticleCategory { Id = default, Name = default, RequiredReadingVideos = default };

        public Task BulkInsertAsync(IEnumerable<ClubArticleCategory> values)
        {
            throw new NotImplementedException();
        }

        public Task BulkUpsertAsync(IEnumerable<ClubArticleCategory> values)
        {
            throw new NotImplementedException();
        }
    }
}
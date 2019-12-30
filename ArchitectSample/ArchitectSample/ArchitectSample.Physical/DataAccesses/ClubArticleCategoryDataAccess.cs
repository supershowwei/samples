using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using Chef.Extensions.DbAccess;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubArticleCategoryDataAccess : SqlServerDataAccess<ClubArticleCategory>
    {
        public ClubArticleCategoryDataAccess()
            : base(File.ReadAllLines(@"D:\Labs\ConnectionStrings.txt").First())
        {
        }

        protected override Expression<Func<ClubArticleCategory, object>> DefaultSelector { get; } =
            x => new { x.Id, x.Name, x.RequiredReadingVideos };

        protected override Expression<Func<ClubArticleCategory>> RequiredColumns { get; } = () =>
            new ClubArticleCategory { Id = default, Name = default, RequiredReadingVideos = default };

        protected override (string, DataTable) ConvertToTableValuedParameters(IEnumerable<ClubArticleCategory> values)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using Chef.Extensions.DbAccess;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubArticleCategoryDataAccess : SqlServerDataAccess<ClubArticleCategory>
    {
        public ClubArticleCategoryDataAccess()
            : base(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Club;Integrated Security=True")
        {
        }

        protected override Expression<Func<ClubArticleCategory, object>> DefaultSelector { get; } =
            x => new { x.Id, x.Name, x.RequiredReadingVideos };

        protected override Expression<Func<ClubArticleCategory>> RequiredColumns { get; } = () =>
            new ClubArticleCategory { Id = default, Name = default, RequiredReadingVideos = default };
    }
}
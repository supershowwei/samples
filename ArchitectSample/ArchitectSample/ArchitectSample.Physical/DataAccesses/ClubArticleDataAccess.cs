using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using Chef.Extensions.DbAccess;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubArticleDataAccess : SqlServerDataAccess<ClubArticle>
    {
        public ClubArticleDataAccess()
            : base(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Club;Integrated Security=True")
        {
        }

        protected override Expression<Func<ClubArticle, object>> DefaultSelector { get; } = x => new { x.Id, x.ClubId, x.Topic, x.Content };

        protected override Expression<Func<ClubArticle>> RequiredColumns { get; } = () =>
            new ClubArticle { Id = default, ClubId = default, Topic = default, Content = default };

        protected override (string, DataTable) ConvertToTableValuedParameters(IEnumerable<ClubArticle> values)
        {
            throw new NotImplementedException();
        }
    }
}
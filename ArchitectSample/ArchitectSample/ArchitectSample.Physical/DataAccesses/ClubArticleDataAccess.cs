using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubArticleDataAccess : DataAccessBase<ClubArticle>, IDataAccess<ClubArticle>
    {
        public ClubArticleDataAccess()
            : base(File.ReadAllLines(@"D:\Labs\ConnectionStrings.txt").First())
        {
        }

        protected override Expression<Func<ClubArticle, object>> DefaultSelector { get; } = x => new { x.Id, x.ClubId, x.Topic, x.Content };
    }
}
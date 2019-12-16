using System;
using System.Collections.Generic;
using ArchitectSample.Protocol.Model.Data;

namespace ArchitectSample.Physical.Repositories
{
    public partial class ClubRepository
    {
        public List<ClubArticle> QueryArticles(int clubId, DateTime startPublicationTime, DateTime endPublicationTime)
        {
            throw new NotImplementedException();
        }
    }
}
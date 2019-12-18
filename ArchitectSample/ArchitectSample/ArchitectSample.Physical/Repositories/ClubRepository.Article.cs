using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;

namespace ArchitectSample.Physical.Repositories
{
    public partial class ClubRepository
    {
        public Task<List<ClubArticle>> QueryArticlesAsync(int clubId, DateTime startPublicationTime, DateTime endPublicationTime)
        {
            throw new NotImplementedException();
        }
    }
}
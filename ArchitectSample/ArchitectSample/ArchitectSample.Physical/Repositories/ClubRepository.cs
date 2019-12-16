using System;
using System.Collections.Generic;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.Repositories
{
    public class ClubRepository : IClubRepository
    {
        public List<Member> QueryMembers(int clubId)
        {
            throw new NotImplementedException();
        }

        public List<ClubArticle> QueryArticles(int clubId, DateTime startPublicationTime, DateTime endPublicationTime)
        {
            throw new NotImplementedException();
        }
    }
}
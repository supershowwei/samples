using System;
using System.Collections.Generic;
using ArchitectSample.Protocol.Model.Data;

namespace ArchitectSample.Protocol.Physical
{
    public interface IClubRepository
    {
        List<Member> QueryMembers(int clubId);

        List<ClubArticle> QueryArticles(int clubId, DateTime startPublicationTime, DateTime endPublicationTime);
    }
}
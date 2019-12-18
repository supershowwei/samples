using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;

namespace ArchitectSample.Protocol.Physical
{
    public interface IClubRepository
    {
        Task<List<Member>> QueryMembersAsync(int clubId);

        Task<List<ClubArticle>> QueryArticlesAsync(int clubId, DateTime startPublicationTime, DateTime endPublicationTime);
    }
}
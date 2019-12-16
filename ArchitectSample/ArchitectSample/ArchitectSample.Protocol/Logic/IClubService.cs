using System;
using System.Collections.Generic;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;

namespace ArchitectSample.Protocol.Logic
{
    public interface IClubService
    {
        ServiceResult<Club> GetClub(int clubId);

        ServiceResult<List<ClubArticle>> ListArticles(int clubId, DateTime startPublicationTime, DateTime endPublicationTime);

        ServiceResult<List<Member>> ListMembers(int clubId);
    }
}
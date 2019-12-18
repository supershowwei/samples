using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;

namespace ArchitectSample.Protocol.Logic
{
    public interface IClubService
    {
        Task<ServiceResult<Club>> GetClub(int clubId);

        Task<ServiceResult<List<ClubArticle>>> ListArticles(int clubId, DateTime startPublicationTime, DateTime endPublicationTime);

        Task<ServiceResult<List<Member>>> ListMembers(int clubId);
    }
}
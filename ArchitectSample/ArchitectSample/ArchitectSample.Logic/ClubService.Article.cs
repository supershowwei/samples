using System;
using System.Collections.Generic;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Logic
{
    public partial class ClubService
    {
        public ServiceResult<List<ClubArticle>> ListArticles(int clubId, DateTime startPublicationTime, DateTime endPublicationTime)
        {
            var articles = this.clubRepository.QueryArticles(clubId, startPublicationTime, endPublicationTime);

            return ServiceResult.Success(articles);
        }
    }
}
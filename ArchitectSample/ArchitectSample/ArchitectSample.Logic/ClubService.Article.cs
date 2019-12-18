using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;

namespace ArchitectSample.Logic
{
    public partial class ClubService
    {
        public async Task<ServiceResult<List<ClubArticle>>> ListArticlesAsync(int clubId, DateTime startPublicationTime, DateTime endPublicationTime)
        {
            var articles = await this.clubRepository.QueryArticlesAsync(clubId, startPublicationTime, endPublicationTime);

            return ServiceResult.Success(articles);
        }
    }
}
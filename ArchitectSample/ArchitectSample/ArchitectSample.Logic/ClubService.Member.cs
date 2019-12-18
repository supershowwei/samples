using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;

namespace ArchitectSample.Logic
{
    public partial class ClubService
    {
        public async Task<ServiceResult<List<Member>>> ListMembersAsync(int clubId)
        {
            var members = await this.clubRepository.QueryMembersAsync(clubId);

            return ServiceResult.Success(members);
        }
    }
}
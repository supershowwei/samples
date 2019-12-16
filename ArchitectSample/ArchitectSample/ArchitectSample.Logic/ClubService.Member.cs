using System;
using System.Collections.Generic;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;

namespace ArchitectSample.Logic
{
    public partial class ClubService
    {
        public ServiceResult<List<Member>> ListMembers(int clubId)
        {
            var members = this.clubRepository.QueryMembers(clubId);

            return ServiceResult.Success(members);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.Repositories
{
    public partial class ClubRepository : IClubRepository
    {
        public Task<List<Member>> QueryMembersAsync(int clubId)
        {
            throw new NotImplementedException();
        }
    }
}
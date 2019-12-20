using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;

namespace ArchitectSample.Physical.Repositories
{
    public partial class ClubRepository
    {
        public Task<List<Member>> QueryMembersAsync(int clubId)
        {
            throw new NotImplementedException();
        }
    }
}
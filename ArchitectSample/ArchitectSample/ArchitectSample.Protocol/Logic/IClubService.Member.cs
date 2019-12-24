using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;

namespace ArchitectSample.Protocol.Logic
{
    public partial interface IClubService
    {
        Task<ServiceResult<List<Member>>> ListMembersAsync(int clubId);
    }
}
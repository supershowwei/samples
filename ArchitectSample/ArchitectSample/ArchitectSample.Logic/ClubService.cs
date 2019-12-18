using System.Threading.Tasks;
using ArchitectSample.Protocol.Logic;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Model.Results;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Logic
{
    public partial class ClubService : IClubService
    {
        private readonly IDataAccess<Club> clubDataAccess;
        private readonly IClubRepository clubRepository;

        public ClubService(IDataAccess<Club> clubDataAccess, IClubRepository clubRepository)
        {
            this.clubDataAccess = clubDataAccess;
            this.clubRepository = clubRepository;
        }

        public async Task<ServiceResult<Club>> GetClubAsync(int clubId)
        {
            var club = await this.clubDataAccess.QueryOneAsync(c => c.Id == clubId);

            return ServiceResult.Success(club);
        }
    }
}
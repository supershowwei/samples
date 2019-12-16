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

        public ServiceResult<Club> GetClub(int clubId)
        {
            var club = this.clubDataAccess.QueryOne(c => c.Id == clubId);

            return ServiceResult.Success(club);
        }
    }
}
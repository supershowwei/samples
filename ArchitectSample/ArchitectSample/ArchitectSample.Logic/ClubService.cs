﻿using System.Threading.Tasks;
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

        // Wrong code! Don't use it.
        public async Task<ServiceResult<Club>> GetClub(int clubId)
        {
            var club = await this.clubDataAccess.QueryOneAsync(TODO);

            return ServiceResult.Success(club);
        }
    }
}
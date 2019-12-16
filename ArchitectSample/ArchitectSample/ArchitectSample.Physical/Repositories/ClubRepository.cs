﻿using System;
using System.Collections.Generic;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.Physical.Repositories
{
    public partial class ClubRepository : IClubRepository
    {
        public List<Member> QueryMembers(int clubId)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;
using Chef.Extensions.Dapper;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubDataAccess : DataAccessBase<Club>, IDataAccess<Club>
    {
        public ClubDataAccess()
            : base(File.ReadAllLines(@"D:\Labs\ConnectionStrings.txt").First())
        {
        }

        protected override Expression<Func<Club, object>> DefaultSelector { get; } = x => new { x.Id, x.Name };

        protected override Expression<Func<Club>> DefaultColumns { get; } =
            () => new Club { Id = default, Name = default, IsActive = default };

        public Task BulkInsertAsync(IEnumerable<Club> values)
        {
            throw new NotImplementedException();
        }

        public Task BulkUpsertAsync(IEnumerable<Club> values)
        {
            throw new NotImplementedException();
        }
    }
}
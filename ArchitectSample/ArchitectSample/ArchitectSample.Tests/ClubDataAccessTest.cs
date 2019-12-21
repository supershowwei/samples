using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ArchitectSample.Physical.DataAccesses;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;
using ArchitectSample.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchitectSample.Tests
{
    [TestClass]
    public class ClubDataAccessTest
    {
        [TestMethod]
        public async Task Test_QueryOneAsync()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25);

            Assert.AreEqual("軌道鞅之股期權常勝軍", club.Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_Multiply()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "永政交易工作室" + suffix },
                            new Club { Id = 16, Name = "名諭爸的股票.權證.實戰夢想室" + suffix },
                            new Club { Id = 19, Name = "何毅的實戰控盤轉折術" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.UpdateAsync(clubs.Select(c => c.Set(() => new Club { Name = c.Name }).Where(x => x.Id == c.Id)));

            var actual = await clubDataAccess.QueryAsync(x => new { x.Id, x.Name }, x => new[] { 15, 16, 19 }.Contains(x.Id));

            Assert.AreEqual("永政交易工作室" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("名諭爸的股票.權證.實戰夢想室" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("何毅的實戰控盤轉折術" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }
    }
}

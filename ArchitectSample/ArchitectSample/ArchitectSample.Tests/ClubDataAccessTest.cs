using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public async Task Test_QueryOneAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25, x => new { x.Name });

            Assert.AreEqual(0, club.Id);
            Assert.AreEqual("軌道鞅之股期權常勝軍", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.QueryAsync(x => new[] { 17, 25 }.Contains(x.Id), x => new { x.Name });

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual(0, clubs[1].Id);
            Assert.AreEqual("玩股網功能測試", clubs[0].Name);
            Assert.AreEqual("軌道鞅之股期權常勝軍", clubs[1].Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubName = "永政交易工作室" + suffix;

            await clubDataAccess.UpdateAsync(x => x.Id.Equals(15), () => new Club { Name = clubName });

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 15);

            Assert.AreEqual(15, club.Id);
            Assert.AreEqual("永政交易工作室" + suffix, club.Name);
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

            var statements = clubs.Select(c => c.ToStatement(x => x.Id == c.Id, () => new Club { Name = c.Name }));

            await clubDataAccess.UpdateAsync(statements);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), x => new { x.Id, x.Name });

            Assert.AreEqual("永政交易工作室" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("名諭爸的股票.權證.實戰夢想室" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("何毅的實戰控盤轉折術" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }
    }
}

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

            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryOneAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25, x => new { x.Name });

            Assert.AreEqual(0, club.Id);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.QueryAsync(x => new[] { 17, 25 }.Contains(x.Id), x => new { x.Name });

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual(0, clubs[1].Id);
            Assert.AreEqual("���Ѻ��\�����", clubs[0].Name);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", clubs[1].Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubName = "�ìF����u�@��" + suffix;

            await clubDataAccess.UpdateAsync(x => x.Id.Equals(15), () => new Club { Name = clubName });

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 15);

            Assert.AreEqual(15, club.Id);
            Assert.AreEqual("�ìF����u�@��" + suffix, club.Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_Multiply()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "�ìF����u�@��" + suffix },
                            new Club { Id = 16, Name = "�W�٪����Ѳ�.�v��.��ԹڷQ��" + suffix },
                            new Club { Id = 19, Name = "��ݪ���Ա��L���N" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var statements = clubs.Select(c => c.ToStatement(x => x.Id == c.Id, () => new Club { Name = c.Name }));

            await clubDataAccess.UpdateAsync(statements);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), x => new { x.Id, x.Name });

            Assert.AreEqual("�ìF����u�@��" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("�W�٪����Ѳ�.�v��.��ԹڷQ��" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("��ݪ���Ա��L���N" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }
    }
}

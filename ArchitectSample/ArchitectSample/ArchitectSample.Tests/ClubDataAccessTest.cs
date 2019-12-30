using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectSample.Physical.DataAccesses;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;
using ArchitectSample.Shared.Extensions;
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

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25, selector: x => new { x.Name });

            Assert.AreEqual(0, club.Id);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryOneAsync_with_Selector_use_QueryObject()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.Where(x => x.Id == 25).Select(x => new { x.Name }).QueryOneAsync();

            Assert.AreEqual(0, club.Id);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.QueryAsync(x => new[] { 17, 25 }.Contains(x.Id), selector: x => new { x.Name });

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual(0, clubs[1].Id);
            Assert.AreEqual("���Ѻ��\�����", clubs[0].Name);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", clubs[1].Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => new[] { 17, 25 }.Contains(x.Id)).Select(x => new { x.Name }).QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual(0, clubs[1].Id);
            Assert.AreEqual("���Ѻ��\�����", clubs[0].Name);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", clubs[1].Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject_and_OrderByDescending()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => new[] { 17, 25 }.Contains(x.Id))
                            .OrderByDescending(x => x.Id)
                            .Select(x => new { x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual(0, clubs[1].Id);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", clubs[0].Name);
            Assert.AreEqual("���Ѻ��\�����", clubs[1].Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject_and_OrderByDescending_and_Top()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => new[] { 17, 25 }.Contains(x.Id))
                            .OrderByDescending(x => x.Id)
                            .Select(x => new { x.Name })
                            .Top(1)
                            .QueryAsync();

            Assert.AreEqual(1, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", clubs[0].Name);
        }

        [TestMethod]
        public async Task Test_QueryAllAsync_with_Selector_use_QueryObject_and_OrderByDescending_and_ThenBy_Top()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.OrderByDescending(x => x.IsActive)
                            .ThenBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .Top(1)
                            .QueryAsync();

            Assert.AreEqual(1, clubs.Count);
            Assert.AreEqual(9, clubs[0].Id);
            Assert.AreEqual("�u�u�t���x��", clubs[0].Name);
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
        public async Task Test_UpdateAsync_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubName = "�ìF����u�@��" + suffix;

            await clubDataAccess.Where(x => x.Id.Equals(15)).Set(() => new Club { Name = clubName }).UpdateAsync();

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

            await clubDataAccess.UpdateAsync(x => x.Id == default, () => new Club { Name = default }, clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            Assert.AreEqual("�ìF����u�@��" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("�W�٪����Ѳ�.�v��.��ԹڷQ��" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("��ݪ���Ա��L���N" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_Multiply_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "�ìF����u�@��" + suffix },
                            new Club { Id = 16, Name = "�W�٪����Ѳ�.�v��.��ԹڷQ��" + suffix },
                            new Club { Id = 19, Name = "��ݪ���Ա��L���N" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == default).Set(() => new Club { Name = default }).UpdateAsync(clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            Assert.AreEqual("�ìF����u�@��" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("�W�٪����Ѳ�.�v��.��ԹڷQ��" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("��ݪ���Ա��L���N" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(() => new Club { Id = 999, Name = "TestClub" });

            var club = await clubDataAccess.Where(x => x.Id == 999).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.AreEqual(999, club.Id);
            Assert.AreEqual("TestClub", club.Name);

            await clubDataAccess.DeleteAsync(x => x.Id == 999);

            club = await clubDataAccess.Where(x => x.Id == 999).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.IsNull(club);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_use_QueryObject()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Set(() => new Club { Id = 999, Name = "TestClub" }).InsertAsync();

            var club = await clubDataAccess.Where(x => x.Id == 999).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.AreEqual(999, club.Id);
            Assert.AreEqual("TestClub", club.Name);

            await clubDataAccess.Where(x => x.Id == 999).DeleteAsync();

            club = await clubDataAccess.Where(x => x.Id == 999).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.IsNull(club);
        }
    }
}

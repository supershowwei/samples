using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ArchitectSample.Physical.DataAccesses;
using ArchitectSample.Protocol.Model.Data;
using Chef.Extensions.DbAccess;
using Chef.Extensions.DbAccess.Fluent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchitectSample.Tests
{
    [TestClass]
    public class ClubDataAccessTest
    {
        [TestMethod]
        public void Test_QueryOne()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = clubDataAccess.QueryOne(x => x.Id == 25);

            Assert.AreEqual("¾H°¶¦¨", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryOneAsync()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25);

            Assert.AreEqual("¾H°¶¦¨", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryOneAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25, selector: x => new { x.Name });

            Assert.AreEqual(0, club.Id);
            Assert.AreEqual("¾H°¶¦¨", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryOneAsync_with_Selector_use_QueryObject()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.Where(x => x.Id == 25).Select(x => new { x.Name }).QueryOneAsync();

            Assert.AreEqual(0, club.Id);
            Assert.AreEqual("¾H°¶¦¨", club.Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.QueryAsync(x => new[] { 17, 25 }.Contains(x.Id), selector: x => new { x.Name });

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual(0, clubs[1].Id);
            Assert.AreEqual("§d²Q®S", clubs[0].Name);
            Assert.AreEqual("¾H°¶¦¨", clubs[1].Name);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => new[] { 17, 25 }.Contains(x.Id)).Select(x => new { x.Name }).QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(0, clubs[0].Id);
            Assert.AreEqual(0, clubs[1].Id);
            Assert.AreEqual("§d²Q®S", clubs[0].Name);
            Assert.AreEqual("¾H°¶¦¨", clubs[1].Name);
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
            Assert.AreEqual("¾H°¶¦¨", clubs[0].Name);
            Assert.AreEqual("§d²Q®S", clubs[1].Name);
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
            Assert.AreEqual("¾H°¶¦¨", clubs[0].Name);
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
            Assert.AreEqual("§d¬ü´f", clubs[0].Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubName = "¼Ú¶§¨¹Þ³" + suffix;

            await clubDataAccess.UpdateAsync(x => x.Id.Equals(15), () => new Club { Name = clubName });

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 15);

            Assert.AreEqual(15, club.Id);
            Assert.AreEqual("¼Ú¶§¨¹Þ³" + suffix, club.Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubName = "¼Ú¶§¨¹Þ³" + suffix;

            await clubDataAccess.Where(x => x.Id.Equals(15)).Set(() => new Club { Name = clubName }).UpdateAsync();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 15);

            Assert.AreEqual(15, club.Id);
            Assert.AreEqual("¼Ú¶§¨¹Þ³" + suffix, club.Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_Multiply()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "¼Ú¶§¨¹Þ³" + suffix },
                            new Club { Id = 16, Name = "Ã¹©É§g" + suffix },
                            new Club { Id = 19, Name = "·¨Öö¶Q" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.UpdateAsync(x => x.Id == default, () => new Club { Name = default }, clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            Assert.AreEqual("¼Ú¶§¨¹Þ³" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("Ã¹©É§g" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("·¨Öö¶Q" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_Multiply_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "¼Ú¶§¨¹Þ³" + suffix },
                            new Club { Id = 16, Name = "Ã¹©É§g" + suffix },
                            new Club { Id = 19, Name = "·¨Öö¶Q" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == default).Set(() => new Club { Name = default }).UpdateAsync(clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            Assert.AreEqual("¼Ú¶§¨¹Þ³" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("Ã¹©É§g" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("·¨Öö¶Q" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(new Club { Id = clubId, Name = "TestClub" });

            var club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.AreEqual(clubId, club.Id);
            Assert.AreEqual("TestClub", club.Name);

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.IsNull(club);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_use_Setter()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);
            
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(() => new Club { Id = clubId, Name = "TestClub" });

            var club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.AreEqual(clubId, club.Id);
            Assert.AreEqual("TestClub", club.Name);

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.IsNull(club);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_use_QueryObject()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Set(() => new Club { Id = clubId, Name = "TestClub" }).InsertAsync();

            var club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.AreEqual(clubId, club.Id);
            Assert.AreEqual("TestClub", club.Name);

            await clubDataAccess.Where(x => x.Id == clubId).DeleteAsync();

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            Assert.IsNull(club);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_Multiply()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(
                new List<Club>
                {
                    new Club { Id = clubIds[1], Name = "TestClub999", IsActive = true }, new Club { Id = clubIds[0], Name = "TestClub998" }
                });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name, x.IsActive })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(clubIds[0], clubs[0].Id);
            Assert.AreEqual(false, clubs[0].IsActive);
            Assert.AreEqual(true, clubs[1].IsActive);
            Assert.AreEqual("TestClub999", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id)).Select(x => new { x.Id, x.Name }).QueryAsync();

            Assert.AreEqual(0, clubs.Count);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_Multiply_use_Setter()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(
                () => new Club { Id = default, Name = default },
                new List<Club> { new Club { Id = clubIds[1], Name = "TestClub999" }, new Club { Id = clubIds[0], Name = "TestClub998" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(clubIds[0], clubs[0].Id);
            Assert.AreEqual("TestClub999", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id)).Select(x => new { x.Id, x.Name }).QueryAsync();

            Assert.AreEqual(0, clubs.Count);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_Multiply_use_Setter_and_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Set(() => new Club { Id = default, Name = default })
                .InsertAsync(new List<Club> { new Club { Id = clubIds[1], Name = "TestClub999" }, new Club { Id = clubIds[0], Name = "TestClub998" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual(clubIds[0], clubs[0].Id);
            Assert.AreEqual("TestClub999", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id)).Select(x => new { x.Id, x.Name }).QueryAsync();

            Assert.AreEqual(0, clubs.Count);
        }

        [TestMethod]
        public async Task Test_UpsertAsync()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.UpsertAsync(x => x.Id == clubId, () => new Club { Name = "TestClub" });

            var club = await clubDataAccess.Where(x => x.Id == clubId)
                            .Select(x => new { x.Id, x.Name, x.IsActive })
                            .QueryOneAsync();

            Assert.AreEqual(clubId, club.Id);
            Assert.AreEqual("TestClub", club.Name);
            Assert.AreEqual(true, club.IsActive);

            await clubDataAccess.UpsertAsync(
                x => x.Id == clubId && x.IsActive == true,
                () => new Club { Name = "TestClub997", IsActive = false });

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            Assert.AreEqual(clubId, club.Id);
            Assert.AreEqual("TestClub997", club.Name);
            Assert.AreEqual(false, club.IsActive);

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            Assert.IsNull(club);
        }

        [TestMethod]
        public async Task Test_UpsertAsync_use_QueryObject()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == clubId).Set(() => new Club { Name = "TestClub" }).UpsertAsync();

            var club = await clubDataAccess.Where(x => x.Id == clubId)
                            .Select(x => new { x.Id, x.Name, x.IsActive })
                            .QueryOneAsync();

            Assert.AreEqual(clubId, club.Id);
            Assert.AreEqual("TestClub", club.Name);
            Assert.AreEqual(true, club.IsActive);


            await clubDataAccess.Where(x => x.Id == clubId && x.IsActive == true)
                .Set(() => new Club { Name = "TestClub997", IsActive = false })
                .UpsertAsync();

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            Assert.AreEqual(clubId, club.Id);
            Assert.AreEqual("TestClub997", club.Name);
            Assert.AreEqual(false, club.IsActive);

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            Assert.IsNull(club);
        }

        [TestMethod]
        public async Task Test_UpsertAsync_Multiply()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.UpsertAsync(
                x => x.Id == default,
                () => new Club { Name = default },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub1", clubs[0].Name);
            Assert.AreEqual("TestClub2", clubs[1].Name);

            await clubDataAccess.UpsertAsync(
                x => x.Id == default,
                () => new Club { Name = default },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub3", clubs[0].Name);
            Assert.AreEqual("TestClub4", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));
        }

        [TestMethod]
        public async Task Test_UpsertAsync_Multiply_use_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == default)
                .Set(() => new Club { Name = default })
                .UpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub1", clubs[0].Name);
            Assert.AreEqual("TestClub2", clubs[1].Name);

            await clubDataAccess.Where(x => x.Id == default)
                .Set(() => new Club { Name = default })
                .UpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub3", clubs[0].Name);
            Assert.AreEqual("TestClub4", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));
        }

        [TestMethod]
        public async Task Test_BulkInsert()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.BulkInsertAsync(
                () => new Club { Id = default, Name = default },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub1", clubs[0].Name);
            Assert.AreEqual("TestClub2", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));
        }

        [TestMethod]
        public async Task Test_BulkInsert_use_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Set(() => new Club { Id = default, Name = default })
                .BulkInsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub1", clubs[0].Name);
            Assert.AreEqual("TestClub2", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));
        }

        [TestMethod]
        public async Task Test_BulkUpdate()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "¼Ú¶§¨¹Þ³" + suffix },
                            new Club { Id = 16, Name = "Ã¹©É§g" + suffix },
                            new Club { Id = 19, Name = "·¨Öö¶Q" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.BulkUpdateAsync(x => x.Id == default, () => new Club { Name = default }, clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            Assert.AreEqual("¼Ú¶§¨¹Þ³" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("Ã¹©É§g" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("·¨Öö¶Q" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }

        [TestMethod]
        public async Task Test_BulkUpdate_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "¼Ú¶§¨¹Þ³" + suffix },
                            new Club { Id = 16, Name = "Ã¹©É§g" + suffix },
                            new Club { Id = 19, Name = "·¨Öö¶Q" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == default).Set(() => new Club { Name = default }).BulkUpdateAsync(clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            Assert.AreEqual("¼Ú¶§¨¹Þ³" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("Ã¹©É§g" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("·¨Öö¶Q" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }

        [TestMethod]
        public async Task Test_BulkUpsertAsync()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.BulkUpsertAsync(
                x => x.Id >= 0 && x.Id <= 0,
                () => new Club { Name = default },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub1", clubs[0].Name);
            Assert.AreEqual("TestClub2", clubs[1].Name);

            await clubDataAccess.BulkUpsertAsync(
                x => x.Id == default,
                () => new Club { Name = default },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub3", clubs[0].Name);
            Assert.AreEqual("TestClub4", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));
        }

        [TestMethod]
        public async Task Test_BulkUpsertAsync_use_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id >= default(int) && x.Id <= default(int))
                .Set(() => new Club { Name = default })
                .BulkUpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub1", clubs[0].Name);
            Assert.AreEqual("TestClub2", clubs[1].Name);

            await clubDataAccess.Where(x => x.Id == default)
                .Set(() => new Club { Name = default })
                .BulkUpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            Assert.AreEqual(2, clubs.Count);
            Assert.AreEqual("TestClub3", clubs[0].Name);
            Assert.AreEqual("TestClub4", clubs[1].Name);

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));
        }

        [TestMethod]
        public async Task Test_TransactionScope_Query_and_Update()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(() => new Club { Id = clubId, Name = "TestClub" });

            Club club;
            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

                club.Name += "989";

                await clubDataAccess.Where(x => x.Id == clubId).Set(() => new Club { Name = club.Name }).UpdateAsync();

                tx.Complete();
            }

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            Assert.AreEqual("TestClub989", club.Name);
        }
    }
}

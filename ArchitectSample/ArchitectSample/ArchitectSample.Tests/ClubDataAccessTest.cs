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

            Assert.AreEqual("�y�D�ߤ��Ѵ��v�`�ӭx", club.Name);
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

            await clubDataAccess.UpdateAsync(clubs.Select(c => c.Set(() => new Club { Name = c.Name }).Where(x => x.Id == c.Id)));

            var actual = await clubDataAccess.QueryAsync(x => new { x.Id, x.Name }, x => new[] { 15, 16, 19 }.Contains(x.Id));

            Assert.AreEqual("�ìF����u�@��" + suffix, actual.Single(x => x.Id.Equals(15)).Name);
            Assert.AreEqual("�W�٪����Ѳ�.�v��.��ԹڷQ��" + suffix, actual.Single(x => x.Id.Equals(16)).Name);
            Assert.AreEqual("��ݪ���Ա��L���N" + suffix, actual.Single(x => x.Id.Equals(19)).Name);
        }
    }
}

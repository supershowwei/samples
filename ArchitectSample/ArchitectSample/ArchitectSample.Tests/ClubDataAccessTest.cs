using System.Reflection;
using System.Threading.Tasks;
using ArchitectSample.Physical.DataAccesses;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;
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
    }
}

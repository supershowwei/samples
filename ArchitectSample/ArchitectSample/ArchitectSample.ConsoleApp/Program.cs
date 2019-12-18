using System;
using System.Threading.Tasks;
using ArchitectSample.Physical.DataAccesses;
using ArchitectSample.Protocol.Model.Data;
using ArchitectSample.Protocol.Physical;

namespace ArchitectSample.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25);

            Console.WriteLine(club.Name);
            Console.Read();
        }
    }
}

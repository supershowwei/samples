using System.Threading.Tasks;
using Chef.Extensions.DbAccess;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectSample.WebApp.Areas.Club.Controllers
{
    [Area("Club")]
    [Route("clubs")]
    public class HomeController : Controller
    {
        private readonly IDataAccess<Protocol.Model.Data.Club> clubDataAccess;

        public HomeController(IDataAccess<Protocol.Model.Data.Club> clubDataAccess)
        {
            this.clubDataAccess = clubDataAccess;
        }

        [HttpGet("{clubId:int}")]
        public async Task<IActionResult> Index(int clubId)
        {
            var club = await this.clubDataAccess.QueryOneAsync(x => x.Id.Equals(clubId), selector: x => new { x.Id, x.Name });

            this.ViewBag.ClubId = club.Id;

            return View();
        }
    }
}
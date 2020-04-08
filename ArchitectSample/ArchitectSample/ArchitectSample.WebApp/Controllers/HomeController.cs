using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Logic;
using ArchitectSample.WebApp.Models;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ArchitectSample.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IClubService clubService;

        public HomeController(ILogger<HomeController> logger, ILifetimeScope lifetimeScope)
        {
            this.logger = logger;
            this.lifetimeScope = lifetimeScope;

            //this.clubService = lifetimeScope.ResolveNamed<IClubService>("abc");
            this.clubService = lifetimeScope.Resolve<IClubService>();
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Privacy()
        {
            var clubArticles = await this.clubService.ListArticlesAsync(1, DateTime.Now, DateTime.Now);

            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
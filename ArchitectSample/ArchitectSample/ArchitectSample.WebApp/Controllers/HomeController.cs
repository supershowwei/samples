using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArchitectSample.Protocol.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ArchitectSample.WebApp.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;

namespace ArchitectSample.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AutofacServiceProvider serviceProvider;

        public HomeController(ILogger<HomeController> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider as AutofacServiceProvider;
        }

        public IActionResult Index()
        {
            var clubService = this.serviceProvider.LifetimeScope.ResolveNamed<IClubService>("abc");

            clubService = this.serviceProvider.LifetimeScope.Resolve<IClubService>();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

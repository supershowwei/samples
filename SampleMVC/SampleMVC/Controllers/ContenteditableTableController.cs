using System.Web.Mvc;

namespace SampleMVC.Controllers
{
    public class ContenteditableTableController : Controller
    {
        public ActionResult New()
        {
            return this.View();
        }
    }
}
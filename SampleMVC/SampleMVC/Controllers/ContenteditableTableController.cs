using System.Web.Mvc;

namespace SampleMVC.Controllers
{
    public class ContenteditableTableController : Controller
    {
        // GET: ContenteditableTable
        public ActionResult New()
        {
            return this.View();
        }
    }
}
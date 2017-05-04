using System.Web.Mvc;

namespace SampleMVC.Controllers
{
    public class HttpVerbController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPatch]
        public ActionResult Patch(PatchedData data)
        {
            return this.Json(data);
        }
    }

    public class PatchedData
    {
        public string Text { get; set; }
    }
}
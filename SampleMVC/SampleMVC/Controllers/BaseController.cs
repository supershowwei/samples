using System.Text;
using System.Web.Mvc;

namespace SampleMVC.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override JsonResult Json(
            object data,
            string contentType,
            Encoding contentEncoding,
            JsonRequestBehavior behavior)
        {
            if (behavior.Equals(JsonRequestBehavior.DenyGet) && (this.Request != null)
                && string.Equals(this.Request.HttpMethod, "GET"))
            {
                return new JsonResult();
            }

            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}
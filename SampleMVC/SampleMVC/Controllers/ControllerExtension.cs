using System.Net;
using System.Web.Mvc;

namespace SampleMVC.Controllers
{
    public static class ControllerExtension
    {
        public static HttpStatusCodeResult HttpInternalServerError(this Controller me, string statusDescription = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, statusDescription);
        }

        public static HttpStatusCodeResult HttpBadRequest(this Controller me, string statusDescription = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, statusDescription);
        }
    }
}
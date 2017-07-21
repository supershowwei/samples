using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SampleMVC.Controllers
{
    public class RequestUrlController : Controller
    {
        // GET: RequestUrl
        public ActionResult List()
        {
            var uriPartialAuthority = this.Request.Url.GetLeftPart(UriPartial.Authority);
            var uriPartialPath = this.Request.Url.GetLeftPart(UriPartial.Path);
            var uriPartialQuery = this.Request.Url.GetLeftPart(UriPartial.Query);
            var uriPartialScheme = this.Request.Url.GetLeftPart(UriPartial.Scheme);

            var appRelativeCurrentExecutionFilePath = this.Request.AppRelativeCurrentExecutionFilePath;

            this.ViewBag.Urls =
                new Dictionary<string, string>
                    {
                        ["Request.RawUrl"] = this.Request.RawUrl,
                        ["Request.Url.GetLeftPart(UriPartial.Authority)"] = uriPartialAuthority,
                        ["Request.Url.GetLeftPart(UriPartial.Path)"] = uriPartialPath,
                        ["Request.Url.GetLeftPart(UriPartial.Query)"] = uriPartialQuery,
                        ["Request.Url.GetLeftPart(UriPartial.Scheme)"] = uriPartialScheme,
                        ["Request.Path"] = this.Request.Path,
                        ["Request.PhysicalPath"] = this.Request.PhysicalPath,
                        ["Request.AppRelativeCurrentExecutionFilePath"] = appRelativeCurrentExecutionFilePath,
                        ["Request.ApplicationPath"] = this.Request.ApplicationPath,
                        ["Request.CurrentExecutionFilePath"] = this.Request.CurrentExecutionFilePath,
                        ["Request.FilePath"] = this.Request.FilePath,
                        ["Request.PhysicalApplicationPath"] = this.Request.PhysicalApplicationPath,
                        ["Request.Url.AbsolutePath"] = this.Request.Url.AbsolutePath,
                        ["Request.Url.AbsoluteUri"] = this.Request.Url.AbsoluteUri,
                        ["Request.Url.Scheme"] = this.Request.Url.Scheme,
                        ["Request.Url.Host"] = this.Request.Url.Host,
                        ["Request.Url.Port"] = this.Request.Url.Port.ToString(),
                        ["Request.Url.Authority"] = this.Request.Url.Authority,
                        ["Request.Url.LocalPath"] = this.Request.Url.LocalPath,
                        ["Request.Url.PathAndQuery"] = this.Request.Url.PathAndQuery,
                        ["Request.Url.Query"] = this.Request.Url.Query,
                        ["Request.Url.Segments"] = string.Join("<br />", this.Request.Url.Segments)
                    }.OrderBy(x => x.Key);

            return this.View();
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SampleMVC.Filters
{
    public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            WriteLog(
                "{0} '{1}' from '{2}' occur exception '{3}'\r\n\r\nCookies:\r\n{4}\r\n\r\nArguments:\r\n{5}\r\n\r\nStackTrace:\r\n{6}",
                filterContext.HttpContext.Request.HttpMethod,
                filterContext.HttpContext.Request.Url.AbsoluteUri,
                GetClientIP(filterContext.HttpContext.Request),
                filterContext.Exception.GetBaseException().Message,
                GetCookies(filterContext.HttpContext.Request),
                TryGetString(filterContext.HttpContext.Request.InputStream),
                filterContext.Exception.StackTrace);

            if (!filterContext.HttpContext.IsDebuggingEnabled)
            {
                filterContext.Result = new ViewResult { ViewName = "~/Views/Error/InternalServerError.cshtml" };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }

        private static string GetCookies(HttpRequestBase request)
        {
            return string.Join(
                "\r\n",
                request.Cookies.AllKeys.Select(key => request.Cookies[key])
                    .Where(x => x != null)
                    .Select(x => $"{x.Name}={x.Value}"));
        }

        private static string GetClientIP(HttpRequestBase request)
        {
            return request.ServerVariables["HTTP_VIA"] == null
                       ? request.ServerVariables["REMOTE_ADDR"]
                       : request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }

        private static string TryGetString(Stream inputStream)
        {
            try
            {
                if (inputStream.Length > 0)
                {
                    using (var sr = new StreamReader(inputStream, Encoding.UTF8))
                    {
                        sr.BaseStream.Seek(0, SeekOrigin.Begin);

                        return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }

        private static void WriteLog(string format, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
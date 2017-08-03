using System;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace SampleMVC.Filters
{
    public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            WriteLog(
                "{0} '{1}' occur exception '{2}'\r\n\r\n Arguments:\r\n{3}\r\n\r\nStackTrace:\r\n{4}",
                filterContext.HttpContext.Request.HttpMethod,
                filterContext.HttpContext.Request.Url.AbsoluteUri,
                filterContext.Exception.GetBaseException().Message,
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
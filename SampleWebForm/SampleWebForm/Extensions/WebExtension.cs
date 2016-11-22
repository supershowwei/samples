using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace SampleWebForm.Extensions
{
    public class WebExtension
    {
        // ASP.NET [C#] REDIRECT WITH POST DATA
        public static void RedirectWithData(NameValueCollection data, string url)
        {
            var response = HttpContext.Current.Response;
            response.Clear();

            var s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<form name='form' action='{0}' method='post'>", url);
            foreach (string key in data)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key, data[key]);
            }

            s.Append("</form></body></html>");
            response.Write(s.ToString());
            response.End();
        }
    }
}
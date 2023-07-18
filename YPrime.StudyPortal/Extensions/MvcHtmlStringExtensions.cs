using System.Web;
using System.Web.Mvc;

namespace YPrime.StudyPortal.Extensions
{
    public static class MvcHtmlStringExtensions
    {
        public static MvcHtmlString DecodeHtml(this MvcHtmlString htmlString)
        {
            MvcHtmlString result = null;

            if (htmlString != null)
            {
                result = new MvcHtmlString(HttpUtility.HtmlDecode(htmlString.ToString()));

            }

            return result;
        }
    }
}
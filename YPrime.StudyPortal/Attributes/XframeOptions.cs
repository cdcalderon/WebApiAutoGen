using System.Web.Mvc;

namespace YPrime.StudyPortal.Attributes
{
    public class XframeOptions : ActionFilterAttribute
    {
        public override void OnResultExecuting(
            ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader(
                "X-Frame-Options", "DENY");
        }
    }
}
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace YPrime.API.Controllers
{
    public class ValidateAntiForgeryAjaxTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            HttpRequestBase request = context.Request;
            //  if (!HttpContext.Current.IsDebuggingEnabled)
            // {
            string cookieToken = "";
            string formToken = "";

            if (request.Headers["RequestVerificationToken"] != null)
            {
                string[] tokens = request.Headers["RequestVerificationToken"].Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            AntiForgery.Validate(cookieToken, formToken);
            // }
        }
    }
}
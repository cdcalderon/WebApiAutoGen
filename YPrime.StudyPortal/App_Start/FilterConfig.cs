using System.Web.Mvc;
using YPrime.StudyPortal.Attributes;

namespace YPrime.StudyPortal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Add a custom authorization filter to all controllers
            // filters.Add(new Filters.EccAuthorizeAttribute());
            filters.Add(new NoCacheAttribute());

            filters.Add(new XframeOptions());

            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}
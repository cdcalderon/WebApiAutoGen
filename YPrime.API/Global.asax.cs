using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;

namespace YPrime.API
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
        }

        private void Session_Start(object sender, EventArgs e)
        {
            var softwareReleaseRepository = DependencyResolver.Current.GetService<ISoftwareReleaseRepository>();
            YPrimeSession.Instance.ConfigurationId =
                Task.Run(async () =>
                {
                    var userSiteIds = new List<Guid>();
                    var userCountryIds = new List<Guid>();

                    var vals = await softwareReleaseRepository.FindLatestConfigurationVersion(userSiteIds, userCountryIds);
                    return vals;
                }).Result;
        }

        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            // cookies get added automatically because of using the YPrimeSession plumbing
            // but they are not needed for the api responses
            HttpContext.Current.Response.Cookies.Clear();
        }
    }
}
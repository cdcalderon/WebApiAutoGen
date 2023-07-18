using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Exceptions;
using YPrime.StudyPortal.Extensions;

namespace YPrime.StudyPortal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FunctionAuthorizationAttribute : AuthorizeAttribute
    {
        public FunctionAuthorizationAttribute()
        {
        }

        public FunctionAuthorizationAttribute(string Name, string Description)
        {
            this.Name = Name;
            this.Description = Description;
        }

        public FunctionAuthorizationAttribute(string Name, string Description, bool IsBlinded)
        {
            this.Name = Name;
            this.Description = Description;
            this.IsBlinded = IsBlinded;
        }

        public string Name { get; }
        public string Description { get; }
        public bool IsBlinded { get; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = false;

            if (httpContext != null && base.AuthorizeCore(httpContext))
            {
                var currentUser = YPrimeSession.Instance?.CurrentUser;
               
                authorized = currentUser != null && currentUser.HasPermission(Name);

                if (currentUser != null && CheckForStudyUserDeactivation(currentUser.Id))
                {
                    authorized = false;
                    throw new StudyAccessDeactivatedException();
                }
                
                if (PreventAccessForNoReleases())
                {
                    authorized = false;
                }
            }

            return authorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        Error = "403 - Forbidden",
                        LogOnUrl = ConfigurationManager.AppSettings["Auth.PostLogoutRedirectUri"]
            },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.HttpContext.Response.End();
            }
            else if (PreventAccessForNoReleases())
            {
                var redirectResult = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {  "action", "Index" },
                    { "controller", "SoftwareRelease" }
                });

                filterContext.Result = redirectResult;
            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.HttpContext.Response.Status = "403 - Forbidden";
                filterContext.HttpContext.Response.End();
            }
        }

        // Check to make sure that the user is not deactivated and removed from the study.
        private bool CheckForStudyUserDeactivation(Guid userId)
        {           
            var userRepo = DependencyResolver.Current.GetService<IUserRepository>();       
            var studyUserRoles = userRepo?.GetStudyUserRoles(userId);         
            return (studyUserRoles == null || studyUserRoles.Count == 0); 
        }

        private bool PreventAccessForNoReleases()
        {
            var result = YPrimeSession.Instance?.ConfigurationId == Config.Defaults.ConfigurationVersions.InitialVersion.Id &&
                    Name != "CanViewSoftwareReleases";

            return result;
        }
    }
}
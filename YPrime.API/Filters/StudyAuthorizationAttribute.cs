using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace YPrime.API.Filters
{
    public class StudyAuthorizationAttribute :  AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                var authToken = actionContext.Request.Headers
                    .Authorization.Parameter;

                var decodeauthToken = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(authToken));

                var studyId = decodeauthToken.Split(':')[0];

                if (IsAuthorizedRequest(studyId))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(
                           new GenericIdentity(studyId), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        private bool IsAuthorizedRequest(string studyId)
        {
            return studyId.ToLower() == ConfigurationManager.AppSettings["StudyId"].ToLower();
        }
    }
}
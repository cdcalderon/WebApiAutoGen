using Hangfire;
using Hangfire.SqlServer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YPrime.StudyPortal;
using YPrime.StudyPortal.Activators;
using YPrime.StudyPortal.Support;

[assembly: OwinStartup("eCOA-portal", typeof(Startup))]

namespace YPrime.StudyPortal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Auth0 parameters
            string auth0Domain = ConfigurationManager.AppSettings["Auth.Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["Auth.ClientId"];
            string auth0RedirectUri = ConfigurationManager.AppSettings["Auth.RedirectUri"];
            string auth0PostLogoutRedirectUri = ConfigurationManager.AppSettings["Auth.PostLogoutRedirectUri"];

            string[] silentAuthPaths = new string[] { "/Patient/CreateForParticipant" };

            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieSameSite = SameSiteMode.Lax,
                // More information on why the CookieManager needs to be set can be found here: 
                // https://github.com/aspnet/AspNetKatana/wiki/System.Web-response-cookie-integration-issues
                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager())
            });

            //Hangfire setup
            GlobalConfiguration.Configuration.UseActivator(new CustomJobActivator());

            JobStorage.Current = new SqlServerStorage("StudyContext");

            app.UseHangfireServer();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });

            string silentAuthRedirectUrl = string.Empty;            

            // Configure Auth0 authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "Auth0",
                Authority = $"https://{auth0Domain}",
                ClientId = auth0ClientId,
                RedirectUri = auth0RedirectUri,
                PostLogoutRedirectUri = auth0PostLogoutRedirectUri,                

                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },
                ResponseType = "code id_token",                
                Scope = "offline_access openid profile email",
                // More information on why the CookieManager needs to be set can be found here: 
                // https://docs.microsoft.com/en-us/aspnet/samesite/owin-samesite
                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),                

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        // if we failed to authenticate without prompt
                        if (context.ProtocolMessage.Error == "login_required")
                        {
                            context.HandleResponse();
                            context.Response.Redirect(auth0RedirectUri);                 
                        }

                        context.HandleResponse();                      
                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = notification =>
                    {
                        notification.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", notification.ProtocolMessage.IdToken));

                        if (silentAuthRedirectUrl!= string.Empty)
                        {
                            // Add the identity to the owinContext so we can validate it later on when we redirect from silent Auth
                            if (!notification.OwinContext.Authentication.User.Identities.Contains(notification.AuthenticationTicket.Identity))
                            {
                                notification.OwinContext.Authentication.User.AddIdentity(notification.AuthenticationTicket.Identity);
                            }

                            notification.Response.Redirect(silentAuthRedirectUrl);
                            notification.HandleResponse();
                        }

                        silentAuthRedirectUrl = string.Empty;
                        return Task.FromResult(0);
                    },
                    RedirectToIdentityProvider = notification =>
                    {
                      
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var logoutUri = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}";

                            var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = notification.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                            }
                            notification.ProtocolMessage.Prompt = "none";

                            notification.Response.Redirect(logoutUri);
                            notification.HandleResponse();
                        }

                        if (notification.Request.Path.HasValue && silentAuthPaths.Contains(notification.Request.Path.Value))
                        {
                            var claimsIdentity = ClaimsPrincipal.Current.Identities.FirstOrDefault(c => c.AuthenticationType == "Cookies");

                            string idToken = claimsIdentity?.FindFirst(x => x.Type == "id_token")?.Value;                          
 
                            // if havent already authed, 
                            if (string.IsNullOrEmpty(idToken))
                            {                               
                                silentAuthRedirectUrl = notification.Request.Uri.ToString();

                                notification.ProtocolMessage.SetParameter("prompt", "none");
                                                          }
                        }
                        return Task.FromResult(0);
                    }
                }
            });
        }
     
        public class MyRestrictiveAuthorizationFilter : Hangfire.Dashboard.IAuthorizationFilter
        {
            public bool Authorize(IDictionary<string, object> owinEnvironment)
            {
                // In case you need an OWIN context, use the next line,
                // `OwinContext` class is the part of the `Microsoft.Owin` package.
                var context = new OwinContext(owinEnvironment);

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return context.Authentication.User.Identity.IsAuthenticated;
            }
        }
    }
}
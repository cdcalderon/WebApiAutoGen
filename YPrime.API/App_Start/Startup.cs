using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System.Configuration;
using YPrime.API.Support;

[assembly: OwinStartup(typeof(YPrime.API.App_Start.Startup))]

namespace YPrime.API.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            var domain = ConfigurationManager.AppSettings["Auth.Domain"];
            var apiIdentifier = ConfigurationManager.AppSettings["Auth.ApiIdentifier"];

            var keyResolver = new OpenIdConnectSigningKeyResolver($"https://{domain}");
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = apiIdentifier,
                        ValidIssuer = $"https://{domain.TrimEnd('/')}/",
                        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid)
                    }
                });

            // Configure Web API
            WebApiConfig.Configure(app);
        }
    }
}

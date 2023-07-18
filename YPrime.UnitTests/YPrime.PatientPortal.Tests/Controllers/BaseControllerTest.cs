using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers
{
    public abstract class BaseControllerTest : BaseTest
    {
        protected YPrimeSession _yprimeSession = new YPrimeSession();
        protected Mock<ISessionService> MockSessionService = new Mock<ISessionService>();

        protected HttpContext GetMockedHttpContext()
        {
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");

            var context = new HttpContext(
                new HttpRequest(string.Empty, "http://tempuri.org", string.Empty),
                new HttpResponse(new StringWriter()));

            context.User = user.Object;

            var sessionContainer = new HttpSessionStateContainer(
                "id",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10, 
                true, 
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, 
                false);

            SessionStateUtility.AddHttpSessionStateToContext(context, sessionContainer);

            context.Session.Add("YPrimeSessionInstance", _yprimeSession);

            return context;
        }

        [TestInitialize]
        public virtual void Initialize()
        {
            HttpContext.Current = GetMockedHttpContext();
        }
    }
}
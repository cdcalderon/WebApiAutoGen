using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.API.Filters
{
    public class DataSyncAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IDeviceRepository _DeviceRepository;

        public DataSyncAuthorizeAttribute(IDeviceRepository deviceRepository)
        {
            _DeviceRepository = deviceRepository;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            HttpRequestBase request = context.Request;
            //  if (!HttpContext.Current.IsDebuggingEnabled)
            // {
            string Id = "";
            string SyncToken = "";

            if (request.Headers["RequestVerificationToken"] != null)
            {
                string[] tokens = request.Headers["RequestVerificationToken"].Split(':');
                if (tokens.Length == 2)
                {
                    Id = tokens[0].Trim();
                    SyncToken = tokens[1].Trim();
                }
            }

            var IsAuthenticated = _DeviceRepository.AuthenticateDevice(Id, SyncToken);
        }
    }
}
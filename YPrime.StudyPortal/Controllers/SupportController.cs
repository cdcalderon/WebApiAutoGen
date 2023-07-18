using System;
using System.Net.Http;
using System.Web.Mvc;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.StudyPortal.Controllers
{
    public class SupportController : BaseController
    {
        public SupportController(
            ISessionService sessionService)
            : base(sessionService)
        { }

        public ActionResult IsSupportChatAvailable()
        {
            var result = false;
            var SupportChatURL = YPrimeSession.Instance.SupportChatURL;
            if (!string.IsNullOrEmpty(SupportChatURL) && YPrimeSession.Instance.SupportChatEnabled)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.Timeout = TimeSpan.FromSeconds(5);
                        var response = client.GetAsync(SupportChatURL).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        result = false;
                    }
                }
            }

            return Json(new {isSupportChatAvailable = result}, JsonRequestBehavior.AllowGet);
        }
    }
}
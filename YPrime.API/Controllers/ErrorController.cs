using System.Web;
using System.Web.Http;
using Elmah;

namespace YPrime.API.Controllers
{
    public class ErrorController : ApiController
    {
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [HttpHead]
        [HttpOptions]
        public IHttpActionResult NotFound(string path)
        {
            // log error to ELMAH
            if (HttpContext.Current != null)
            {
                ErrorSignal.FromCurrentContext().Raise(new HttpException(404, "404 Not Found: /" + path));
            }

            // return 404
            return NotFound();
        }
    }
}
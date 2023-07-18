using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using YPrime.API.Filters;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.API.Controllers
{
    public class StudyController : ApiController
    {
        private readonly ILanguageService _languageService;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IDataCopyRepository _dataCopyRepository;

        public StudyController(
            ILanguageService languageService,
            ISystemSettingRepository systemSettingRepository,
            IDataCopyRepository dataCopyRepository
            )
        {
            _languageService = languageService;
            _systemSettingRepository = systemSettingRepository;
            _dataCopyRepository = dataCopyRepository;
        }

        [Route("api/Study/AppetizeKey")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetAppetizeKey(string deviceType)
        {
            string result;
            if (deviceType.ToLower() == "handheld")
                result = _systemSettingRepository.GetSystemSettingValue("WebBackupHandheldPublicKey");
            else if (deviceType.ToLower() == "tablet")
                result = _systemSettingRepository.GetSystemSettingValue("WebBackupTabletPublicKey");
            else
                return NotFound();

            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { key = result }))
            };
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return ResponseMessage(response);
        }

        [Route("api/Study/SystemSetting")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddSystemSetting(SystemSetting systemSetting)
        {
            if (systemSetting == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"{nameof(systemSetting)} cannot be null.");
            }

            var id = _systemSettingRepository.AddSystemSetting(systemSetting);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        [Route("api/Study/Languages")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetLanguages()
        {
            var languages = await _languageService.GetAll();
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(languages
                    .Select(x => new { culture = x.CultureName, name = x.Notes })))
            };
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return ResponseMessage(response);
        }

        [Route("api/Study/StudyPortalConfigData")]
        [AcceptVerbs("GET")]
        [StudyAuthorization]
        public async Task<IHttpActionResult> GetStudyPortalConfigData()
        {
            var resultData = await _dataCopyRepository.GetStudyPortalConfigData();

            var response = Ok(resultData);

            return response;
        }

        [Route("api/Study/StudyPortalConfigData")]
        [AcceptVerbs("POST")]
        [StudyAuthorization]
        public async Task<IHttpActionResult> UpdateStudyPortalConfigData(
            [FromBody] StudyPortalConfigDataDto model)
        {
            await _dataCopyRepository.UpdateStudyPortalConfigData(model);

            var response = Ok();

            return response;
        }


    }
}
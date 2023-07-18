using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary.ApiDtos;
using YPrime.eCOA.DTOLibrary.ViewModel;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.API.Controllers
{
    public class DeviceManagementController : BaseApiController
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISoftwareVersionRepository _softwareVersionRepository;
        private readonly ISyncLogRepository _syncLogRepository;

        public DeviceManagementController(ISiteRepository siteRepository, 
            IDeviceRepository deviceRepository,
            ISoftwareVersionRepository softwareVersionRepository, 
            ISyncLogRepository syncLogRepository, 
            ISessionService sessionService) : base(sessionService)
        {
            _siteRepository = siteRepository;
            _deviceRepository = deviceRepository;
            _syncLogRepository = syncLogRepository;
            _softwareVersionRepository = softwareVersionRepository;
        }

        /// <summary>
        /// Gets a list of sites from the database for consumption by a device.
        /// </summary>
        [Route("api/DeviceManagement/GetSites")]        
        public IHttpActionResult GetSites() // This is going to be a study specific API
        {
            var SiteList = _siteRepository.GetAllSitesAsDynamic();
            return Created("DeviceConfiguration", SiteList);
        }

        [AcceptVerbs("POST")]
        [Route("api/DeviceManagement/AssociateDeviceToSite")]
        [Authorize]
        public IHttpActionResult
            AssociateDeviceToSite(Guid DeviceId, string SiteId) // This is going to be a study specific API
        {
            var SiteList = _siteRepository.GetAllSitesAsDynamic();
            return Created("DeviceConfiguration", SiteList);
        }

        [Route("api/DeviceManagement/HealthCheck")]
        public HttpResponseMessage HealthCheck()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AcceptVerbs("POST")]
        [Route("api/DeviceManagement/AddSoftwareVersion")]
        public HttpResponseMessage AddSoftwareVersion(SoftwareVersion softwareVersion)
        {
            bool success = false;

            if (softwareVersion != null)
            {
                var versionExists = _softwareVersionRepository.CheckVersionNumberIsUsed(softwareVersion.VersionNumber);

                Version version;

                if(Version.TryParse(softwareVersion.VersionNumber, out version))
                {
                    if (!versionExists)
                    {
                        softwareVersion.Id = Guid.NewGuid();
                        softwareVersion.PlatformTypeId = PlatformType.Android.Id;

                        var versionAdded = _softwareVersionRepository.AddSoftwareVersion(softwareVersion);
                    }

                    // Only return false for an invalid version
                    success = true;
                }
            }

            var statusCode = success ? HttpStatusCode.OK : HttpStatusCode.BadRequest;

            return Request.CreateResponse(statusCode);
        }

        [AcceptVerbs("POST")]
        [Route("api/DeviceManagement/Post")]
        [Authorize]
        public HttpResponseMessage Post(AddUpdateDeviceDto AddUpdateDeviceDto)
        {
            var response = new HttpResponseMessage();
            var result = new ApiRequestResultViewModel();
            _deviceRepository.AddUpdateDevice(AddUpdateDeviceDto, result);

            if (result.WasSuccessful)
            {
                response = Request.CreateResponse(HttpStatusCode.OK,
                    $"device {AddUpdateDeviceDto.AssetTag} successfully added to study");
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error);
                }

                response = Request.CreateResponse(HttpStatusCode.BadRequest, sb.ToString());
            }

            return response;
        }

        [Route("api/DeviceManagement/GetDeviceTypes")]
        [Authorize]
        public IHttpActionResult GetDeviceTypes()
        {
            var deviceTypes = _deviceRepository.GetAllDeviceTypes();
            return Created("DeviceConfiguration", deviceTypes);
        }

        [Route("api/DeviceManagement/GetDeviceLastSyncDate")]
        [Authorize]
        public IHttpActionResult GetDeviceLastSyncDate(string assetTag)
        {
            var syncDate = _syncLogRepository.GetLastSyncDateFromLogsByDevice(assetTag);
            return Ok(syncDate);
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary.ApiDtos;

namespace YPrime.API.Controllers
{
    public class AnalyticsController : ApiController
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        [Route("api/Analytics/AnalyticsReference")]
        [AcceptVerbs("POST")]
        [Authorize]
        public HttpResponseMessage AddAnalyticsReference(AnalyticsReferenceInputDto analyticsReferenceInput)
        {
            if (analyticsReferenceInput == null || 
                string.IsNullOrEmpty(analyticsReferenceInput.InternalName) ||
                string.IsNullOrEmpty(analyticsReferenceInput.DisplayName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Invalid {nameof(AnalyticsReference)}");
            }

            var analyticsReference = new AnalyticsReference()
            {
                Id = Guid.NewGuid(),
                InternalName = analyticsReferenceInput.InternalName,
                DisplayName = analyticsReferenceInput.DisplayName,
                SponsorReport = analyticsReferenceInput.SponsorReport.HasValue && analyticsReferenceInput.SponsorReport.Value
            };

            try
            {
                var id = _analyticsRepository.AddAnalyticsReference(analyticsReference);
                return Request.CreateResponse(HttpStatusCode.OK, id);
            }
            catch (DuplicateAnalyticsException de)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, de.Message);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Route("api/Analytics/DeleteReportByInternalName")]
        [AcceptVerbs("POST")]
        [Authorize]
        public HttpResponseMessage DeleteReportByInternalName(string internalName)
        {
            try
            {
                if (string.IsNullOrEmpty(internalName))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, false);
                }

                var entity = _analyticsRepository.GetAllAnalyticsReferences().FirstOrDefault(x => x.InternalName == internalName);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, false);
                }

                bool reportRemoved = _analyticsRepository.RemoveReportByInitialName(internalName);
                return Request.CreateResponse(HttpStatusCode.OK, reportRemoved);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Updates a report internal and/or display name
        /// </summary>
        /// <param name="internalName">Internal name of the report to be updated</param>
        /// <param name="updatedInternalName">Updated report internal name, will be ignored if empty or null</param>
        /// <param name="updatedDisplayName">Optional, updated report display name, will be ignored if empty or null</param>
        /// <returns>Response with status code</returns>
        [Route("api/Analytics/UpdateReportName")]
        [AcceptVerbs("POST")]
        [Authorize]
        public HttpResponseMessage UpdateReportName(string internalName, string updatedInternalName, string updatedDisplayName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(internalName) || (string.IsNullOrEmpty(updatedInternalName) && string.IsNullOrEmpty(updatedDisplayName)))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, false);
                }

                if(!string.IsNullOrEmpty(updatedInternalName))
                {
                    var checkUnique = _analyticsRepository.GetAllAnalyticsReferences().FirstOrDefault(x => x.InternalName == updatedInternalName);
                    if(checkUnique != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, false);
                    }
                }

                var updatedReport = _analyticsRepository.UpdateReportName(internalName, updatedInternalName, updatedDisplayName);
                if (updatedReport == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, false);
                }
                return Request.CreateResponse(HttpStatusCode.OK, updatedReport);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
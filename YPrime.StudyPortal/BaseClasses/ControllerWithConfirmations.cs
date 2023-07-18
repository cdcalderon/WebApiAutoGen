using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.StudyPortal.Controllers;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.StudyPortal.BaseClasses
{
    public abstract class ControllerWithConfirmations : BaseController
    {
        protected ControllerWithConfirmations(
            ISessionService sessionService)
            : base(sessionService)
        { }

        public ActionResult DoConfirmation(IDictionary<string, string> data, Guid confirmationTypeId,
            Guid? SiteId = null)
        {
            var confirmationId = Guid.NewGuid();

            TempData.Add(confirmationId.ToString(), new ConfirmationDto
            {
                ConfirmationData = data,
                ConfirmationTypeId = confirmationTypeId,
                SiteId = SiteId,
                UserId = User.Id
            });

            return RedirectToAction("Index", "Confirmation", new RouteValueDictionary {{"id", confirmationId}});
        }

        public async Task<ActionResult> DoConfirmation(EmailDto dto, Guid confirmationTypeId, Guid? siteId = null)
        {
            var confirmationRepository = DependencyResolver.Current.GetService<IConfirmationRepository>();

            var confirmationId = Guid.NewGuid();

            TempData.Add(confirmationId.ToString(), new ConfirmationDto
            {
                ConfirmationData = dto.GetKeyValuesFromProperties(await confirmationRepository.BuildEmailConfiguration()),
                ConfirmationTypeId = confirmationTypeId,
                SiteId = siteId,
                UserId = User.Id
            });

            return RedirectToAction("Index", "Confirmation", new RouteValueDictionary {{"id", confirmationId}});
        }

        public async Task<ActionResult> DoConfirmation(EmailDto dto, IDictionary<string, string> additionalFields,
            Guid confirmationTypeId, Guid? siteId = null)
        {
            var confirmationId = Guid.NewGuid();

            var confirmationData = await BuildConfirmationData(dto, additionalFields);

            TempData.Add(confirmationId.ToString(), new ConfirmationDto
            {
                ConfirmationData = confirmationData,
                ConfirmationTypeId = confirmationTypeId,
                SiteId = siteId,
                UserId = User.Id
            });

            return RedirectToAction("Index", "Confirmation", new RouteValueDictionary {{"id", confirmationId}});
        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                    viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                    ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private async Task<IDictionary<string, string>> BuildConfirmationData(EmailDto dto,
            IDictionary<string, string> additionalFields)
        {
            var confirmationRepository = DependencyResolver.Current.GetService<IConfirmationRepository>();
            var confirmationData = dto.GetKeyValuesFromProperties(await confirmationRepository.BuildEmailConfiguration());

            //override dto properties with additional data
            foreach (var kvp in additionalFields)
            {
                if (confirmationData.ContainsKey(kvp.Key))
                {
                    confirmationData[kvp.Key] = kvp.Value;
                }
                else
                {
                    confirmationData.Add(kvp.Key, kvp.Value);
                }
            }

            return confirmationData;
        }
    }
}
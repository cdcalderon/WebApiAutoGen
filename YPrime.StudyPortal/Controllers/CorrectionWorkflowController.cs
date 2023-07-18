using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.StudyPortal.BaseClasses;

namespace YPrime.StudyPortal.Controllers
{
    public class CorrectionWorkflowController : ControllerWithConfirmations
    {
        private readonly ICorrectionRepository _CorrectionRepository;
        private readonly ITranslationService _translationService;

        public CorrectionWorkflowController(
            ICorrectionRepository correctionRepository, 
            ITranslationService translationService,
            ISessionService sessionService)
            : base(sessionService)
        {
            _CorrectionRepository = correctionRepository;
            _translationService = translationService;
        }

        private string NeedMoreInformationSessionKey = "NeedsMoreInfoKey";
        private bool NeedsMoreInformation
        {
            get { return (bool) Session[NeedMoreInformationSessionKey]; }
            set { Session[NeedMoreInformationSessionKey] = value; }
        }

        // GET: CorrectionWorkflow
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CompletedWorkflow(Guid id)
        {
            var correction = await _CorrectionRepository.GetCorrection(id, CurrentSiteUserCultureCode);
            NeedsMoreInformation = false;
            SetWorkflowViewbag(id, null, "", NeedsMoreInformation);
            return View("~/Views/CorrectionWorkflow/Workflow.cshtml", correction);
        }

        public async Task<ActionResult> Workflow(Guid id)
        {
            var userId = User.Id;
            var userRoles = User.Roles;

            var workflowCorrection = await _CorrectionRepository.GetCorrectionWorkflow(
                id, 
                userRoles,
                userId,
                CurrentSiteUserCultureCode);

            var correction = workflowCorrection.Correction;

            var needsMoreInformation = _CorrectionRepository
                .NeedsMoreInformation(workflowCorrection.Correction);

            NeedsMoreInformation = needsMoreInformation;
            SetWorkflowViewbag(id, null, "", needsMoreInformation);
            ViewBag.PatientNumber = workflowCorrection.PatientNumber;

            return View(correction);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Workflow(Correction correction, Guid correctionId, Guid workflowId,
            Guid? correctionActionId, string discussionComment)
        {
            correction.Id = correctionId;
            foreach (var keyName in ModelState.Keys.Where(x => x.ToUpper().StartsWith("SITE.")).ToList())
            {
                var modelValue = ModelState.First(x => x.Key == keyName).Value;
                modelValue.Errors.Clear();
            }

            ActionResult result = View(correction);
            if (string.IsNullOrWhiteSpace(discussionComment))
            {
                var discussionTranslation = await _translationService.GetByKey("CorrectionMissingNotes");

                ModelState.AddModelError("discussionComment", discussionTranslation);
            }

            if (correctionActionId == null)
            {
                var correctionActionTranslation = await _translationService.GetByKey("CorrectionMissingAction");

                ModelState.AddModelError("correctionActionId", correctionActionTranslation);
            }

            if (ModelState.IsValid)
            {
                var correctionHistoryToAdd = new CorrectionHistory()
                {
                    CorrectionActionId = (Guid)correctionActionId,
                    UserName = User.UserName,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    DateTimeStamp = DateTime.UtcNow.ToString("dd'-'MMM'-'yyyy hh':'mm tt 'UTC'")
                };

                if (NeedsMoreInformation)
                {
                    _CorrectionRepository.UpdateCorrectionWorkFlowNeedsMoreInformation(workflowId,
                        YPrimeSession.Instance.CurrentUser.Id, discussionComment, correctionHistoryToAdd);
                }
                else
                {
                    await _CorrectionRepository.UpdateCorrectionWorkFlow(workflowId, (Guid) correctionActionId,
                        YPrimeSession.Instance.CurrentUser.Id, discussionComment, correctionHistoryToAdd);                    

                    var successTranslation = await _translationService.GetByKey("Success");
                    var correctionUpdatedTranslation = await _translationService.GetByKey("CorrectionUpdatedSuccessfully");

                    SetPopUpMessageOnLoad(
                        successTranslation,
                        correctionUpdatedTranslation);

                    // Format Correction correctly
                    await _CorrectionRepository.FormatControlValues(correction, CorrectionStateInternal.Confirm);

                    var table = RenderRazorViewToString("~/Views/Correction/CorrectionDataDisplay.cshtml", correction);
                    var data = new Dictionary<string, string>();
                    data.Add("VisitQuestionnaireConfirmation", table);

                    return DoConfirmation(data, ConvertCorrectionActionToEmailType(correctionActionId));
                }

                result = RedirectToAction("Index", "Correction", new { });
            }
            else
            {
                //NOTE: This might not work as expected
                correction = ExecuteAsyncActionSynchronously(() => _CorrectionRepository.GetCorrection(correction.Id, CurrentSiteUserCultureCode));
                result = View(correction);
            }

            SetWorkflowViewbag(workflowId, correctionActionId, discussionComment, true);
            return result;
        }

        private Guid ConvertCorrectionActionToEmailType(Guid? correctionActionId)
        {
            var emailType = EmailTypes.DataCorrectionPendingApproval;

            if (correctionActionId == CorrectionActionEnum.Rejected)
            {
                emailType = EmailTypes.DataCorrectionRejected;
            }
            else if (correctionActionId == CorrectionActionEnum.NeedsMoreInformation)
            {
                emailType = EmailTypes.DataCorrectionNeedMoreInformation;
            }
            else if (correctionActionId == CorrectionActionEnum.Approved)
            {
                emailType = EmailTypes.DataCorrectionApproved;
            }

            return emailType;
        }

        private void SetWorkflowViewbag(Guid workFlowId, Guid? correctionActionId, string discussionComment,
            bool needsMoreInformation)
        {
            ViewBag.WorkFlowId = workFlowId;
            ViewBag.DiscussionComment = discussionComment;
            ViewBag.CorrectionActions = _CorrectionRepository.GetCorrectionActions();
            //TODO: refactor this!!!
            ViewBag.CorrectionActionId = needsMoreInformation
                ? Guid.Parse("70DA4CC0-ACE7-45F1-BF8A-DB720131A601")
                : correctionActionId;
            ViewBag.NeedsMoreInformation = needsMoreInformation;
        }
    }
}
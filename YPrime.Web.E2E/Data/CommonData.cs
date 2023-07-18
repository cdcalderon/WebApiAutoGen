using System.Collections.Generic;
using System.Collections.Immutable;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Data
{
    public static class CommonData
    {
        public const string YpApproverGroupID = "b2557b07-9420-41c2-aec4-bde42f2ee7cf";
        public const string QuestionResponseMuchBetterId = "d066b9e2-1a25-47ad-81e0-04560c045293";
        public const string createdDateTime = "Created DateTime";
        public const string StartedDateTime = "Started DateTime";
        public const string CompletedDateTime = "Completed DateTime";
        public const string ChangeSubjectInformation = "Change subject Information";
        public const string RemoveASubject = "Remove a subject";
        public const string MergeSubjects = "Merge subjects";
        public const string ChangeQuestionnaireResponses = "Change questionnaire responses";
        public const string ChangeQuestionnaireInformation = "Change questionnaire information";
        public const string AddPaperQuestionnaire = "Add Paper Questionnaire";
        public const string ChangeSubjectVisit = "Change subject Visit";
        public const string QuestionnaireFormsId = "d40f7628-338a-4e1a-8d24-8dd84a635394";
        public const string InitialSoftwareRelease = "Initial Software Release";
        public const string IntialSoftwareReleaseVersionNumber = "0.0.0.1";
        public const string VersionID = "84D3C71C-AF5C-4281-AAD6-74C630D0F523";
        public const string IntialSoftwareReleaseVersionID = "6559B308-785B-4A87-88D1-0B0F970EBA1F";
        public const string visitIdScreening = "d9ab7df7-151c-472b-8a54-1ae9b6a1411c";
        public const string visitIdTreatment = "aaa67219-a5b8-4355-904a-8c21e7bdba23";
        public const string visitIdEnrollment = "4109f7d7-6835-41ac-98d0-12ce63f3a5ea";
        public const string DefaultPortalUserMappingName = "PortalE2EUser";
        public const string DefaultPortalUserName = "ypportale2e@gmail.com";

        public const string TemperatureDatabaseFormat = "0.0000";

        /* Pre defined visit names */
        public const string ScreeningVisitName = "Screening Visit";
        public const string EnrollmentVisitName = "Enrollment Visit";
        public const string TreatmentVisitName = "Treatment Visit";

        /* Correction types */
        public static Dictionary<string, string> correctionTypes = new Dictionary<string, string>
        {
            {"Change subject Information", "b7f6c079-ab83-426b-92ed-6bd14b13ded0"},
            {"Remove a subject", "3e0f6504-cfde-4681-84dd-a39cfabc0039"},
            {"Merge subjects", "1635ecb6-0c44-4e4c-b120-4dd2743a0860"},
            {"Change questionnaire responses" , "c8a0c278-0685-4e63-9f09-388d832c3d60"},
            {"Change questionnaire information" , "3f1e468c-032b-4c2b-9aa9-1d9cb1a885bf"},
            {"Add Paper Questionnaire", "750c9466-db94-41df-907d-c97f321373c5"},
            {"Change subject Visit", "80fda13c-ee28-4f48-b666-3f2998707c84"},
        };

        /* TranslationKeys */

        public static Dictionary<string, string> translationKeys = new Dictionary<string, string>
        {
            {"Height", "80d5bcc0-e4dd-4bca-ac3f-2cf1b70b1419"},
            {"Weight", "931ba387-70a6-4ebb-92b9-09813a8264ed"},
            {"Gender", "e122cbf8-4a48-459e-8d3a-4b7efb55aa38"},
            {"Date of Birth", "0b7c46d2-fff2-43e6-8fde-d5b9284444ed"},
        };

        public static readonly IReadOnlyList<E2EUser> UserMappings = new List<E2EUser>()
        {
            new E2EUser(DefaultPortalUserMappingName, DefaultPortalUserName, "S1thr3yaQV-%")
        }.AsReadOnly();
    }
}

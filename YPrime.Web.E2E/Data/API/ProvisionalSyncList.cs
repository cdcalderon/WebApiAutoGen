using System.Collections.Generic;
using YPrime.Data.Study.Models;

namespace YPrime.Web.E2E.Data.API
{
    public static class ProvisionalSyncList
    {
        public static List<string> ClientEntries
        {
            get
            {
                return new List<string>
                {
                    nameof(Answer),
                    nameof(AnswerScore),
                    nameof(CareGiver),
                    nameof(Device),
                    nameof(DiaryEntry),
                    nameof(InputFieldTypeResult),
                    nameof(MissedVisitReason),
                    nameof(Patient),
                    nameof(PatientAttribute),
                    nameof(PatientVisit),
                    nameof(QuestionInputFieldTypeResult),
                    nameof(SecurityQuestion),
                    nameof(Site),
                    nameof(SiteLanguage),
                    nameof(StudyUser),
                    nameof(StudyUserRole),
                    nameof(SystemAction),
                    nameof(SystemActionStudyRole)
                };
            }
        }
    }
}

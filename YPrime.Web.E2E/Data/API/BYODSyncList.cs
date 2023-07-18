using System;
using System.Collections.Generic;
using System.Text;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using YPrime.Web.E2E.Models.Api;

namespace YPrime.Web.E2E.Data.API
{
    public static class BYODSyncList
    {
        public static List<string> ClientEntries { get
            {
                return new List<string>
                {
                    nameof(Answer), 
                    nameof(AnswerScore),
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
                    nameof(SiteLanguage)
                };
            }
        }
    }
}

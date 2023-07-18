using System;
using System.Collections.Generic;
using YPrime.Data.Study.Models;

namespace YPrime.Web.E2E.Data.API
{
    public static class HandheldInitialSyncList
    {
        public static List<string> ClientEntries
        {
            get
            {
                return new List<string>
                {
                    nameof(Device),
                    nameof(Patient),
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

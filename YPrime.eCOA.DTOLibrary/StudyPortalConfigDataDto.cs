using System.Collections.Generic;
using YPrime.Data.Study.Models;

namespace YPrime.eCOA.DTOLibrary
{
    public class StudyPortalConfigDataDto
    {
        public List<EmailContentStudyRole> EmailContentStudyRoles { get; set; } = new List<EmailContentStudyRole>();
 
        public List<ReportStudyRole> ReportStudyRoles { get; set; } = new List<ReportStudyRole>();

        public List<StudyRoleUpdate> StudyRoleUpdates { get; set; } = new List<StudyRoleUpdate>();

        public List<StudyRoleWidget> StudyRoleWidgets { get; set; } = new List<StudyRoleWidget>();

        public List<SystemActionStudyRole> SystemActionStudyRoles { get; set; } = new List<SystemActionStudyRole>();

        public List<AnalyticsReference> AnalyticsReferences { get; set; } = new List<AnalyticsReference>();

        public List<AnalyticsReferenceStudyRole> AnalyticsReferenceStudyRoles { get; set; } = new List<AnalyticsReferenceStudyRole>();
    }
}

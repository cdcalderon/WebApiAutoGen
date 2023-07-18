using System.Collections.Generic;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models.DataSync;

namespace YPrime.Web.E2E.Models.Api
{
    public class ApiTestData
    {
        public CheckForUpdatesRequest CheckForUpdatesRequest { get; set; } = new CheckForUpdatesRequest();

        public CheckForUpdatesResponse CheckForUpdatesResponse { get; set; } = new CheckForUpdatesResponse();

        public DataSyncRequest DataSyncRequest { get; set; } = new DataSyncRequest();

        public DataSyncResponse DataSyncResponse { get; set; } = new DataSyncResponse();

        public List<PatientMapping> PatientMappings { get; set; } = new List<PatientMapping>();

        public List<PatientVisitMapping> PatientVisitMappings { get; set; } = new List<PatientVisitMapping>();

        public List<DiaryEntryMapping> DiaryEntryMappings { get; set; } = new List<DiaryEntryMapping>();

        public List<AnswerMapping> AnswerMappings { get; set; } = new List<AnswerMapping>();

        public List<SoftwareVersion> AddedSoftwareVersions { get; set; } = new List<SoftwareVersion>();

        public List<SoftwareRelease> AddedSoftwareReleases { get; set; } = new List<SoftwareRelease>();

        public SSORequestStudy SSORequestStudy { get; set; } = new SSORequestStudy();

        public SSORequestHeaders SSORequestHeaders { get; set; } = new SSORequestHeaders();
    }
}

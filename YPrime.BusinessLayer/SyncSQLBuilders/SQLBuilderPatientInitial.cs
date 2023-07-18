using System;
using System.Linq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.SyncSQLBuilders
{
    public class SQLBuilderPatientInitial : ISyncSQLBuilder
    {
        private readonly IStudyDbContext _db;
        private readonly string _scopedSiteId;
        private readonly string _scopedDeviceId;

        public SQLBuilderPatientInitial(
            IStudyDbContext db,
            string deviceId,
            string siteId)
        {
            _db = db;
            _scopedSiteId = siteId;
            _scopedDeviceId = deviceId;
        }

        public string GetSQLForTable(string TableName, string activePatientStatusIds)
        {
            var SQL = string.Empty;
            var SiteId = _scopedSiteId;
            var DeviceId = _scopedDeviceId;

            switch (TableName)
            {
                case nameof(Device):
                    SQL = $@"select
	                            d.* 
                            from 
	                            Device d
                            where 
	                            d.Id = '{DeviceId}'";
                    break;
                case nameof(Patient):
                    SQL = $@"select p.* From [dbo].Patient p
                           where p.SiteId = '{SiteId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(StudyUser):
                    SQL = $@"select su.*
                            from [dbo].[StudyUser] su
                            where su.Id in
                            (
                                select sur.StudyUserId
                                from [dbo].[StudyUserRole] sur
                                where sur.SiteId = '{SiteId}'
                            )";
                    break;
                case nameof(StudyUserRole):
                    SQL = $"select * from [StudyUserRole] where SiteId = '{SiteId}'";
                    break;
                case nameof(Site):
                    SQL = $"select * from [Site] where Id = '{SiteId}'";
                    break;
                case nameof(SiteLanguage):
                    SQL = $"select * from [SiteLanguage] where SiteId = '{SiteId}'";
                    break;
                case nameof(SystemAction):
                    SQL = "select * from SystemAction where DeviceAction = 1";
                    break;
                case nameof(SystemActionStudyRole):
                    SQL =
                        "select sr.* from systemactionstudyrole sr left join systemaction sa on sa.Id = sr.SystemActionId where sa.DeviceAction = 1";
                    break;
                case nameof(InputFieldTypeResult):
                case nameof(QuestionInputFieldTypeResult):
                case nameof(SecurityQuestion):
                case nameof(ScreenReportDialog):
                    SQL = $"select * from [{TableName}]";
                    break;
                case nameof(MissedVisitReason):
                    SQL = $"select * from [config].[{TableName}]";
                    break;
                case nameof(Answer):
                case nameof(AnswerScore):
                case nameof(CareGiver):
                case nameof(DiaryEntry):
                case nameof(PatientAttribute):
                case nameof(PatientVisit):
                    break;
                default:
                    throw new StudyConfigurationException($"{TableName} not found in sql builder list");
            }

            return (SQL);
        }
    }
}

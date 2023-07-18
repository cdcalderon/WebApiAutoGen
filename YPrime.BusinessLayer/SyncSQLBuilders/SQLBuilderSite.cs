using System;
using System.Linq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.SyncSQLBuilders
{
    public class SQLBuilderSite : ISyncSQLBuilder
    {
        protected readonly IStudyDbContext _db;
        private readonly string _scopedSyncId;

        public SQLBuilderSite(
            IStudyDbContext db,
            string siteId)
        {
            _db = db;
            _scopedSyncId = siteId;
        }

        public string GetSQLForTable(string TableName, string activePatientStatusIds)
        {
            var SQL = string.Empty;
            var SiteId = _scopedSyncId;

            switch (TableName)
            {
                case nameof(Device):
                    SQL = $@"select
	                            d.* 
                            from 
	                            Device d
                            where 
	                            d.SiteId = '{SiteId}'";
                    break;
                case nameof(DiaryEntry):
                    SQL = $@"select
	                            e.* 
                            from 
	                            [dbo].DiaryEntry e
	                            join [dbo].Patient p on e.PatientId = p.id
                             where p.SiteId = '{SiteId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(Answer):
                    SQL = $@"select 
	                            a.* 
                            from 
	                            [dbo].Answer a
	                            join [dbo].DiaryEntry e
		                            on e.Id = a.DiaryEntryId
	                            join [dbo].Patient p
	                                on e.PatientId = p.id
                             where p.SiteId = '{SiteId}' and p.PatientStatusTypeId in ({activePatientStatusIds}) and a.IsArchived=0";
                    break;
                case nameof(Patient):
                    SQL = $@"select p.* From [dbo].Patient p
                           where p.SiteId = '{SiteId}'";
                    break;
                case nameof(PatientAttribute):
                    SQL = $@"Select pa.* From [dbo].PatientAttribute pa join [dbo].Patient p on p.Id = pa.PatientId 
                          where p.SiteId = '{SiteId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(CareGiver):
                    SQL = $@"Select c.* From [dbo].CareGiver c join [dbo].Patient p on p.Id = c.PatientId 
                          where p.SiteId = '{SiteId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(PatientVisit):
                    SQL = $@"Select pv.* From [dbo].PatientVisit pv join [dbo].Patient p on p.Id = pv.PatientId 
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
                case nameof(AnswerScore):
                case nameof(InputFieldTypeResult):
                case nameof(QuestionInputFieldTypeResult):
                case nameof(SecurityQuestion):
                case nameof(ScreenReportDialog):
                    SQL = $"select * from [{TableName}]";
                    break;
                case nameof(MissedVisitReason):
                    SQL = $"select * from [config].[{TableName}]";
                    break;
                default:
                    throw new StudyConfigurationException($"{TableName} not found in sql builder list");
            }

            return (SQL);
        }
    }
}
using System;
using System.Linq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.SyncSQLBuilders
{
    public class SQLBuilderPatient : ISyncSQLBuilder
    {
        private readonly IStudyDbContext _db;
        private readonly string _scopedSyncId;

        public SQLBuilderPatient(
            IStudyDbContext db,
            string patientId)
        {
            _db = db;
            _scopedSyncId = patientId;
        }

        public string GetSQLForTable(string TableName, string activePatientStatusIds)
        {
            var SQL = string.Empty;
            var PatientId = _scopedSyncId;

            switch (TableName)
            {
                case nameof(DiaryEntry):
                    SQL = $@"select
                                e.* 
                            from 
                                [dbo].DiaryEntry e
                                join [dbo].Patient p on e.PatientId = p.id
                            where p.Id = '{PatientId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(Answer):
                    SQL = $@"select 
                               a.* 
                            from 
                               [dbo].Answer a
                               join [dbo].DiaryEntry e on e.Id = a.DiaryEntryId
                               join [dbo].Patient p on e.PatientId = p.id
                            where p.Id = '{PatientId}' and p.PatientStatusTypeId in ({activePatientStatusIds}) and a.IsArchived=0";
                    break;
                case nameof(Patient):
                    SQL = $@"Select p.* From [dbo].Patient p
                            where p.Id = '{PatientId}'";
                    break;
                case nameof(PatientAttribute):
                    SQL = $@"Select pa.* From [dbo].PatientAttribute pa join [dbo].Patient p on p.Id = pa.PatientId 
                            where p.Id = '{PatientId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(CareGiver):
                    SQL = $@"Select c.* From [dbo].CareGiver c join Patient p on p.Id = c.PatientId 
                           where p.Id = '{PatientId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(PatientVisit):
                    SQL = $@"Select pv.* From [dbo].PatientVisit pv join [dbo].Patient p on p.Id = pv.PatientId 
                           where p.Id = '{PatientId}' and p.PatientStatusTypeId in ({activePatientStatusIds})";
                    break;
                case nameof(Site):
                    SQL = $"select s.* from [Site] s where Id in (Select SiteId from Patient where id = '{PatientId}')";
                    break;
                case nameof(StudyUser):
                    SQL = $@"select su.*
                            from [dbo].[StudyUser] su
                            where su.Id in
                            (
                                select sur.StudyUserId
                                from [dbo].[StudyUserRole] sur
                                join [dbo].[Patient] p on p.SiteId = sur.SiteId
                                where p.Id = '{PatientId}'
                            )";
                    break;
                case nameof(StudyUserRole):
                    SQL =
                        $"select s.* from [StudyUserRole] s where s.SiteId in (Select SiteId from Patient where id = '{PatientId}')";
                    break;
                case nameof(SiteLanguage):
                    SQL =
                        $"select s.* from [SiteLanguage] s where s.SiteId in (Select SiteId from Patient where id = '{PatientId}')";
                    break;
                case nameof(SystemAction):
                    SQL = "select * from SystemAction where DeviceAction = 1";
                    break;
                case nameof(SystemActionStudyRole):
                    SQL =
                        "select sr.* from systemactionstudyrole sr left join systemaction sa on sa.Id = sr.SystemActionId where sa.DeviceAction = 1";
                    break;
                case nameof(Device):
                    SQL = $"select * from Device where PatientId='{PatientId}'";
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
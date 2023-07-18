using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using YPrime.Data.Study.Proxies;

namespace YPrime.Data.Study
{
    public interface IStudyDbContext
    {
        Guid? CorrectionId { get; set; }

        DbSet<SiteLanguage> SiteLanguages { get; set; }
        DbSet<Answer> Answers { get; set; }
        DbSet<AnalyticsReference> AnalyticsReferences { get; set; }
        DbSet<AnalyticsReferenceStudyRole> AnalyticsReferenceStudyRoles { get; set; }
        DbSet<CareGiver> CareGivers { get; set; }
        DbSet<CorrectionApprovalData> CorrectionApprovalDatas { get; set; }
        DbSet<CorrectionApprovalDataAdditional> CorrectionApprovalDataAdditionals { get; set; }
        DbSet<CorrectionDiscussion> CorrectionDiscussions { get; set; }
        DbSet<Correction> Corrections { get; set; }
        DbSet<CorrectionStatus> CorrectionStatuses { get; set; }
        DbSet<CorrectionHistory> CorrectionHistories { get; set; }
        DbSet<CorrectionAction> CorrectionActions { get; set; }
        DbSet<CorrectionWorkflow> CorrectionWorkflows { get; set; }
        DbSet<DataApproval> DataApprovals { get; set; }
        DbSet<DCFRequest> DCFRequest { get; set; }
        DbSet<Device> Devices { get; set; }
        DbSet<DeviceData> DeviceDatas { get; set; }
        DbSet<DeviceSession> DeviceSessions { get; set; }
        DbSet<DiaryEntry> DiaryEntries { get; set; }
        DbSet<EmailRecipient> EmailRecipients { get; set; }
        DbSet<EmailContent> EmailContents { get; set; }
        DbSet<EmailSent> EmailSents { get; set; }
        DbSet<Export> Exports { get; set; }
        DbSet<PatientAttribute> PatientAttributes { get; set; }
        DbSet<Patient> Patients { get; set; }
        DbSet<PatientVisit> PatientVisits { get; set; }
        DbSet<ReferenceMaterial> ReferenceMaterials { get; set; }
        DbSet<ReportStudyRole> ReportStudyRoles { get; set; }
        DbSet<EmailContentStudyRole> EmailContentStudyRoles { get; set; }
        DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        DbSet<Site> Sites { get; set; }
        DbSet<SiteActiveHistory> SiteActiveHistory { get; set; }
        DbSet<SoftwareVersion> SoftwareVersions { get; set; }
        DbSet<SoftwareRelease> SoftwareReleases { get; set; }
        DbSet<SoftwareReleaseCountry> SoftwareReleaseCountry { get; set; }
        DbSet<SoftwareReleaseDeviceType> SoftwareReleaseDeviceTypes { get; set; }
        DbSet<StudyRoleUpdate> StudyRoleUpdates { get; set; }
        DbSet<StudyRoleWidget> StudyRoleWidgets { get; set; }
        DbSet<StudyUser> StudyUsers { get; set; }
        DbSet<StudyUserRole> StudyUserRoles { get; set; }
        DbSet<StudyUserWidget> StudyUserWidgets { get; set; }
        DbSet<SyncLog> SyncLogs { get; set; }
        DbSet<SystemAction> SystemActions { get; set; }
        DbSet<SystemActionStudyRole> SystemActionStudyRoles { get; set; }
        DbSet<SystemSetting> SystemSettings { get; set; }
        DbSet<VisitComplianceReportQuestionnaireType> VisitComplianceReportQuestionnaireTypes { get; set; }
        DbSet<Widget> Widgets { get; set; }
        DbSet<WidgetCount> WidgetCounts { get; set; }
        DbSet<WidgetLink> WidgetLinks { get; set; }
        DbSet<WidgetSystemAction> WidgetSystemActions { get; set; }
        DbSet<MissedVisitReason> MissedVisitReasons { get; set; }
        DbSet<AdditionalTablesToSync> AdditionalTablesToSync { get; set; }
        DbSet<ScreenReportDialog> ScreenReportDialog { get; set; }
        DbSet<InputFieldTypeResult> InputFieldTypeResults { get; set; }
        DbSet<QuestionInputFieldTypeResult> QuestionInputFieldTypeResults { get; set; }
        DbSet<NotificationRequest> NotificationRequests { get; set; }

        Database Database { get; }

        void LoadPropertyFromDb<TEntity, TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class;

        int SaveChanges(string userId);

        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync(string username);

        void SaveDeviceAudits(List<dynamic> deviceAuditEntries);

        void Dispose();

        List<T> ExecuteSqlToList<T>(string sql, params object[] parameters);
        IEnumerable<dynamic> CollectionFromSqlStoredProcedure(string storedProcedure, Dictionary<string, object> parameters);

        void DetatchEntity<TEntity>(TEntity entity) where TEntity : class;

        IDbContextTransactionProxy BeginTransaction();
        IDbContextTransactionProxy BeginTransaction(IsolationLevel isolationLevel);

        Guid? YpStudyUserIdFromHeader { get; set; }
    }
}

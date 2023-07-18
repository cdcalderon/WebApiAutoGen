namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OmniAuditReplacementDateTime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AdditionalTablesToSync", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Answer", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AnswerScore", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DiaryEntry", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CareGiver", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Patient", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Correction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CorrectionApprovalData", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CorrectionApprovalDataAdditional", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CorrectionDiscussion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CorrectionAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.StudyUser", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ReferenceMaterial", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.StudyUserRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Site", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Export", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SiteLanguage", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SoftwareRelease", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SoftwareVersion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.StudyUserWidget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Widget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.StudyRoleWidget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.WidgetCount", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.WidgetLink", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.WidgetSystemAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SystemAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SystemActionStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CorrectionHistory", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("config.CorrectionStatus", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CorrectionWorkflow", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Device", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeviceData", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SyncLog", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PatientAttribute", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PatientVisit", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("config.MissedVisitReason", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SecurityQuestion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DataApproval", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DCFRequest", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeviceSession", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("config.EmailContent", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmailContentStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmailRecipient", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmailSent", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.QuestionInputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NotificationRequest", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ReportStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ScreenReportDialog", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SoftwareReleaseCountry", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SoftwareReleaseDeviceType", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.StudyRoleUpdate", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SystemSetting", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.VisitComplianceReportQuestionnaireType", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VisitComplianceReportQuestionnaireType", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SystemSetting", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.StudyRoleUpdate", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SoftwareReleaseDeviceType", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SoftwareReleaseCountry", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ScreenReportDialog", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ReportStudyRole", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.NotificationRequest", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.QuestionInputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EmailSent", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EmailRecipient", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EmailContentStudyRole", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("config.EmailContent", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeviceSession", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DCFRequest", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DataApproval", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SecurityQuestion", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("config.MissedVisitReason", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PatientVisit", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PatientAttribute", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SyncLog", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeviceData", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Device", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CorrectionWorkflow", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("config.CorrectionStatus", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CorrectionHistory", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SystemActionStudyRole", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SystemAction", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.WidgetSystemAction", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.WidgetLink", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.WidgetCount", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.StudyRoleWidget", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Widget", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.StudyUserWidget", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SoftwareVersion", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SoftwareRelease", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SiteLanguage", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Export", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Site", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.StudyUserRole", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ReferenceMaterial", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.StudyUser", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CorrectionAction", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CorrectionDiscussion", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CorrectionApprovalDataAdditional", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CorrectionApprovalData", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Correction", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Patient", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CareGiver", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DiaryEntry", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AnswerScore", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Answer", "LastModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AdditionalTablesToSync", "LastModified", c => c.DateTime(nullable: false));
        }
    }
}

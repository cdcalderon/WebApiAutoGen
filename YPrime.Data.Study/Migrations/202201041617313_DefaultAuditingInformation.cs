namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultAuditingInformation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AdditionalTablesToSync", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.AdditionalTablesToSync", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.Answer", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.Answer", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.AnswerScore", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.AnswerScore", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.DiaryEntry", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.DiaryEntry", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.CareGiver", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.CareGiver", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.Patient", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.Patient", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.Correction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.Correction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalData", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalData", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalDataAdditional", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalDataAdditional", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.CorrectionDiscussion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.CorrectionDiscussion", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.CorrectionAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.CorrectionAction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.StudyUser", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.StudyUser", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.ReferenceMaterial", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.ReferenceMaterial", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.StudyUserRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.StudyUserRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.Site", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.Site", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.Export", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.Export", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SiteLanguage", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SiteLanguage", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SoftwareRelease", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SoftwareRelease", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SoftwareVersion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SoftwareVersion", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.StudyUserWidget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.StudyUserWidget", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.Widget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.Widget", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.StudyRoleWidget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.StudyRoleWidget", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.WidgetCount", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.WidgetCount", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.WidgetLink", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.WidgetLink", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.WidgetSystemAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.WidgetSystemAction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SystemAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SystemAction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SystemActionStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SystemActionStudyRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.CorrectionHistory", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.CorrectionHistory", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("config.CorrectionStatus", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("config.CorrectionStatus", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.CorrectionWorkflow", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.CorrectionWorkflow", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.Device", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.Device", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.DeviceData", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.DeviceData", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SyncLog", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SyncLog", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.PatientAttribute", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.PatientAttribute", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.PatientVisit", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.PatientVisit", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("config.MissedVisitReason", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("config.MissedVisitReason", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SecurityQuestion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SecurityQuestion", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.DataApproval", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.DataApproval", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.DCFRequest", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.DCFRequest", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.DeviceSession", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.DeviceSession", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("config.EmailContent", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("config.EmailContent", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.EmailContentStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.EmailContentStudyRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.EmailRecipient", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.EmailRecipient", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.EmailSent", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.EmailSent", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.InputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.InputFieldTypeResult", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.QuestionInputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.QuestionInputFieldTypeResult", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.NotificationRequest", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.NotificationRequest", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.ReportStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.ReportStudyRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.ScreenReportDialog", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.ScreenReportDialog", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseCountry", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseCountry", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseDeviceType", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseDeviceType", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.StudyRoleUpdate", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.StudyRoleUpdate", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.SystemSetting", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.SystemSetting", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
            AlterColumn("dbo.VisitComplianceReportQuestionnaireType", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                    },
                }));
            AlterColumn("dbo.VisitComplianceReportQuestionnaireType", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                    },
                }));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VisitComplianceReportQuestionnaireType", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.VisitComplianceReportQuestionnaireType", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SystemSetting", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SystemSetting", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyRoleUpdate", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyRoleUpdate", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseDeviceType", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseDeviceType", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseCountry", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareReleaseCountry", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.ScreenReportDialog", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.ScreenReportDialog", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.ReportStudyRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.ReportStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.NotificationRequest", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.NotificationRequest", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.QuestionInputFieldTypeResult", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.QuestionInputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.InputFieldTypeResult", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.InputFieldTypeResult", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.EmailSent", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.EmailSent", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.EmailRecipient", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.EmailRecipient", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.EmailContentStudyRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.EmailContentStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("config.EmailContent", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("config.EmailContent", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DeviceSession", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DeviceSession", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DCFRequest", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DCFRequest", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DataApproval", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DataApproval", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SecurityQuestion", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SecurityQuestion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("config.MissedVisitReason", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("config.MissedVisitReason", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.PatientVisit", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.PatientVisit", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.PatientAttribute", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.PatientAttribute", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SyncLog", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SyncLog", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DeviceData", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DeviceData", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Device", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Device", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionWorkflow", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionWorkflow", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("config.CorrectionStatus", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("config.CorrectionStatus", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionHistory", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionHistory", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SystemActionStudyRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SystemActionStudyRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SystemAction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SystemAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.WidgetSystemAction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.WidgetSystemAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.WidgetLink", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.WidgetLink", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.WidgetCount", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.WidgetCount", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyRoleWidget", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyRoleWidget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Widget", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Widget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyUserWidget", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyUserWidget", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareVersion", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareVersion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareRelease", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SoftwareRelease", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SiteLanguage", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.SiteLanguage", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Export", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Export", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Site", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Site", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyUserRole", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyUserRole", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.ReferenceMaterial", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.ReferenceMaterial", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyUser", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.StudyUser", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionAction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionAction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionDiscussion", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionDiscussion", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalDataAdditional", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalDataAdditional", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalData", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CorrectionApprovalData", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Correction", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Correction", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Patient", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Patient", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CareGiver", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.CareGiver", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DiaryEntry", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.DiaryEntry", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.AnswerScore", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.AnswerScore", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Answer", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.Answer", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
            AlterColumn("dbo.AdditionalTablesToSync", "LastModifiedByDatabaseUser", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "SUSER_NAME()", newValue: null)
                    },
                }));
            AlterColumn("dbo.AdditionalTablesToSync", "LastModified", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "GETUTCDATE()", newValue: null)
                    },
                }));
        }
    }
}

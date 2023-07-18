namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdditionalTablesToSync",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TableName = c.String(),
                        DoSync = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AnswerScoreId = c.Guid(),
                        DiaryEntryId = c.Guid(nullable: false),
                        QuestionId = c.Guid(nullable: false),
                        ChoiceId = c.Guid(),
                        FreeTextAnswer = c.String(),
                        AnswerOrder = c.Int(),
                        CongifurationId = c.Guid(),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnswerScore", t => t.AnswerScoreId)
                .ForeignKey("dbo.DiaryEntry", t => t.DiaryEntryId)
                .Index(t => t.AnswerScoreId)
                .Index(t => t.DiaryEntryId);
            
            CreateTable(
                "dbo.AnswerScore",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EAP = c.Single(nullable: false),
                        SEM = c.Single(nullable: false),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DiaryEntry",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RowId = c.Int(nullable: false),
                        DiaryStatusId = c.Int(nullable: false),
                        DataSourceId = c.Int(nullable: false),
                        DeviceId = c.Guid(),
                        DiaryDate = c.DateTime(nullable: false, storeType: "date"),
                        StartedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        TransmittedTime = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "getutcdate()")
                                },
                            }),
                        UserId = c.Guid(),
                        CareGiverId = c.Guid(),
                        VisitId = c.Guid(),
                        Ongoing = c.Boolean(nullable: false),
                        ReviewedByUserid = c.Guid(),
                        ReviewedDate = c.DateTimeOffset(precision: 7),
                        PatientId = c.Guid(nullable: false),
                        CompletedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        QuestionnaireId = c.Guid(nullable: false),
                        CongifurationId = c.Guid(),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CareGiver", t => t.CareGiverId)
                .ForeignKey("dbo.Device", t => t.DeviceId)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .ForeignKey("dbo.StudyUser", t => t.ReviewedByUserid)
                .Index(t => t.DeviceId)
                .Index(t => t.CareGiverId)
                .Index(t => t.ReviewedByUserid)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.CareGiver",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PatientId = c.Guid(nullable: false),
                        CareGiverTypeId = c.Guid(),
                        Initials = c.String(),
                        Pin = c.String(),
                        IsTempPin = c.Boolean(nullable: false),
                        IsHandheldTrainingComplete = c.Boolean(nullable: false),
                        IsTabletTrainingComplete = c.Boolean(nullable: false),
                        LoginAttempts = c.Short(),
                        SecurityQuestionId = c.Guid(),
                        SecurityAnswer = c.String(),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.Patient",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PatientNumber = c.String(nullable: false, maxLength: 255),
                        LanguageId = c.Guid(nullable: false),
                        CompletionDate = c.DateTime(),
                        LastUpdate = c.DateTime(),
                        Notes = c.String(maxLength: 150),
                        RowId = c.Int(nullable: false, identity: true),
                        Pin = c.String(maxLength: 100),
                        IsTempPin = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        SecurityQuestionId = c.Guid(),
                        SecurityAnswer = c.String(maxLength: 100),
                        IsHandheldTrainingComplete = c.Boolean(nullable: false),
                        IsTabletTrainingComplete = c.Boolean(nullable: false),
                        AssetTag = c.String(),
                        SiteId = c.Guid(nullable: false),
                        PatientStatusTypeId = c.Int(nullable: false),
                        EnrolledDate = c.DateTimeOffset(nullable: false, precision: 7),
                        CongifurationId = c.Guid(),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .ForeignKey("dbo.SecurityQuestion", t => t.SecurityQuestionId)
                .Index(t => t.PatientNumber)
                .Index(t => t.SecurityQuestionId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.Correction",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        CompletedDate = c.DateTimeOffset(precision: 7),
                        StartedByUserId = c.Guid(nullable: false),
                        CorrectionStatusId = c.Guid(nullable: false),
                        CorrectionTypeId = c.Guid(nullable: false),
                        CurrentWorkflowOrder = c.Int(),
                        PatientId = c.Guid(),
                        SiteId = c.Guid(),
                        DiaryEntryId = c.Guid(),
                        ReasonForCorrection = c.String(),
                        DataCorrectionNumber = c.Int(nullable: false, identity: true),
                        QuestionnaireId = c.Guid(),
                        NoApprovalNeeded = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "0")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("config.CorrectionStatus", t => t.CorrectionStatusId)
                .ForeignKey("dbo.DiaryEntry", t => t.DiaryEntryId)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .ForeignKey("dbo.StudyUser", t => t.StartedByUserId)
                .Index(t => t.StartedByUserId)
                .Index(t => t.CorrectionStatusId)
                .Index(t => t.PatientId)
                .Index(t => t.SiteId)
                .Index(t => t.DiaryEntryId);
            
            CreateTable(
                "dbo.CorrectionApprovalData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CorrectionId = c.Guid(nullable: false),
                        RowId = c.Guid(nullable: false),
                        TableName = c.String(),
                        ColumnName = c.String(),
                        TranslationKey = c.String(),
                        OldDataPoint = c.String(),
                        NewDataPoint = c.String(),
                        OldDisplayValue = c.String(),
                        NewDisplayValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Correction", t => t.CorrectionId)
                .Index(t => t.CorrectionId);
            
            CreateTable(
                "dbo.CorrectionApprovalDataAdditional",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CorrectionApprovalDataId = c.Guid(nullable: false),
                        ColumnName = c.String(),
                        ColumnValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CorrectionApprovalData", t => t.CorrectionApprovalDataId)
                .Index(t => t.CorrectionApprovalDataId);
            
            CreateTable(
                "dbo.CorrectionDiscussion",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Discussion = c.String(),
                        DiscussionDate = c.DateTimeOffset(nullable: false, precision: 7),
                        CorrectionId = c.Guid(nullable: false),
                        StudyUserId = c.Guid(nullable: false),
                        CorrectionActionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Correction", t => t.CorrectionId)
                .ForeignKey("dbo.CorrectionAction", t => t.CorrectionActionId)
                .ForeignKey("dbo.StudyUser", t => t.StudyUserId)
                .Index(t => t.CorrectionId)
                .Index(t => t.StudyUserId)
                .Index(t => t.CorrectionActionId);
            
            CreateTable(
                "dbo.CorrectionAction",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TranslationKey = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        IconCss = c.String(),
                        StatusCss = c.String(),
                        Actionable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudyUser",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(),
                        Email = c.String(),
                        LandingPageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReferenceMaterial",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StudyUserId = c.Guid(nullable: false),
                        ReferenceMaterialTypeId = c.Guid(nullable: false),
                        Name = c.String(),
                        FileName = c.String(),
                        ContentType = c.String(),
                        CreatedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedTime = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudyUser", t => t.StudyUserId)
                .Index(t => t.StudyUserId);
            
            CreateTable(
                "dbo.StudyUserRole",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StudyUserId = c.Guid(nullable: false),
                        StudyRoleId = c.Guid(nullable: false),
                        SiteId = c.Guid(nullable: false),
                        YPRoleId = c.Guid(nullable: false),
                        Notes = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .ForeignKey("dbo.StudyUser", t => t.StudyUserId)
                .Index(t => t.StudyUserId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.Site",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 450),
                        SiteNumber = c.String(nullable: false, maxLength: 450),
                        IsActive = c.Boolean(nullable: false),
                        Address1 = c.String(nullable: false),
                        Address2 = c.String(),
                        Address3 = c.String(),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Zip = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        FaxNumber = c.String(),
                        PrimaryContact = c.String(nullable: false),
                        TimeZone = c.String(nullable: false),
                        LastUpdate = c.DateTime(),
                        Notes = c.String(maxLength: 255),
                        PatientDOBFormatId = c.Int(),
                        Investigator = c.String(maxLength: 50),
                        WebBackupExpireDate = c.DateTime(),
                        CountryId = c.Guid(nullable: false),
                        ConfigurationId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.SiteNumber, unique: true);
            
            CreateTable(
                "dbo.Export",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        UserId = c.Guid(nullable: false),
                        SiteId = c.Guid(),
                        PatientId = c.Guid(),
                        PatientStatusTypeId = c.Int(),
                        QuestionnaireTypeId = c.Int(),
                        ExportStatusId = c.Int(nullable: false),
                        DiaryStartDate = c.DateTimeOffset(precision: 7),
                        DiaryEndDate = c.DateTimeOffset(precision: 7),
                        ScheduledStartTime = c.DateTimeOffset(precision: 7),
                        CreatedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        StartedTime = c.DateTimeOffset(precision: 7),
                        CompletedTime = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExportStatus", t => t.ExportStatusId)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .Index(t => t.SiteId)
                .Index(t => t.PatientId)
                .Index(t => t.ExportStatusId);
            
            CreateTable(
                "dbo.ExportStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteActiveHistory",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SiteId = c.Guid(nullable: false),
                        Previous = c.Boolean(nullable: false),
                        Current = c.Boolean(nullable: false),
                        ChangeDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.SiteLanguage",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SiteId = c.Guid(nullable: false),
                        LanguageId = c.Guid(nullable: false),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.SoftwareRelease",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SoftwareVersionId = c.Guid(nullable: false),
                        DateCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        StudyWide = c.Boolean(nullable: false),
                        Required = c.Boolean(nullable: false),
                        ConfigurationVersion = c.String(),
                        SRDVersion = c.String(),
                        ConfigurationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareVersion", t => t.SoftwareVersionId)
                .Index(t => t.SoftwareVersionId);
            
            CreateTable(
                "dbo.SoftwareVersion",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VersionNumber = c.String(),
                        PackagePath = c.String(),
                        Priority = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "1")
                                },
                            }),
                        PlatformTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudyUserWidget",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StudyUserId = c.Guid(nullable: false),
                        WidgetId = c.Guid(nullable: false),
                        WidgetPosition = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudyUser", t => t.StudyUserId)
                .ForeignKey("dbo.Widget", t => t.WidgetId)
                .Index(t => t.StudyUserId)
                .Index(t => t.WidgetId);
            
            CreateTable(
                "dbo.Widget",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        TranslationTitleText = c.String(),
                        TranslationDescriptionText = c.String(),
                        TranslationButtonText = c.String(),
                        ControllerName = c.String(),
                        ControllerActionName = c.String(),
                        WidgetTypeId = c.Guid(nullable: false),
                        WidgetPosition = c.Int(nullable: false),
                        IconName = c.String(),
                        ColumnWidth = c.Int(nullable: false),
                        ColumnHeight = c.Int(nullable: false),
                        ReportId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudyRoleWidget",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StudyRoleId = c.Guid(nullable: false),
                        WidgetId = c.Guid(nullable: false),
                        WidgetPosition = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Widget", t => t.WidgetId)
                .Index(t => t.WidgetId);
            
            CreateTable(
                "dbo.WidgetCount",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WidgetId = c.Guid(nullable: false),
                        FunctionName = c.String(),
                        TableName = c.String(),
                        TranslationText = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Widget", t => t.WidgetId)
                .Index(t => t.WidgetId);
            
            CreateTable(
                "dbo.WidgetLink",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WidgetId = c.Guid(nullable: false),
                        ControllerName = c.String(),
                        ControllerActionName = c.String(),
                        TranslationText = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Widget", t => t.WidgetId)
                .Index(t => t.WidgetId);
            
            CreateTable(
                "dbo.WidgetSystemAction",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WidgetId = c.Guid(nullable: false),
                        SystemActionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemAction", t => t.SystemActionId)
                .ForeignKey("dbo.Widget", t => t.WidgetId)
                .Index(t => t.WidgetId)
                .Index(t => t.SystemActionId);
            
            CreateTable(
                "dbo.SystemAction",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        IsBlinded = c.Boolean(nullable: false),
                        ActionLocation = c.String(),
                        DeviceAction = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemActionStudyRole",
                c => new
                    {
                        SystemActionId = c.Guid(nullable: false),
                        StudyRoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SystemActionId, t.StudyRoleId })
                .ForeignKey("dbo.SystemAction", t => t.SystemActionId)
                .Index(t => t.SystemActionId);
            
            CreateTable(
                "dbo.CorrectionHistory",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CorrectionId = c.Guid(nullable: false),
                        Discussion = c.String(),
                        DateCreated = c.DateTimeOffset(nullable: false, precision: 7),
                        StudyUserId = c.Guid(),
                        CorrectionActionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Correction", t => t.CorrectionId)
                .ForeignKey("dbo.CorrectionAction", t => t.CorrectionActionId)
                .ForeignKey("dbo.StudyUser", t => t.StudyUserId)
                .Index(t => t.CorrectionId)
                .Index(t => t.StudyUserId)
                .Index(t => t.CorrectionActionId);
            
            CreateTable(
                "config.CorrectionStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TranslationKey = c.String(),
                        Resolved = c.Boolean(nullable: false),
                        NeedsMoreInformation = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CorrectionWorkflow",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ApproverGroupId = c.Guid(),
                        CorrectionId = c.Guid(nullable: false),
                        WorkflowOrder = c.Int(nullable: false),
                        CorrectionActionId = c.Guid(),
                        StudyUserId = c.Guid(),
                        WorkflowChangedDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Correction", t => t.CorrectionId)
                .ForeignKey("dbo.CorrectionAction", t => t.CorrectionActionId)
                .ForeignKey("dbo.StudyUser", t => t.StudyUserId)
                .Index(t => t.CorrectionId)
                .Index(t => t.CorrectionActionId)
                .Index(t => t.StudyUserId);
            
            CreateTable(
                "dbo.Device",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SiteId = c.Guid(),
                        PatientId = c.Guid(),
                        LastReportedSoftwareVersionId = c.Guid(nullable: false),
                        LastReportedConfigurationId = c.Guid(nullable: false),
                        SoftwareReleaseId = c.Guid(nullable: false),
                        DeviceTypeId = c.Guid(),
                        MACAddress = c.String(maxLength: 12),
                        SerialNumber = c.String(maxLength: 16),
                        IMEI1 = c.String(maxLength: 16),
                        IMEI2 = c.String(maxLength: 16),
                        AssetTag = c.String(),
                        SendDatabase = c.Boolean(nullable: false),
                        LastSyncDate = c.DateTime(),
                        DoAdditionalTableSync = c.Boolean(nullable: false),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareVersion", t => t.LastReportedSoftwareVersionId)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .ForeignKey("dbo.SoftwareRelease", t => t.SoftwareReleaseId)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.SiteId)
                .Index(t => t.PatientId)
                .Index(t => t.LastReportedSoftwareVersionId)
                .Index(t => t.SoftwareReleaseId);
            
            CreateTable(
                "dbo.SyncLog",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DeviceId = c.Guid(nullable: false),
                        SoftwareVersionId = c.Guid(),
                        SyncAction = c.String(nullable: false),
                        SyncDate = c.DateTimeOffset(nullable: false, precision: 7),
                        SyncData = c.String(),
                        SyncSuccess = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareVersion", t => t.SoftwareVersionId)
                .ForeignKey("dbo.Device", t => t.DeviceId)
                .Index(t => t.DeviceId)
                .Index(t => t.SoftwareVersionId);
            
            CreateTable(
                "dbo.PatientAttribute",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PatientId = c.Guid(nullable: false),
                        PatientAttributeConfigurationDetailId = c.Guid(nullable: false),
                        AttributeValue = c.String(nullable: false),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.PatientVisit",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VisitReasonId = c.Int(),
                        ProjectedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        SystemDate = c.DateTimeOffset(precision: 7),
                        OutsideVisitWindow = c.Boolean(nullable: false),
                        UnscheduledVisitOrder = c.Int(),
                        Notes = c.String(maxLength: 150),
                        IRTPatientVisitStatusTypeId = c.Int(nullable: false),
                        MissedVisitReasonId = c.Guid(),
                        PatientId = c.Guid(nullable: false),
                        VisitId = c.Guid(nullable: false),
                        PatientVisitStatusTypeId = c.Int(nullable: false),
                        VisitDate = c.DateTimeOffset(precision: 7),
                        ActivationDate = c.DateTimeOffset(precision: 7),
                        CongifurationId = c.Guid(),
                        SyncVersion = c.Int(nullable: false),
                        IsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("config.MissedVisitReason", t => t.MissedVisitReasonId)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.MissedVisitReasonId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "config.MissedVisitReason",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TranslationKey = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SecurityQuestion",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TranslationKey = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DataApproval",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DCFRequest",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(nullable: false, maxLength: 10),
                        TypeOfDataChange = c.String(),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        Notes = c.String(),
                        LastUpdate = c.DateTime(),
                        PatientNumber = c.String(maxLength: 255),
                        TicketNumber = c.String(maxLength: 50),
                        PatientId = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Patient", t => t.PatientId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.DeviceSession",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DeviceId = c.Guid(nullable: false),
                        Token = c.String(),
                        AvailableCount = c.Int(nullable: false),
                        Expired = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "config.EmailContent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PatientStatusTypeId = c.Int(),
                        Name = c.String(),
                        TranslationKey = c.String(),
                        IsBlinded = c.Boolean(nullable: false),
                        IsSiteSpecific = c.Boolean(nullable: false),
                        Notes = c.String(),
                        LastUpdate = c.DateTime(),
                        BodyTemplate = c.String(),
                        SubjectLineTemplate = c.String(),
                        IsEmailSentToPerformingUser = c.Boolean(nullable: false),
                        DisplayOnScreen = c.Boolean(nullable: false),
                        EmailContentTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailContentStudyRole",
                c => new
                    {
                        EmailContentId = c.Guid(nullable: false),
                        StudyRoleId = c.Guid(nullable: false),
                        Notes = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => new { t.EmailContentId, t.StudyRoleId })
                .ForeignKey("config.EmailContent", t => t.EmailContentId)
                .Index(t => t.EmailContentId);
            
            CreateTable(
                "dbo.EmailRecipient",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false, maxLength: 200),
                        EmailSentId = c.Guid(nullable: false),
                        EmailRecipientTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailSent", t => t.EmailSentId)
                .Index(t => t.EmailSentId);
            
            CreateTable(
                "dbo.EmailSent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Body = c.String(),
                        Subject = c.String(),
                        DateSent = c.DateTimeOffset(nullable: false, precision: 7),
                        EmailContentId = c.Guid(nullable: false),
                        StudyUserId = c.Guid(),
                        SiteId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("config.EmailContent", t => t.EmailContentId)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .ForeignKey("dbo.StudyUser", t => t.StudyUserId)
                .Index(t => t.EmailContentId)
                .Index(t => t.StudyUserId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.InputFieldTypeResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientAttributeConfigurationDetailId = c.Guid(),
                        InputFieldTypeId = c.Int(nullable: false),
                        ResultCode = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 300),
                        IsPatientAttribute = c.Boolean(),
                        UnitOfMeasure = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionInputFieldTypeResult",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        QuestionId = c.Guid(nullable: false),
                        ChoiceId = c.Guid(),
                        InputFieldTypeResultId = c.Int(nullable: false),
                        DisplayInApp = c.Boolean(nullable: false),
                        SaveInPortal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InputFieldTypeResult", t => t.InputFieldTypeResultId)
                .Index(t => t.InputFieldTypeResultId);
            
            CreateTable(
                "dbo.ReportStudyRole",
                c => new
                    {
                        ReportId = c.Guid(nullable: false),
                        StudyRoleId = c.Guid(nullable: false),
                        ReportName = c.String(),
                    })
                .PrimaryKey(t => new { t.ReportId, t.StudyRoleId });
            
            CreateTable(
                "dbo.ScreenReportDialog",
                c => new
                    {
                        TranslationKey = c.String(nullable: false, maxLength: 128),
                        ButtonConfirmTranslationKey = c.String(),
                        ButtonCancelTranslationKey = c.String(),
                        TitleTranslationKey = c.String(),
                    })
                .PrimaryKey(t => t.TranslationKey);
            
            CreateTable(
                "dbo.SoftwareReleaseCountry",
                c => new
                    {
                        SoftwareRelease_Id = c.Guid(nullable: false),
                        Country_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SoftwareRelease_Id)
                .ForeignKey("dbo.SoftwareRelease", t => t.SoftwareRelease_Id)
                .Index(t => t.SoftwareRelease_Id);
            
            CreateTable(
                "dbo.SoftwareReleaseDeviceType",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SoftwareReleaseId = c.Guid(nullable: false),
                        DeviceTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareRelease", t => t.SoftwareReleaseId)
                .Index(t => t.SoftwareReleaseId);
            
            CreateTable(
                "dbo.VisitComplianceReportQuestionnaireType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VisitId = c.Int(nullable: false),
                        QuestionnaireTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.VisitId, t.QuestionnaireTypeId }, unique: true, name: "UIX_VisitComplianceReportQuestionnaireType");
            
            CreateTable(
                "dbo.SoftwareReleaseSite",
                c => new
                    {
                        SoftwareRelease_Id = c.Guid(nullable: false),
                        Site_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SoftwareRelease_Id, t.Site_Id })
                .ForeignKey("dbo.SoftwareRelease", t => t.SoftwareRelease_Id, cascadeDelete: true)
                .ForeignKey("dbo.Site", t => t.Site_Id, cascadeDelete: true)
                .Index(t => t.SoftwareRelease_Id)
                .Index(t => t.Site_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SoftwareReleaseDeviceType", "SoftwareReleaseId", "dbo.SoftwareRelease");
            DropForeignKey("dbo.SoftwareReleaseCountry", "SoftwareRelease_Id", "dbo.SoftwareRelease");
            DropForeignKey("dbo.QuestionInputFieldTypeResult", "InputFieldTypeResultId", "dbo.InputFieldTypeResult");
            DropForeignKey("dbo.EmailSent", "StudyUserId", "dbo.StudyUser");
            DropForeignKey("dbo.EmailSent", "SiteId", "dbo.Site");
            DropForeignKey("dbo.EmailRecipient", "EmailSentId", "dbo.EmailSent");
            DropForeignKey("dbo.EmailSent", "EmailContentId", "config.EmailContent");
            DropForeignKey("dbo.EmailContentStudyRole", "EmailContentId", "config.EmailContent");
            DropForeignKey("dbo.DCFRequest", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.DiaryEntry", "ReviewedByUserid", "dbo.StudyUser");
            DropForeignKey("dbo.Patient", "SecurityQuestionId", "dbo.SecurityQuestion");
            DropForeignKey("dbo.PatientVisit", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.PatientVisit", "MissedVisitReasonId", "config.MissedVisitReason");
            DropForeignKey("dbo.PatientAttribute", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.DiaryEntry", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.Device", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.SyncLog", "DeviceId", "dbo.Device");
            DropForeignKey("dbo.SyncLog", "SoftwareVersionId", "dbo.SoftwareVersion");
            DropForeignKey("dbo.Device", "SoftwareReleaseId", "dbo.SoftwareRelease");
            DropForeignKey("dbo.Device", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Device", "LastReportedSoftwareVersionId", "dbo.SoftwareVersion");
            DropForeignKey("dbo.DiaryEntry", "DeviceId", "dbo.Device");
            DropForeignKey("dbo.Correction", "StartedByUserId", "dbo.StudyUser");
            DropForeignKey("dbo.Correction", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Correction", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.Correction", "DiaryEntryId", "dbo.DiaryEntry");
            DropForeignKey("dbo.CorrectionWorkflow", "StudyUserId", "dbo.StudyUser");
            DropForeignKey("dbo.CorrectionWorkflow", "CorrectionActionId", "dbo.CorrectionAction");
            DropForeignKey("dbo.CorrectionWorkflow", "CorrectionId", "dbo.Correction");
            DropForeignKey("dbo.Correction", "CorrectionStatusId", "config.CorrectionStatus");
            DropForeignKey("dbo.CorrectionHistory", "StudyUserId", "dbo.StudyUser");
            DropForeignKey("dbo.CorrectionHistory", "CorrectionActionId", "dbo.CorrectionAction");
            DropForeignKey("dbo.CorrectionHistory", "CorrectionId", "dbo.Correction");
            DropForeignKey("dbo.CorrectionDiscussion", "StudyUserId", "dbo.StudyUser");
            DropForeignKey("dbo.WidgetSystemAction", "WidgetId", "dbo.Widget");
            DropForeignKey("dbo.WidgetSystemAction", "SystemActionId", "dbo.SystemAction");
            DropForeignKey("dbo.SystemActionStudyRole", "SystemActionId", "dbo.SystemAction");
            DropForeignKey("dbo.WidgetLink", "WidgetId", "dbo.Widget");
            DropForeignKey("dbo.WidgetCount", "WidgetId", "dbo.Widget");
            DropForeignKey("dbo.StudyUserWidget", "WidgetId", "dbo.Widget");
            DropForeignKey("dbo.StudyRoleWidget", "WidgetId", "dbo.Widget");
            DropForeignKey("dbo.StudyUserWidget", "StudyUserId", "dbo.StudyUser");
            DropForeignKey("dbo.StudyUserRole", "StudyUserId", "dbo.StudyUser");
            DropForeignKey("dbo.StudyUserRole", "SiteId", "dbo.Site");
            DropForeignKey("dbo.SoftwareRelease", "SoftwareVersionId", "dbo.SoftwareVersion");
            DropForeignKey("dbo.SoftwareReleaseSite", "Site_Id", "dbo.Site");
            DropForeignKey("dbo.SoftwareReleaseSite", "SoftwareRelease_Id", "dbo.SoftwareRelease");
            DropForeignKey("dbo.SiteLanguage", "SiteId", "dbo.Site");
            DropForeignKey("dbo.SiteActiveHistory", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Patient", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Export", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Export", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.Export", "ExportStatusId", "dbo.ExportStatus");
            DropForeignKey("dbo.ReferenceMaterial", "StudyUserId", "dbo.StudyUser");
            DropForeignKey("dbo.CorrectionDiscussion", "CorrectionActionId", "dbo.CorrectionAction");
            DropForeignKey("dbo.CorrectionDiscussion", "CorrectionId", "dbo.Correction");
            DropForeignKey("dbo.CorrectionApprovalDataAdditional", "CorrectionApprovalDataId", "dbo.CorrectionApprovalData");
            DropForeignKey("dbo.CorrectionApprovalData", "CorrectionId", "dbo.Correction");
            DropForeignKey("dbo.CareGiver", "PatientId", "dbo.Patient");
            DropForeignKey("dbo.DiaryEntry", "CareGiverId", "dbo.CareGiver");
            DropForeignKey("dbo.Answer", "DiaryEntryId", "dbo.DiaryEntry");
            DropForeignKey("dbo.Answer", "AnswerScoreId", "dbo.AnswerScore");
            DropIndex("dbo.SoftwareReleaseSite", new[] { "Site_Id" });
            DropIndex("dbo.SoftwareReleaseSite", new[] { "SoftwareRelease_Id" });
            DropIndex("dbo.VisitComplianceReportQuestionnaireType", "UIX_VisitComplianceReportQuestionnaireType");
            DropIndex("dbo.SoftwareReleaseDeviceType", new[] { "SoftwareReleaseId" });
            DropIndex("dbo.SoftwareReleaseCountry", new[] { "SoftwareRelease_Id" });
            DropIndex("dbo.QuestionInputFieldTypeResult", new[] { "InputFieldTypeResultId" });
            DropIndex("dbo.EmailSent", new[] { "SiteId" });
            DropIndex("dbo.EmailSent", new[] { "StudyUserId" });
            DropIndex("dbo.EmailSent", new[] { "EmailContentId" });
            DropIndex("dbo.EmailRecipient", new[] { "EmailSentId" });
            DropIndex("dbo.EmailContentStudyRole", new[] { "EmailContentId" });
            DropIndex("dbo.DCFRequest", new[] { "PatientId" });
            DropIndex("dbo.PatientVisit", new[] { "PatientId" });
            DropIndex("dbo.PatientVisit", new[] { "MissedVisitReasonId" });
            DropIndex("dbo.PatientAttribute", new[] { "PatientId" });
            DropIndex("dbo.SyncLog", new[] { "SoftwareVersionId" });
            DropIndex("dbo.SyncLog", new[] { "DeviceId" });
            DropIndex("dbo.Device", new[] { "SoftwareReleaseId" });
            DropIndex("dbo.Device", new[] { "LastReportedSoftwareVersionId" });
            DropIndex("dbo.Device", new[] { "PatientId" });
            DropIndex("dbo.Device", new[] { "SiteId" });
            DropIndex("dbo.CorrectionWorkflow", new[] { "StudyUserId" });
            DropIndex("dbo.CorrectionWorkflow", new[] { "CorrectionActionId" });
            DropIndex("dbo.CorrectionWorkflow", new[] { "CorrectionId" });
            DropIndex("dbo.CorrectionHistory", new[] { "CorrectionActionId" });
            DropIndex("dbo.CorrectionHistory", new[] { "StudyUserId" });
            DropIndex("dbo.CorrectionHistory", new[] { "CorrectionId" });
            DropIndex("dbo.SystemActionStudyRole", new[] { "SystemActionId" });
            DropIndex("dbo.WidgetSystemAction", new[] { "SystemActionId" });
            DropIndex("dbo.WidgetSystemAction", new[] { "WidgetId" });
            DropIndex("dbo.WidgetLink", new[] { "WidgetId" });
            DropIndex("dbo.WidgetCount", new[] { "WidgetId" });
            DropIndex("dbo.StudyRoleWidget", new[] { "WidgetId" });
            DropIndex("dbo.StudyUserWidget", new[] { "WidgetId" });
            DropIndex("dbo.StudyUserWidget", new[] { "StudyUserId" });
            DropIndex("dbo.SoftwareRelease", new[] { "SoftwareVersionId" });
            DropIndex("dbo.SiteLanguage", new[] { "SiteId" });
            DropIndex("dbo.SiteActiveHistory", new[] { "SiteId" });
            DropIndex("dbo.Export", new[] { "ExportStatusId" });
            DropIndex("dbo.Export", new[] { "PatientId" });
            DropIndex("dbo.Export", new[] { "SiteId" });
            DropIndex("dbo.Site", new[] { "SiteNumber" });
            DropIndex("dbo.Site", new[] { "Name" });
            DropIndex("dbo.StudyUserRole", new[] { "SiteId" });
            DropIndex("dbo.StudyUserRole", new[] { "StudyUserId" });
            DropIndex("dbo.ReferenceMaterial", new[] { "StudyUserId" });
            DropIndex("dbo.CorrectionDiscussion", new[] { "CorrectionActionId" });
            DropIndex("dbo.CorrectionDiscussion", new[] { "StudyUserId" });
            DropIndex("dbo.CorrectionDiscussion", new[] { "CorrectionId" });
            DropIndex("dbo.CorrectionApprovalDataAdditional", new[] { "CorrectionApprovalDataId" });
            DropIndex("dbo.CorrectionApprovalData", new[] { "CorrectionId" });
            DropIndex("dbo.Correction", new[] { "DiaryEntryId" });
            DropIndex("dbo.Correction", new[] { "SiteId" });
            DropIndex("dbo.Correction", new[] { "PatientId" });
            DropIndex("dbo.Correction", new[] { "CorrectionStatusId" });
            DropIndex("dbo.Correction", new[] { "StartedByUserId" });
            DropIndex("dbo.Patient", new[] { "SiteId" });
            DropIndex("dbo.Patient", new[] { "SecurityQuestionId" });
            DropIndex("dbo.Patient", new[] { "PatientNumber" });
            DropIndex("dbo.CareGiver", new[] { "PatientId" });
            DropIndex("dbo.DiaryEntry", new[] { "PatientId" });
            DropIndex("dbo.DiaryEntry", new[] { "ReviewedByUserid" });
            DropIndex("dbo.DiaryEntry", new[] { "CareGiverId" });
            DropIndex("dbo.DiaryEntry", new[] { "DeviceId" });
            DropIndex("dbo.Answer", new[] { "DiaryEntryId" });
            DropIndex("dbo.Answer", new[] { "AnswerScoreId" });
            DropTable("dbo.SoftwareReleaseSite");
            DropTable("dbo.VisitComplianceReportQuestionnaireType");
            DropTable("dbo.SoftwareReleaseDeviceType");
            DropTable("dbo.SoftwareReleaseCountry");
            DropTable("dbo.ScreenReportDialog");
            DropTable("dbo.ReportStudyRole");
            DropTable("dbo.QuestionInputFieldTypeResult");
            DropTable("dbo.InputFieldTypeResult");
            DropTable("dbo.EmailSent");
            DropTable("dbo.EmailRecipient");
            DropTable("dbo.EmailContentStudyRole");
            DropTable("config.EmailContent");
            DropTable("dbo.DeviceSession");
            DropTable("dbo.DCFRequest");
            DropTable("dbo.DataApproval");
            DropTable("dbo.SecurityQuestion");
            DropTable("config.MissedVisitReason");
            DropTable("dbo.PatientVisit");
            DropTable("dbo.PatientAttribute");
            DropTable("dbo.SyncLog");
            DropTable("dbo.Device");
            DropTable("dbo.CorrectionWorkflow");
            DropTable("config.CorrectionStatus");
            DropTable("dbo.CorrectionHistory");
            DropTable("dbo.SystemActionStudyRole");
            DropTable("dbo.SystemAction");
            DropTable("dbo.WidgetSystemAction");
            DropTable("dbo.WidgetLink");
            DropTable("dbo.WidgetCount");
            DropTable("dbo.StudyRoleWidget");
            DropTable("dbo.Widget");
            DropTable("dbo.StudyUserWidget");
            DropTable("dbo.SoftwareVersion",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "IsActive",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "1" },
                        }
                    },
                });
            DropTable("dbo.SoftwareRelease");
            DropTable("dbo.SiteLanguage");
            DropTable("dbo.SiteActiveHistory");
            DropTable("dbo.ExportStatus");
            DropTable("dbo.Export");
            DropTable("dbo.Site");
            DropTable("dbo.StudyUserRole");
            DropTable("dbo.ReferenceMaterial");
            DropTable("dbo.StudyUser");
            DropTable("dbo.CorrectionAction");
            DropTable("dbo.CorrectionDiscussion");
            DropTable("dbo.CorrectionApprovalDataAdditional");
            DropTable("dbo.CorrectionApprovalData");
            DropTable("dbo.Correction",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "NoApprovalNeeded",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "0" },
                        }
                    },
                });
            DropTable("dbo.Patient");
            DropTable("dbo.CareGiver");
            DropTable("dbo.DiaryEntry",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "TransmittedTime",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "getutcdate()" },
                        }
                    },
                });
            DropTable("dbo.AnswerScore");
            DropTable("dbo.Answer");
            DropTable("dbo.AdditionalTablesToSync");
        }
    }
}

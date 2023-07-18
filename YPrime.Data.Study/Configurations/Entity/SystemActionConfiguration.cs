using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Constants;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class SystemActionConfiguration
    {
        public SystemActionConfiguration(EntityTypeConfiguration<SystemAction> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'System Action Seeding'");

            context.SystemActions.AddOrUpdate(dt => dt.Id,
                new SystemAction
                {
                  Id = SystemActionTypes.CanViewAnalytics,
                  Name = nameof(SystemActionTypes.CanViewAnalytics),
                  ActionLocation = "AnalyticsController:Index",
                  Description = SystemActionTypeDescriptions.CanViewAnalyticsDescription,
                  IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanUseDeviceWidget,
                    Name = nameof(SystemActionTypes.CanUseDeviceWidget),
                    ActionLocation = "DevicesController:Widget",
                    Description = "Can use the device widget.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanCreateExport,
                    Name = nameof(SystemActionTypes.CanCreateExport),
                    ActionLocation = "ExportController:Create",
                    Description = "Can create export.",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanCreateDCF,
                    Name = nameof(SystemActionTypes.CanCreateDCF),
                    ActionLocation = "DCFRequestController:Create",
                    Description = "Can create a new DCF.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanEditPatient,
                    Name = nameof(SystemActionTypes.CanEditPatient),
                    ActionLocation = "PatientController:Edit",
                    Description = "Can edit subject",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanAddEditSites,
                    Name = nameof(SystemActionTypes.CanAddEditSites),
                    ActionLocation = "SiteController:AddEdit",
                    Description = "Ability to add/edit sites",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanCreateBringYourOwnDeviceCode,
                    Name = nameof(SystemActionTypes.CanCreateBringYourOwnDeviceCode),
                    ActionLocation = "PatientController:CreateBYODCode",
                    Description = "Can create a subject bring your own device code.",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewDCFList,
                    Name = nameof(SystemActionTypes.CanViewDCFList),
                    ActionLocation = "DCFRequestController:Index",
                    Description = "Can view the list of DCFs.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewReports,
                    Name = nameof(SystemActionTypes.CanViewReports),
                    ActionLocation = "ReportController:Index",
                    Description = SystemActionTypeDescriptions.CanViewReportsDescription,
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanRemoveVisitFromUser,
                    Name = nameof(SystemActionTypes.CanRemoveVisitFromUser),
                    ActionLocation = "PatientVisitController:RemoveVisitFromUser",
                    Description = "Can remove reviewer visit association(eCOA).",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewRolesList,
                    Name = nameof(SystemActionTypes.CanViewRolesList),
                    ActionLocation = "RoleController:Index",
                    Description = "View System Roles",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanResetPatientPin,
                    Name = nameof(SystemActionTypes.CanResetPatientPin),
                    ActionLocation = "PatientController:ResetPIN",
                    Description = "Can reset the pin for a subject.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanResetCareGiverPin,
                    Name = nameof(SystemActionTypes.CanResetCareGiverPin),
                    ActionLocation = "CareGiverController:ResetPIN",
                    Description = "Can reset the pin for a caregiver.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanCreateReferenceMaterials,
                    Name = nameof(SystemActionTypes.CanCreateReferenceMaterials),
                    ActionLocation = "ReferenceMaterialController:Create",
                    Description = "Create Reference materials",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewPatientDetails,
                    Name = nameof(SystemActionTypes.CanViewPatientDetails),
                    ActionLocation = "PatientController:DisplayPatient",
                    Description = "Can view subject details.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewCareGiverDetails,
                    Name = nameof(SystemActionTypes.CanViewCareGiverDetails),
                    ActionLocation = "CareGiverController:DisplayCareGivers",
                    Description = "Can view caregiver details.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanCreatePatient,
                    Name = nameof(SystemActionTypes.CanCreatePatient),
                    ActionLocation = "PatientController:Create",
                    Description = "Can create subject",
                    IsBlinded = true,
                    DeviceAction = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewDeviceList,
                    Name = nameof(SystemActionTypes.CanViewDeviceList),
                    ActionLocation = "DevicesController:Index",
                    Description = "Can view the list of devices in the system.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanUploadReferenceMaterials,
                    Name = nameof(SystemActionTypes.CanUploadReferenceMaterials),
                    ActionLocation = "ReferenceMaterialController:Upload",
                    Description = "Upload reference materials.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewReferenceMatieralList,
                    Name = nameof(SystemActionTypes.CanViewReferenceMatieralList),
                    ActionLocation = "ReferenceMaterialController:Index",
                    Description = "View subject materials",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewDiaryEntryDetails,
                    Name = nameof(SystemActionTypes.CanViewDiaryEntryDetails),
                    ActionLocation = "DiaryEntriesController:Details",
                    Description = "Can View Diary entry details..",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewSiteManagement,
                    Name = nameof(SystemActionTypes.CanViewSiteManagement),
                    ActionLocation = "SiteController:Index",
                    Description = "Ability to View site management page.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanAccessReviewerLandingPage,
                    Name = nameof(SystemActionTypes.CanAccessReviewerLandingPage),
                    ActionLocation = "PatientVisitController:ReviewerLandingPage",
                    Description = "Ability access the reviewer landing page.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanSeePatientList,
                    Name = nameof(SystemActionTypes.CanSeePatientList),
                    ActionLocation = "PatientController:Index",
                    Description = "Ability to see the subject list on the main screen",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewExport,
                    Name = nameof(SystemActionTypes.CanViewExport),
                    ActionLocation = "ExportController:Index",
                    Description = "Can View export.",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanRunExport,
                    Name = nameof(SystemActionTypes.CanRunExport),
                    ActionLocation = "ExportController:RunExport",
                    Description = "Can run export",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanDownloadExportResults,
                    Name = nameof(SystemActionTypes.CanDownloadExportResults),
                    ActionLocation = "ExportController:DownloadExport",
                    Description = "Can download export results",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanAddVisitToUser,
                    Name = nameof(SystemActionTypes.CanAddVisitToUser),
                    ActionLocation = "PatientVisitController:AddVisitToUser",
                    Description = "Can add visit reviewer association(eCOA).",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanEditPatient2,
                    Name = nameof(SystemActionTypes.CanEditPatient2),
                    ActionLocation = "PatientController:EditPatientInformation",
                    Description = "Ability to edit the details of a subject.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanDownloadReferenceMaterials,
                    Name = nameof(SystemActionTypes.CanDownloadReferenceMaterials),
                    ActionLocation = "ReferenceMaterialController:Download",
                    Description = "Download reference materials.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanSeePatientVisitList,
                    Name = nameof(SystemActionTypes.CanSeePatientVisitList),
                    ActionLocation = "PatientVisitController:IndexAccordian",
                    Description = "Ability to see the subject visit list on the main screen",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewDeviceDetails,
                    Name = nameof(SystemActionTypes.CanViewDeviceDetails),
                    ActionLocation = "DevicesController:Details",
                    Description = "Can view the details page for a device .",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanUnblindPatient,
                    Name = nameof(SystemActionTypes.CanUnblindPatient),
                    ActionLocation = "PatientController:Unblind",
                    Description = "Can unblind subject",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanReviewPatientVisit,
                    Name = nameof(SystemActionTypes.CanReviewPatientVisit),
                    ActionLocation = "PatientVisitController:ReviewPatientVisit",
                    Description = "Subject list on the main screen",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewConfirmationList,
                    Name = nameof(SystemActionTypes.CanViewConfirmationList),
                    ActionLocation = "ConfirmationController:ShowSavedConfirmations",
                    Description = "Can view confirmation list.",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewDataCorrections,
                    Name = nameof(SystemActionTypes.CanViewDataCorrections),
                    ActionLocation = "CorrectionController:Index",
                    Description = "Can view Data Corrections",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanCreateDataCorrections,
                    Name = nameof(SystemActionTypes.CanCreateDataCorrections),
                    ActionLocation = "CorrectionController:Create",
                    Description = "Can Create Data Corrections.",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanImportSites,
                    Name = nameof(SystemActionTypes.CanImportSites),
                    ActionLocation = "SiteController:Import",
                    Description = "Can Import sites.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanEditSiteLanguages,
                    Name = nameof(SystemActionTypes.CanEditSiteLanguages),
                    ActionLocation = "SiteController:SiteLanguageTab",
                    Description = "Can edit site languages",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewDashboard,
                    Name = nameof(SystemActionTypes.CanViewDashboard),
                    ActionLocation = "DashboardController:Index",
                    Description = "Ability to View 'At a Glance' dashboard.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewUpcomingVisitsWidget,
                    Name = nameof(SystemActionTypes.CanViewUpcomingVisitsWidget),
                    ActionLocation = "PatientController:UpcomingVisitsWidget",
                    Description = "Can view Upcoming Visits Widget.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanDeleteReferenceMaterials,
                    Name = nameof(SystemActionTypes.CanDeleteReferenceMaterials),
                    ActionLocation = "ReferenceMaterialController:Delete",
                    Description = "Delete Reference Materials",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CreateWebBackupHandheldLink,
                    Name = nameof(SystemActionTypes.CreateWebBackupHandheldLink),
                    ActionLocation = "WebBackupController:CreateWebBackupHandheldLink",
                    Description = "Can Activate Web-Backup (Handheld)",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanLoginToDevice,
                    Name = nameof(SystemActionTypes.CanLoginToDevice),
                    ActionLocation = "DeviceRoles:CanLoginToDevice",
                    Description = "Can log into a mobile device.",
                    DeviceAction = true,
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.SiteBulkUpdate,
                    Name = nameof(SystemActionTypes.SiteBulkUpdate),
                    ActionLocation = "SiteController:SiteBulkUpdate",
                    Description = "Can Site Bulk Update",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewSoftwareVersionManagement,
                    Name = nameof(SystemActionTypes.CanViewSoftwareVersionManagement),
                    ActionLocation = "SoftwareVersionController:Index",
                    Description = "Can view Software Version Management",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanViewSoftwareReleases,
                    Name = nameof(SystemActionTypes.CanViewSoftwareReleases),
                    ActionLocation = "SoftwareReleaseController:Index",
                    Description = "Ability to View Software Release Page",
                    IsBlinded = false
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanActivateTabletWebBackup,
                    Name = nameof(SystemActionTypes.CanActivateTabletWebBackup),
                    ActionLocation = "WebBackupController:WebBackupToggle",
                    Description = "Can activate Web-Backup (Tablet)",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanAccessTabletWebBackup,
                    Name = nameof(SystemActionTypes.CanAccessTabletWebBackup),
                    ActionLocation = "WebBackupController:Index",
                    Description = "Can access Web-Backup button (Tablet)",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanActivateVisitInPortal,
                    Name = nameof(SystemActionTypes.CanActivateVisitInPortal),
                    ActionLocation = "PatientVisitController:ActivateVisitInPortal",
                    Description = "Can activate visits in Portal.",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanCreateCaregiverInPortal,
                    Name = nameof(SystemActionTypes.CanCreateCaregiverInPortal),
                    ActionLocation = "CareGiverController:CanCreateCaregiverInPortal",
                    Description = "Can create caregiver in Portal",
                    IsBlinded = true
                },
                new SystemAction
                {
                    Id = SystemActionTypes.CanScheduleHangfireJobs,
                    Name = nameof(SystemActionTypes.CanScheduleHangfireJobs),
                    ActionLocation = "HangfireJobsController:ScheduleAllJobs",
                    Description = "Can schedule Hangfire jobs",
                    IsBlinded = true
                }
            );

            context.SaveChanges();
        }
    }
}
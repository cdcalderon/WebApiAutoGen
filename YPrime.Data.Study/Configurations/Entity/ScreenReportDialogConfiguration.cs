using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models.Models;
using YPrime.Data.Study.Models.Models.Constants;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class ScreenReportDialogConfiguration
    {
        public ScreenReportDialogConfiguration(EntityTypeConfiguration<ScreenReportDialog> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Screen Report Dialog Seeding'");

            // clear all entries
            context.ScreenReportDialog.RemoveRange(context.ScreenReportDialog);
            context.SaveChanges();

            // add new entries
            context.ScreenReportDialog.AddOrUpdate(a => a.TranslationKey,
                new ScreenReportDialog
                {
                    TranslationKey = "EnterPatientNumber",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "PatientExists",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SubjectEnrollmentConfirmation",
                    ButtonCancelTranslationKey = "Cancel",
                    ButtonConfirmTranslationKey = "Confirm",
                    TitleTranslationKey = "Confirm",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SubjectEnrollmentConfirmationSixDigit",
                    ButtonCancelTranslationKey = "Cancel",
                    ButtonConfirmTranslationKey = "Confirm",
                    TitleTranslationKey = "Confirm",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "DataSyncSuccess|SoftwareUpdateAvailable",
                    ButtonCancelTranslationKey = "Later",
                    ButtonConfirmTranslationKey = "StartDownload",
                    TitleTranslationKey = "Info",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision
                },
                 new ScreenReportDialog
                 {
                     TranslationKey = "DataSyncSuccess|SoftwareUpToDate",
                     ButtonCancelTranslationKey = "",
                     ButtonConfirmTranslationKey = "Ok",
                     TitleTranslationKey = "Info",
                     DeviceType = ScreenReportDeviceTypes.Both,
                     DialogType = ScreenReportDialogTypes.Both
                 },
                new ScreenReportDialog
                {
                    TranslationKey = "DataSyncFailure",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Info",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SelectStatus",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "NoStatusSelected",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "ConfirmStatusMessage",
                    ButtonCancelTranslationKey = "No",
                    ButtonConfirmTranslationKey = "Yes",
                    TitleTranslationKey = "ConfirmStatusHeader",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "ContinueCompletingVisit",
                    ButtonCancelTranslationKey = "No",
                    ButtonConfirmTranslationKey = "Yes",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Tablet,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "ContinueCompletingVisitSoftStop",
                    ButtonCancelTranslationKey = "No",
                    ButtonConfirmTranslationKey = "Yes",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Tablet,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SubjectFormsActive",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Info",
                    DeviceType = ScreenReportDeviceTypes.Tablet,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "DownloadFailed",
                    ButtonCancelTranslationKey = "Cancel",
                    ButtonConfirmTranslationKey = "Retry",
                    TitleTranslationKey = "DownloadFailure",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "DataSyncNoInternet",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Info",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "BatteryLevelCriticalError",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Error",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "PinUpdatedSuccessfully",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Success",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "ExitWithoutSavingQuestion",
                    ButtonCancelTranslationKey = "Cancel",
                    ButtonConfirmTranslationKey = "Confirm",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both

                },
                new ScreenReportDialog
                {
                    TranslationKey = "invalidSecurityQuestion",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "invalidPin",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SelectionMarkVisitMissed",
                    ButtonCancelTranslationKey = "Cancel",
                    ButtonConfirmTranslationKey = "Continue",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Tablet,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "DataSyncConfirmationAssign",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Handheld,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "DataSyncConfirmationUnAssign",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Handheld,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "EnrollingSubject",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SyncingData",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "Saving",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SavingQuestionnaire",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "DownloadLatestUpdate",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision
                },
                new ScreenReportDialog
                {
                    TranslationKey = "PleaseEnterPin",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "PleaseSelectCaregiver",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Info",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                },
                new ScreenReportDialog
                {
                    TranslationKey = "PleaseSelectCareGiverType",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Info",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "PleaseSelectSubject",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Invalid",
                    DeviceType = ScreenReportDeviceTypes.Handheld,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "PreviousVisitNotCompleted",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Tablet,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SureUnassignSubjectDevice",
                    ButtonCancelTranslationKey = "No",
                    ButtonConfirmTranslationKey = "Yes",
                    TitleTranslationKey = "ConfirmQuestion",
                    DeviceType = ScreenReportDeviceTypes.Handheld,
                    DialogType = ScreenReportDialogTypes.Provision,
                    IsSiteFacing = true,
                },
                new ScreenReportDialog
                {
                    TranslationKey = "GrantCameraPermissionToApp",
                    ButtonCancelTranslationKey = "Cancel",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Warning",
                    DeviceType = ScreenReportDeviceTypes.Handheld,
                    DialogType = ScreenReportDialogTypes.BYOD
                },
                new ScreenReportDialog
                {
                    TranslationKey = "RegisteringDeviceText",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Handheld,
                    DialogType = ScreenReportDialogTypes.BYOD
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SoftwareUpdateAvailableBYODDevices",
                    ButtonCancelTranslationKey = "Later",
                    ButtonConfirmTranslationKey = "Update",
                    TitleTranslationKey = "",
                    DeviceType = ScreenReportDeviceTypes.Handheld,
                    DialogType = ScreenReportDialogTypes.BYOD
                },
                new ScreenReportDialog
                {
                    TranslationKey = "SoftwareUpdateUnavailable",
                    ButtonCancelTranslationKey = "",
                    ButtonConfirmTranslationKey = "Ok",
                    TitleTranslationKey = "Info",
                    DeviceType = ScreenReportDeviceTypes.Both,
                    DialogType = ScreenReportDialogTypes.Both
                }
            );

            context.SaveChanges();
        }
    }
}
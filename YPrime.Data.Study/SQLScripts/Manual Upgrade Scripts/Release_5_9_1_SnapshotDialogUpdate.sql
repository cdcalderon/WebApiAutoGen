set XACT_ABORT on
begin tran

UPDATE [dbo].[ScreenReportDialog]
SET DialogType='Provision'
WHERE TranslationKey='DataSyncSuccess|SoftwareUpdateAvailable';

UPDATE [dbo].[ScreenReportDialog]
SET DialogType='Both'
WHERE TranslationKey='ExitWithoutSavingQuestion';

UPDATE [dbo].[ScreenReportDialog]
SET DialogType='Both'
WHERE TranslationKey='PleaseSelectCaregiver';

commit
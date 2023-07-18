/****** Script for updating ScreenReportDialog IsSiteFacing flag  ******/

UPDATE ScreenReportDialog
SET IsSiteFacing = 1
WHERE TranslationKey in 
('EnterPatientNumber',
'PatientExists',
'SubjectEnrollmentConfirmation',
'SubjectEnrollmentConfirmationSixDigit',
'SelectStatus',
'ConfirmStatusMessage',
'ContinueCompletingVisit',
'ContinueCompletingVisitSoftStop',
'SubjectFormsActive',
'SelectionMarkVisitMissed',
'DataSyncConfirmationAssign',
'EnrollingSubject',
'PleaseSelectCareGiverType',
'PleaseSelectSubject',
'PreviousVisitNotCompleted',
'SureUnassignSubjectDevice',
'DataSyncConfirmationUnAssign'
)
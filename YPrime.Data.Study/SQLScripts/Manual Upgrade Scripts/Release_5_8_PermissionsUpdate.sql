set XACT_ABORT on
begin tran

-- Default StudyRoles
Declare @SubInvestigatorRole UniqueIdentifier = 'D984A2A4-1BF2-E611-80D8-000D3A1029A9';
Declare @SponsorRole UniqueIdentifier = 'DA84A2A4-1BF2-E611-80D8-000D3A1029A9';
Declare @ClinicalResearchAssociateRole UniqueIdentifier = 'CF84A2A4-1BF2-E611-80D8-000D3A1029A9';
Declare @StudyCoordinatorRole UniqueIdentifier = 'D884A2A4-1BF2-E611-80D8-000D3A1029A9';
Declare @InvestigatorRole UniqueIdentifier = 'D284A2A4-1BF2-E611-80D8-000D3A1029A9';
Declare @YPrimeRole UniqueIdentifier = 'DF84A2A4-1BF2-E611-80D8-000D3A1029A9';

-- New Report/Analytics Ids
Declare @CanViewReports UniqueIdentifier = '8C6B5F66-A32D-4F7B-80FC-2D33EA49FA6B';
Declare @CanViewAnalytics UniqueIdentifier = '71266218-0fbe-4051-99ce-f338359b2c7d';

-- Legacy Report/Analytics Ids
Declare @CanViewReport UniqueIdentifier = '3208182F-A22E-40B6-B9FB-7B40C251A2F4';
Declare @CanViewReportList UniqueIdentifier = '8C6B5F66-A32D-4F7B-80FC-2D33EA49FA6B';
Declare @CanViewAnalyticsReportList UniqueIdentifier = '7AEC4147-4E1E-44FA-8F73-0DC6220D7854'

-- Drop Unused Legacy Permissions
delete from SystemActionStudyRole 
where SystemActionId in 
(
	@CanViewReport, 
	@CanViewAnalyticsReportList
)

-- Drop Unused Legacy System Actions
delete from SystemAction
where Id in
(
	@CanViewReport,
	@CanViewAnalyticsReportList
)

-- Repoint CanViewReportList permission
update SystemAction 
set 
	Name = 'CanViewReports',
	Description = 'Can View Reports'
where Id = @CanViewReportList

-- OPTIONAL. ONLY REQUIRED IF YOU WANT TO USE THE NEW ANALYTICS FUNCTIONS. 
-- Analytics Key setup. Enable/Disable roles as applicable
Insert into SystemAction values (@CanViewAnalytics, 'CanViewAnalytics', 'Can View Analytics', 1, 'AnalyticsController:Index', '0', NULL, '1900-01-01 00:00:00.0000000', NULL);
Insert into SystemActionStudyRole values (@CanViewAnalytics, @YPrimeRole, NULL, '1900-01-01 00:00:00.0000000',NULL);
Insert into SystemActionStudyRole values (@CanViewAnalytics, @InvestigatorRole, NULL, '1900-01-01 00:00:00.0000000',NULL);
Insert into SystemActionStudyRole values (@CanViewAnalytics, @SubInvestigatorRole, NULL, '1900-01-01 00:00:00.0000000',NULL);
Insert into SystemActionStudyRole values (@CanViewAnalytics, @StudyCoordinatorRole, NULL, '1900-01-01 00:00:00.0000000',NULL);
Insert into SystemActionStudyRole values (@CanViewAnalytics, @ClinicalResearchAssociateRole, NULL, '1900-01-01 00:00:00.0000000',NULL);
Insert into SystemActionStudyRole values (@CanViewAnalytics, @SponsorRole, NULL, '1900-01-01 00:00:00.0000000',NULL);

-- Should return 2 system actions(1 if analytics steps were skipped)
select * from SystemAction 
where Id in 
(
	@CanViewAnalytics,				-- Added
	@CanViewAnalyticsReportList,	-- Removed
	@CanViewReport,					-- Removed
	@CanViewReportList				-- Updated to @CanViewReports
)

-- Set of permissions across reports/analytics
select * from SystemActionStudyRole
where SystemActionId in 
(
	@CanViewReports,
	@CanViewAnalytics
)

commit
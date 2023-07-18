if exists(select * from information_schema.routines where specific_name = 'PatientVisitAuditRecReport_Filtered')
DROP PROCEDURE [dbo].PatientVisitAuditRecReport_Filtered
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[PatientVisitAuditRecReport_Filtered] 
	@SiteNumberID varchar(max) = '',
	@SubjectNumber varchar(50) = '',
	@Protocol varchar(max) = '',
	@visitJson varchar(max) = '',
	@patientVisitStatusJson varchar(max) = ''

AS
BEGIN


SET NOCOUNT ON

DECLARE @StudyName VARCHAR(255) = @Protocol;
DECLARE @dtFormat VARCHAR(25) = 'dd-MMM-yyyy hh:mm tt zzz';
DECLARE @patientID UNIQUEIDENTIFIER
DECLARE @DefaultCulture VARCHAR(12) = 'en-US'

SELECT @patientID = id FROM dbo.Patient WHERE PatientNumber=@SubjectNumber

DECLARE @visit TABLE 
(
	Id uniqueidentifier,
	Name nvarchar(60),
	DaysExpected int,
	WindowBefore int,
	WindowAfter int,
	Notes nvarchar(150),
	LastUpdate datetime,
	WindowOverride bit,
	OverrideReason nvarchar(255),
	IsScheduled bit,
	ReasonFlag bit, 
	VisitOrder int, 
	VisitAnchor UNIQUEIDENTIFIER,
	VisitStop_HSN nchar(1),
	InterviewModeAvailable bit, 
	CloseoutFormAvailable bit, 
	AlwaysAvailable bit, 
	CircularVisit bit,
	DefaultCycleDayId uniqueidentifier,
	SuccessPatientStatusTypeId int,
	FailurePatientStatusTypeId int,
	VisitAvailableBusinessRuleId uniqueidentifier,
	MaxLength nvarchar(60),
	VisitAvailableBusinessRuleTrueFaleIndicator bit,
	DosageModuleEnabled bit
);

DECLARE @patientVisitStatusType Table
(
	Id int, 
	Name nvarchar(max)
);

INSERT INTO @visit
SELECT * 
FROM
OPENJSON(@visitJson)
WITH
(
	Id uniqueidentifier,
	Name nvarchar(60),
	DaysExpected int,
	WindowBefore int,
	WindowAfter int,
	Notes nvarchar(150),
	LastUpdate datetime,
	WindowOverride bit,
	OverrideReason nvarchar(255),
	IsScheduled bit,
	ReasonFlag bit, 
	VisitOrder int, 
	VisitAnchor UNIQUEIDENTIFIER,
	VisitStop_HSN nchar(1),
	InterviewModeAvailable bit, 
	CloseoutFormAvailable bit, 
	AlwaysAvailable bit, 
	CircularVisit bit,
	DefaultCycleDayId uniqueidentifier,
	SuccessPatientStatusTypeId int,
	FailurePatientStatusTypeId int,
	VisitAvailableBusinessRuleId uniqueidentifier,
	MaxLength nvarchar(60),
	VisitAvailableBusinessRuleTrueFaleIndicator bit,
	DosageModuleEnabled bit
);

INSERT INTO @patientVisitStatusType
SELECT * 
FROM
OPENJSON(@patientVisitStatusJson)
WITH
(
	Id int, 
	Name nvarchar(max)
);


create table #AuditRows(
	id int	identity(1,1),
	ModifiedBy varchar(200),
	ModifiedDate datetimeoffset,
	VisitDate datetimeoffset,
	VisitOrder int,
	PatientVisitStatusTypeId int,
    VisitId uniqueidentifier,
	MissedVisitReasonId uniqueidentifier,
	AuditCorrectionId uniqueidentifier,
	AuditDeviceID uniqueidentifier,
	AuditSource char(1),
	SiteNumber varchar(255),
	VisitActivationDate datetimeoffset,
	SortOrder int
);

insert into #AuditRows
	(ModifiedBy,ModifiedDate,VisitDate,VisitOrder,PatientVisitStatusTypeId,VisitId,MissedVisitReasonId,
	 AuditCorrectionId,AuditDeviceID,AuditSource,SiteNumber,VisitActivationDate,SortOrder)
	select 
		case 
			when su.UserName is not null then su.UserName
			else ModifiedBy
		end ModifiedBy,
		ModifiedDate,
		VisitDate,
		v.VisitOrder,
		PatientVisitStatusTypeId,
		VisitId,
		MissedVisitReasonId,
		AuditCorrectionId,
		AuditDeviceId,
		AuditSource,
		s.SiteNumber,
		ActivationDate,
		1
	from ypaudit.patientvisit yp
	join dbo.Patient p on p.Id=yp.PatientId
	join dbo.Site s on s.Id=p.SiteId

	left join @visit v on v.Id=yp.VisitId

	left outer join dbo.StudyUser su on 
		TRY_CAST(yp.ModifiedBy as uniqueidentifier) IS NOT NULL AND 
		TRY_CAST(yp.ModifiedBy as uniqueidentifier) = [su].[Id]
	where yp.patientId=@patientID 
			and ( 
				(yp.AuditSource='D' or yp.AuditSource ='W') 
				or (yp.AuditAction='U' and yp.AuditCorrectionID is not null )
				OR (yp.AuditSource='A' and su.UserName is not null)
				)
	order by VisitOrder,ModifiedDate;

select	
		@StudyName								as [Protocol],
        currRow.SiteNumber							as [SiteNumber],
        @SubjectNumber							as [SubjectNumber],
		currRow.VisitId							as [VisitId],
		v.[Name]								as [VisitName],
		'Patient Visit Status'					as [SubjectVisitAttribute],
		priorVisit.[Name]						as [OldValue],
		currVisit.[Name]						as [NEWVALUE],
		case
		when priorRow.PatientVisitStatusTypeId is null
		then 'New'
		else 'Update'
		end 									as [ChangeReasonType],
		FORMAT(currRow.ModifiedDate, @dtFormat) as [ChangedDate],
		CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER,  currRow.ModifiedBy) IS NOT NULL 
		THEN  
			CASE WHEN su.UserName IS NOT NULL 
					THEN su.UserName 
					ELSE 'Device'							
			END
		ELSE currRow.ModifiedBy
		END AS ChangedBy,
	    IsNull(c.reasonForCorrection,'')		as [CorrectionReason],
        IsNull(format(c.DataCorrectionNumber, '0000'),'')	as [DCFNumber],
	    case when currRow.AuditSource = 'W' then 'Web Backup' else IsNull(dv.assetTag,'') end as [AssetTag],
		currRow.modifiedDate as Date_time,
		v.VisitOrder,
		1 AS SortOrder
from #AuditRows currRow
left join #AuditRows PriorRow on PriorRow.id=CurrRow.Id-1 and currrow.VisitID=PriorRow.visitID
outer apply (
	SELECT TOP 1 * FROM #AuditRows NextRow 
	WHERE NextRow.Id>CurrRow.Id and currRow.VisitId=NextRow.VisitId and (currRow.PatientVisitStatusTypeId<>NextRow.PatientVisitStatusTypeId or NextRow.PatientVisitStatusTypeId is null)
	ORDER BY VisitOrder,ModifiedDate
) NextRow
left JOIN @patientVisitStatusType PriorVisit ON PriorVisit.Id = priorRow.PATIENTVISITSTATUSTYPEID
left JOIN @patientVisitStatusType CurrVisit ON CurrVisit.Id = currRow.PATIENTVISITSTATUSTYPEID
LEFT JOIN [dbo].device dv on dv.id=currRow.AuditDeviceId
LEFT OUTER JOIN [dbo].[StudyUser] [su] on TRY_CAST(currRow.ModifiedBy as uniqueidentifier) <> null AND [su].[Id] = currRow.ModifiedBy
LEFT JOIN [dbo].Correction c on c.id=currRow.AuditCorrectionId
LEFT JOIN @visit v on v.Id=currRow.VisitId
where (currRow.PatientVisitStatusTypeId <> priorRow.PatientVisitStatusTypeId
       or priorRow.PatientVisitStatusTypeId is null)
union
select	
		@StudyName,
        currRow.SiteNumber,
        @SubjectNumber,
		currRow.VisitId,
		v.[Name],
		'Visit Date',
		FORMAT(priorRow.VisitDate, @dtFormat),
		FORMAT(currRow.VisitDate, @dtFormat),
		case
		when priorRow.VisitDate is null
		then 'New'
		else 'Update'
		end,
		FORMAT(currRow.ModifiedDate, @dtFormat),
		CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER,  currRow.ModifiedBy) IS NOT NULL 
		THEN  
			CASE WHEN su.UserName IS NOT NULL 
					THEN su.UserName 
					ELSE 'Device'							
			END
		ELSE currRow.ModifiedBy
		END,
	    IsNull(c.reasonForCorrection,''),
        IsNull(format(c.DataCorrectionNumber, '0000'),''),
	    case when currRow.AuditSource = 'W' then 'Web Backup' else IsNull(dv.assetTag,'') end,
		currRow.modifiedDate,
		v.VisitOrder,
		2 AS SortOrder
from #AuditRows currRow
left join #AuditRows PriorRow on PriorRow.id=CurrRow.Id-1 and currrow.VisitID=PriorRow.visitID
LEFT JOIN [dbo].device dv on dv.id=currRow.AuditDeviceId and currRow.AuditDeviceId is not null 
LEFT JOIN [dbo].Correction c on c.id=currRow.AuditCorrectionId
LEFT OUTER JOIN [dbo].[StudyUser] [su] on TRY_CAST(currRow.ModifiedBy as uniqueidentifier) <> null AND [su].[Id] = currRow.ModifiedBy
LEFT JOIN @visit v on v.Id=currRow.VisitId
where (currRow.visitDate <> PriorRow.visitDate) 
     or (priorRow.visitDate is null and currRow.VisitDate is not null)
union
select	
		@StudyName,
        currRow.SiteNumber,
        @SubjectNumber,
		currRow.VisitId,
		v.[Name],
		'Visit Activation Date',
		FORMAT(priorRow.VisitActivationDate, @dtFormat),
		FORMAT(currRow.VisitActivationDate, @dtFormat),
		case
		when priorRow.VisitActivationDate is null
		then 'New'
		else 'Update'
		end,
		FORMAT(currRow.ModifiedDate, @dtFormat),
		CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER,  currRow.ModifiedBy) IS NOT NULL 
		THEN  
			CASE WHEN su.UserName IS NOT NULL 
					THEN su.UserName 
					ELSE 'Device'							
			END
		ELSE currRow.ModifiedBy
		END,
	    IsNull(c.reasonForCorrection,''),
        IsNull(format(c.DataCorrectionNumber, '0000'),''),
	    case when currRow.AuditSource = 'W' then 'Web Backup' else IsNull(dv.assetTag,'') end,
		currRow.modifiedDate,
		v.VisitOrder,
		3 AS SortOrder
from #AuditRows currRow
left join #AuditRows PriorRow on PriorRow.id=CurrRow.Id-1 and currrow.VisitID=PriorRow.visitID
LEFT JOIN [dbo].device dv on dv.id=currRow.AuditDeviceId and currRow.AuditDeviceId is not null 
LEFT JOIN [dbo].Correction c on c.id=currRow.AuditCorrectionId
LEFT OUTER JOIN [dbo].[StudyUser] [su] on TRY_CAST(currRow.ModifiedBy as uniqueidentifier) <> null AND [su].[Id] = currRow.ModifiedBy
LEFT JOIN @visit v on v.Id=currRow.VisitId
where (currRow.VisitActivationDate <> PriorRow.VisitActivationDate) 
     or (priorRow.VisitActivationDate is null and currRow.VisitActivationDate is not null)
order by v.VisitOrder,currRow.ModifiedDate, SortOrder


drop table #AuditRows

END
GO



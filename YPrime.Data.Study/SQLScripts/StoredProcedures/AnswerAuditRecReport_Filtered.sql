if exists(select * from information_schema.routines where specific_name = 'AnswerAuditRecReport_Filtered')
DROP PROCEDURE [dbo].[AnswerAuditRecReport_Filtered]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[AnswerAuditRecReport_Filtered]
	@SiteNumber varchar(max) = '',
	@SubjectNumber varchar(50) = '',
	@Protocol varchar(max) = '',
	@QuestionnaireJson varchar(max) = '',
	@QuestionJson varchar(max) = '',
	@ChoiceJson varchar(max) = '',
	@PaperCorrectionTypeId varchar(max) = ''
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @correctionCompletedStatusId VARCHAR(36) = '14FC9304-7684-4BB1-8F1D-3B21302BE582';
	DECLARE @dtFormat VARCHAR(20) = 'dd-MMM-yyyy hh:mm tt';
	DECLARE @dtWithTimeZoneFormat VARCHAR(24) = 'dd-MMM-yyyy hh:mm tt zzz';
	DECLARE @DefaultCulture VARCHAR(5) = 'en-US';
	DECLARE @SiteName VARCHAR(255);
	DECLARE @StudyName VARCHAR(255) = @Protocol;
	DECLARE @CelsiusSuffix NVARCHAR(1) = N'℃';
	DECLARE @FahrenheitSuffix NVARCHAR(1) = N'℉';
	DECLARE @WebBackupName VARCHAR(10) = 'Web Backup';
	SELECT @SiteName = siteNumber FROM dbo.[site] WHERE id=@SiteNumber

	DECLARE @questionnaire TABLE
	(
		Id uniqueidentifier,
		Name varchar(max),
		InternalName varchar(max)
	);

	DECLARE @question TABLE
	(
		Id uniqueidentifier,
		Name varchar(max),
		QuestionnaireId uniqueidentifier,
		Sequence int,
		InputFieldTypeId int
	);

	DECLARE @choice TABLE
	(
		Id uniqueidentifier,
		Name varchar(max),
		QuestionId uniqueidentifier
	);

	DECLARE @inputFieldType TABLE
	(
		Id int,
		Name varchar(50)
	);

	INSERT INTO @questionnaire
	SELECT *
	FROM
	OPENJSON(@QuestionnaireJson)
	WITH
	(
		Id uniqueidentifier,
		Name varchar(max),
		InternalName varchar(max)
	);

	INSERT INTO @question
	SELECT *
	FROM
	OPENJSON(@QuestionJson)
	WITH
	(
		Id uniqueidentifier,
		Name varchar(max),
		QuestionnaireId uniqueidentifier,
		Sequence int,
		InputFieldTypeId int
	);

	INSERT INTO @choice
	SELECT *
	FROM
	OPENJSON(@choiceJson)
	WITH
	(
		Id uniqueidentifier,
		Name varchar(max),
		QuestionId uniqueidentifier
	);

	-- Find all Add records from choice questions
	SELECT	@studyName as Protocol,
			@siteName as SiteNumber,
			p.PatientNumber as SubjectNumber,
			CAST(FORMAT(de.DIARYDATE, @dtFormat) AS VARCHAR(11))   as [DiaryDate],
			qn.InternalName as [Questionnaire],
			dbo.StripHtml(q.Name) as Question,
			STRING_AGG(case when a.AuditAction = 'D' OR (crd.OldDataPoint = crd.NewDataPoint AND crd.OldDisplayValue IS NOT NULL)  then dbo.stripHtml(c.Name) else null end ,', ') as OldValue,
			case
				when CHARINDEX('A',STRING_AGG(a.AuditAction,'')) > 0  -- at least one Add check for multiselect
				then STRING_AGG(case when a.AuditAction != 'D' then dbo.stripHtml(c.Name) else null end ,', ') 
                else ''
            end as NewValue,
			case
			when IsNull(CAST(IsNull(crd.CorrectionId, CR.Id) as VARCHAR(36)),'') = '' OR de.DataSourceId = 4 then 'New'
			else 'Update' end
			as ChangeReasonType,
			CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER,  a.ModifiedBy) IS NOT NULL THEN su.username ELSE  a.ModifiedBy END AS ChangedBy,
			FORMAT(Max(a.ModifiedDate),@dtWithTimeZoneFormat) as ChangedDate,
			case when IsNull(crd.CorrectionId, CR.Id) is null then '' else IsNull(crd.ReasonForCorrection, cr.ReasonForCorrection) end as [CorrectionReason],
			case when IsNull(crd.CorrectionId, CR.Id) is null then '' else RIGHT('0000' + CONVERT(VARCHAR(4), IsNull(crd.DataCorrectionNumber, cr.DataCorrectionNumber)),4) end as [DCFNumber],
			case when AuditSource = 'W' then @WebBackupName else IsNull(dv.assetTag,'') end as [AssetTag],
			q.[Sequence] as questionOrder,
			q.InputFieldTypeId
			FROM ypaudit.Answer a
			JOIN dbo.DiaryEntry de ON  de.id=a.DiaryEntryId
			JOIN dbo.patient p ON  p.id=de.PatientId
			JOIN @questionnaire qn ON  qn.id=de.QuestionnaireId
			JOIN @question q ON  q.QuestionnaireId = qn.id and a.QuestionId=q.id
			JOIN @choice c ON  c.QuestionId=q.id and c.Id=a.ChoiceId
			LEFT JOIN [dbo].device dv ON  dv.id=a.AuditDeviceId
			LEFT OUTER JOIN StudyUser su ON TRY_CAST(a.ModifiedBy as uniqueidentifier) <> null AND su.Id = a.ModifiedBy
			LEFT JOIN dbo.Correction CR ON  CR.Id = a.AuditCorrectionId
			LEFT JOIN (SELECT crd.*,
						      ISNULL(rowToCorrectionId.AuditCorrectionId, crd.CorrectionId) As TrueCorrectionId,
							  CR.ReasonForCorrection,
							  CR.DataCorrectionNumber
						FROM dbo.CorrectionApprovalData crd  
						JOIN dbo.Correction CR ON  CR.Id = crd.CorrectionId AND CR.CorrectionStatusId = @correctionCompletedStatusId
						LEFT JOIN (  SELECT Id, AuditCorrectionId FROM ypaudit.Answer where AuditCorrectionId IS NOT NULL) rowToCorrectionId
						on crd.RowId = rowToCorrectionId.Id AND NOT EXISTS (SELECT * FROM dbo.CorrectionApprovalData crd WHERE rowToCorrectionId.Id = crd.RowId AND rowToCorrectionId.AuditCorrectionId = crd.CorrectionId)) crd 
			 ON (crd.CorrectionId = CR.Id or crd.TrueCorrectionId = CR.Id) AND crd.NewDataPoint=convert(nvarchar(36), a.ChoiceId) 
	WHERE a.choiceID is not null and p.PatientNumber=@subjectNumber
			and (
				(AuditSource='D' and a.IsDirty =1 and a.AuditAction='A') OR
				(AuditSource='W' and a.IsDirty =1 and a.AuditAction='A') OR
				(AuditSource='A' and a.AuditCorrectionId is not null and a.AuditAction<>'U')
			)
	
	GROUP BY p.PatientNumber,de.DiaryDate,de.DataSourceId,qn.InternalName,dv.AssetTag,q.[Sequence],IsNull(crd.CorrectionId, CR.Id),
			IsNull(crd.ReasonForCorrection, CR.ReasonForCorrection),IsNull(crd.DataCorrectionNumber, CR.DataCorrectionNumber),q.Name,
			a.ModifiedBy,a.AuditSource, su.UserName, q.InputFieldTypeId, q.Id
	UNION
	-- Find all A records from Free-form answers
	select	@studyName as Protocol,
			@siteName as SiteNumber,
			p.PatientNumber as SubjectNumber,
			CAST(FORMAT(de.DIARYDATE, @dtFormat) AS VARCHAR(11))   as [DiaryDate],
			qn.InternalName,
			dbo.stripHtml(q.Name) as questionText,
			case 
				when crd.oldDisplayValue is null then ''
				when q.InputFieldTypeId = 29 then -- Temperature Control
					case
						when crd.OldDisplayValue NOT LIKE N'%℃%' AND crd.OldDisplayValue NOT LIKE N'%℉%'
							then Concat(crd.oldDisplayValue, @CelsiusSuffix)
						else crd.OldDisplayValue
					end
				else crd.oldDisplayValue
			end as OldValue,
			case 
				when a.AuditAction = 'D' then '' 
				when q.InputFieldTypeId = 29 then -- Temperature Control
					case
						when a.FreeTextAnswer LIKE '%C%'
							then Replace(a.FreeTextAnswer, 'C', @CelsiusSuffix)
						when a.FreeTextAnswer like '%F%'
							then Replace(a.FreeTextAnswer, 'F', @FahrenheitSuffix)
						else Concat(a.FreeTextAnswer, @CelsiusSuffix)
					end
				else a.FreeTextAnswer 
			end as NewValue,
			case
			when IsNull(CAST(cr.id as VARCHAR(36)),'') = '' OR (de.DataSourceId = 4 and cr.CorrectionTypeId=@PaperCorrectionTypeId) then 'New'
			else 'Update' end
			as ChangeReasonType,
			a.ModifiedBy,
			FORMAT(A.modifiedDate,@dtWithTimeZoneFormat) as ModifiedDate,
			case when cr.id is null then '' else CR.ReasonForCorrection end as [CorrectionReason],
			case when cr.id is null then '' else RIGHT('0000' + CONVERT(VARCHAR(4), CR.DataCorrectionNumber),4) end as [DCFNumber],
			case when AuditSource = 'W' then @WebBackupName else IsNull(dv.assetTag,'') end as [AssetTag],
			q.[Sequence] as questionOrder,
			q.InputFieldTypeId
			FROM ypaudit.answer a
			JOIN dbo.DiaryEntry de ON  de.id=a.DiaryEntryId
			JOIN dbo.patient p ON  p.id=de.PatientId
			JOIN @questionnaire qn ON  qn.id=de.QuestionnaireId
			JOIN @question q ON  q.QuestionnaireId = qn.id and a.QuestionId=q.id
			LEFT JOIN [dbo].device dv ON  dv.id=a.AuditDeviceId
			LEFT JOIN dbo.Correction CR ON  CR.Id = a.AuditCorrectionId
			LEFT JOIN dbo.CorrectionApprovalData crd ON crd.correctionID = CR.ID and crd.RowId=a.Id
			WHERE a.choiceID is null and p.PatientNumber=@subjectNumber
			and (
				(AuditSource='D' and a.IsDirty = 1 and a.AuditAction='A') OR
				(a.AuditAction='D') OR
				(AuditSource='W' and a.IsDirty = 1 and a.AuditAction='A') OR
				(AuditSource='A' and (cr.id is not null or dv.AssetTag is not null))
			)
			GROUP BY p.PatientNumber,de.DiaryDate,de.DataSourceId,a.FreeTextAnswer,dv.AssetTag,q.[Sequence],qn.InternalName,cr.Id,cr.ReasonForCorrection,cr.DataCorrectionNumber
					,q.Name,a.ModifiedBy,crd.OldDisplayValue,a.AuditSource,a.AuditAction, a.ModifiedDate, q.InputFieldTypeId, cr.CorrectionTypeId

	UNION
	-- Look for update records with choice ID questions
	SELECT 	@studyName as protocol,
			@siteName as SiteNumber,
			p.PatientNumber as SubjectNumber,
			CAST(FORMAT(de.DIARYDATE, @dtFormat) AS VARCHAR(11))   as [DiaryDate],
			qn.InternalName as [Questionnaire],
			dbo.stripHtml(q.Name) as Question,
			dbo.stripHtml(cPrev.Name) as OldValue,
			dbo.stripHtml(cCurr.Name) as NewValue,
			'Update' as ChangeReasonType,
			curr.ModifiedBy,
			FORMAT(curr.modifiedDate,@dtWithTimeZoneFormat) as ModifiedDate,
			case when cr.id is null then '' else CR.ReasonForCorrection end as [CorrectionReason],
			case when cr.id is null then '' else RIGHT('0000' + CONVERT(VARCHAR(4), CR.DataCorrectionNumber),4) end as [DCFNumber],
			IsNull(dv.assetTag,'') as AssetTag,
			q.[Sequence] as questionOrder,
			q.InputFieldTypeId
		FROM ypaudit.Answer  prev
		JOIN (	select diaryEntryID,questionId,choiceId,a.syncversion ,ModifiedBy,ModifiedDate,AuditSource,AuditDeviceId,a.AuditCorrectionId
				from ypaudit.Answer a
				JOIN dbo.DiaryEntry de ON  a.DiaryEntryId=de.id
				where a.AuditAction in ('U')) curr
				ON prev.DiaryEntryId=curr.DiaryEntryId and prev.QuestionId=curr.QuestionId and prev.SyncVersion = curr.SyncVersion -1
		JOIN dbo.DiaryEntry de ON  de.id=prev.DiaryEntryId
		JOIN @question q ON  curr.questionId = q.id
		JOIN @questionnaire qn ON  qn.id=q.QuestionnaireId
		JOIN @choice cPrev ON  cPrev.QuestionId=prev.QuestionId and cPrev.Id=prev.ChoiceId
		JOIN @choice cCurr ON  cCurr.QuestionId=curr.QuestionId and cCurr.Id=curr.ChoiceId
		JOIN dbo.patient p ON  de.PatientId=p.id and p.PatientNumber=@SubjectNumber
		LEFT JOIN dbo.device dv ON  dv.id=curr.AuditDeviceId
		LEFT JOIN dbo.Correction CR ON  CR.Id = curr.AuditCorrectionId
		WHERE (prev.ChoiceId is not null or curr.ChoiceId is not null) and Prev.ChoiceId <> curr.ChoiceId
	UNION

	SELECT	@studyName,
			@siteName as SiteNumber,
			p.PatientNumber as SubjectNumber,
			CAST(FORMAT(de.DIARYDATE, @dtFormat) AS VARCHAR(11))   as [DiaryDate],
			qn.InternalName as [Questionnaire],
			dbo.stripHtml(q.Name) as Question,
			case 
				when prev.FreeTextAnswer is null then ''
			when q.InputFieldTypeId = 29 then -- Temperature Control
					case
						when prev.FreeTextAnswer LIKE '%C%'
							then Replace(prev.FreeTextAnswer, 'C', @CelsiusSuffix)
						when prev.FreeTextAnswer like '%F%'
							then Replace(prev.FreeTextAnswer, 'F', @FahrenheitSuffix)
						else Concat(prev.FreeTextAnswer, @CelsiusSuffix)
					end
				else prev.FreeTextAnswer
			end as OldValue,
			case 
				when curr.FreeTextAnswer is null then ''
				when q.InputFieldTypeId = 29 then -- Temperature Control
					case
						when curr.FreeTextAnswer LIKE '%C%'
							then Replace(curr.FreeTextAnswer, 'C', @CelsiusSuffix)
						when curr.FreeTextAnswer like '%F%'
							then Replace(curr.FreeTextAnswer, 'F', @FahrenheitSuffix)
						else Concat(curr.FreeTextAnswer, @CelsiusSuffix)
					end
				else curr.FreeTextAnswer
			end as NewValue,
			'Update' as ChangeReasonType,
			curr.ModifiedBy,
			FORMAT(curr.modifiedDate,@dtWithTimeZoneFormat) as ModifiedDate,
			case when cr.id is null then '' else CR.ReasonForCorrection end as [CorrectionReason],
			case when cr.id is null then '' else RIGHT('0000' + CONVERT(VARCHAR(4), CR.DataCorrectionNumber),4) end as [DCFNumber],
			IsNull(dv.assetTag,'') as AssetTag,
			q.[Sequence] as questionOrder,
			q.InputFieldTypeId
	FROM ypaudit.Answer  prev
	JOIN (	select diaryEntryID,questionId,freeTextAnswer,choiceID,a.syncversion ,ModifiedBy,ModifiedDate,AuditSource,AuditDeviceId,AuditCorrectionId
		    from ypaudit.Answer a
			JOIN DiaryEntry de ON  a.DiaryEntryId=de.id
			where AuditAction in ('U')) curr
	ON prev.DiaryEntryId=curr.DiaryEntryId and prev.QuestionId=curr.QuestionId and prev.SyncVersion = curr.SyncVersion -1
	JOIN dbo.DiaryEntry de ON  de.id=prev.DiaryEntryId
	JOIN @question q ON  curr.questionId = q.id
	JOIN @questionnaire qn ON  qn.id=q.QuestionnaireId

	JOIN dbo.patient p ON  de.PatientId=p.id and p.PatientNumber=@SubjectNumber
	LEFT JOIN dbo.device dv ON  dv.id=curr.AuditDeviceId
	LEFT JOIN dbo.Correction CR ON  CR.Id = curr.AuditCorrectionId
	WHERE (prev.ChoiceId is null and curr.choiceId is null) and prev.FreeTextAnswer <> curr.FreeTextAnswer
	ORDER BY ChangedDate,InternalName,q.[Sequence]
END
GO

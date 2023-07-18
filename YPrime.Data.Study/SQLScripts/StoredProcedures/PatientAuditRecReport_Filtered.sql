
if exists(select * from information_schema.routines where specific_name = 'PatientAuditRecReport_Filtered')
DROP PROCEDURE [dbo].PatientAuditRecReport_Filtered
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[PatientAuditRecReport_Filtered]
	@StudyName VARCHAR(255),
	@SubjectNumTranslation VARCHAR(255),
	@PinSize int,
	@SiteNumber varchar(max) = '',
	@PatientId uniqueidentifier,
	@PatientAttributeJson varchar(max),
	@DataTypeJson varchar(max),
	@LanguageJson varchar(max),
	@PatientStatusTypeJson varchar(max),
	@ChoicesJson varchar(max)
AS
BEGIN

SET NOCOUNT ON

DECLARE @FieldsToMask varchar(300) = 'PIN,SecurityAnswer'
DECLARE @dtFormat nvarchar(20) = 'dd-MMM-yyyy hh:mm tt'
DECLARE @dtWithTimeZoneFormat VARCHAR(24) = 'dd-MMM-yyyy hh:mm tt zzz';
DECLARE @DefaultCulture VARCHAR(5) = 'en-US';
DECLARE @PinMask varchar(6) = SUBSTRING('######',1,@PinSize);

SELECT * INTO #allowed FROM [dbo].String_split(@siteNumber,',');

DECLARE @x int
DECLARE @auditSrcToExclude CHAR(1)= 'A';

-- Parse JSON Into Tables
DECLARE @patientAttributeConfig TABLE 
(
	[Id] uniqueidentifier,
	[Name] nvarchar(max),
	[ChoiceType] nvarchar(255),
	[DateFormat] nVarchar(255)
);

INSERT INTO @patientAttributeConfig
SELECT * 
FROM
OPENJSON(@PatientAttributeJson)
WITH
(
	[Id] uniqueidentifier,
	[Name] nvarchar(max),
	[ChoiceType] nvarchar(255),
	[DateFormat] nVarchar(255)
);

DECLARE @dataTypeConfig TABLE 
(
	[Id] nvarchar(255),
	[Name] nvarchar(max)
);

INSERT INTO @dataTypeConfig
SELECT * 
FROM
OPENJSON(@DataTypeJson)
WITH
(
	[Id] nvarchar(255),
	[Name] nvarchar(max)
);

DECLARE @languageConfig TABLE 
(
	[Id] uniqueidentifier,
	[Name] nvarchar(max)
);

INSERT INTO @languageConfig
SELECT * 
FROM
OPENJSON(@LanguageJson)
WITH
(
	[Id] uniqueidentifier,
	[Name] nvarchar(max)
);

DECLARE @patientStatusTypeConfig TABLE 
(
	[Id] int,
	[Name] nvarchar(max)
);

INSERT INTO @patientStatusTypeConfig
SELECT * 
FROM
OPENJSON(@PatientStatusTypeJson)
WITH
(
	[Id] int,
	[Name] nvarchar(max)
);

DECLARE @choicesConfig TABLE 
(
	[Id] uniqueidentifier,
	[Name] nvarchar(max)
);

INSERT INTO @choicesConfig
SELECT * 
FROM
OPENJSON(@ChoicesJson)
WITH
(
	[Id] uniqueidentifier,
	[Name] nvarchar(max)
);

-- see if patient has device records at all, and if not, show api records
SELECT @x = count(1) 
FROM ypaudit.patient 
WHERE id = @patientID 
	and AuditDeviceId IS NOT NULL 
	and AuditAction = 'A';

IF (@x<1) SET @auditSrcToExclude='?';

-- set up data
WITH 
PATIENT_AUDIT AS (	
	SELECT 
		ROW_NUMBER() OVER (PARTITION BY PAT.SITEID, PAT.Id ORDER BY PAT.SITEID, PAT.Id, MODIFIEDDATE) 
		AUDIT_SEQ,
		PAT.[Id],
		PAT.SITEID, 
		PATIENTNUMBER, 
		CASE WHEN PAT.MODIFIEDBY IS NULL THEN PAT.PatientNumber
			WHEN PAT.MODIFIEDBY = '' THEN PAT.PatientNumber
			ELSE PAT.MODIFIEDBY END AS MODIFIEDBY,
		MODIFIEDDATE,
		dv.assetTag AS ASSETTAG, 
		AUDITACTION, 
		COMPLETIONDATE, 
		ENROLLEDDATE, 
		ISHANDHELDTRAININGCOMPLETE, 
		LANGUAGEID,
		PATIENTSTATUSTYPEID, 
		PIN, 
		SECURITYANSWER, 
		AuditCorrectionId, 
		null [Weight], 
		AuditSource,
		AuditDeviceId
	FROM [ypaudit].[Patient] PAT
		JOIN #allowed al on al.[Data]=cast(PAT.SiteID AS VARCHAR(36))
		LEFT JOIN [dbo].device dv on dv.id=AuditDeviceId
	WHERE PAT.id=@patientID
),
PATIENT_ATTRIBUTE_AUDIT AS (
	SELECT ROW_NUMBER() OVER (PARTITION BY SITEID, PATIENTNUMBER ORDER BY SITEID, PATIENTNUMBER, MODIFIEDDATE) AUDIT_SEQ,
		[p].[SITEID], 
		[p].[PATIENTNUMBER], 
		CASE WHEN [MODIFIEDBY] IS NULL THEN [P].[patientNumber]
			WHEN [MODIFIEDBY] = '' THEN [P].[patientNumber]
			ELSE [MODIFIEDBY] END AS [MODIFIEDBY], 
		[MODIFIEDDATE]
		,null [ASSETTAG]
		,[AUDITACTION]
		,null [COMPLETIONDATE]
		,null [ENROLLEDDATE]
		,null [ISHANDHELDTRAININGCOMPLETE]
		,null [LANGUAGEID]
		,null [PATIENTSTATUSTYPEID]
		,null [PIN]
		,null [SECURITYANSWER]
		,[pa].[AuditCorrectionId]
		,[pa].[AttributeValue] [Weight],
		[pa].[AuditSource],
		[pa].[AuditDeviceId],
		[pd].[Name],
		[dt].[Name] AS [DataTypeName],
		pd.DateFormat
	FROM [ypaudit].[PatientAttribute] pa
		 JOIN [PATIENTATTRIBUTE] [PT] ON CAST([PA].[ID]  AS varchar(36))= CAST([PT].[ID] AS varchar(36))
		 join @patientAttributeConfig [pd] on [pd].[id]=[pa].[PatientAttributeConfigurationDetailId]
		 join @dataTypeConfig [dt] on [pd].[ChoiceType]=[dt].[Id]
		 join [patient] [p] on [p].[id] = [PT].[patientid]
		 JOIN #allowed [al] on [al].[Data]=cast([P].[SiteID] AS VARCHAR(36))
	 WHERE [P].[id]=@patientID
),
AUDIT_DATA AS (
	SELECT 
		[AUDIT_SEQ],
		[Id],
		[SITEID], 
		[PATIENTNUMBER], 
		[MODIFIEDBY], 
		[MODIFIEDDATE],
		[ASSETTAG], 
		[AUDITACTION],
		[COMPLETIONDATE], 
		[ENROLLEDDATE], 
		[ISHANDHELDTRAININGCOMPLETE], 
		[LANGUAGEID],
		[PATIENTSTATUSTYPEID], 
		[PIN], 
		[SECURITYANSWER], 
		[AuditCorrectionId],
		[AuditSource],
		[Weight],
		[AuditDeviceId]
	FROM PATIENT_AUDIT
),
AUD_DATA_COMPARE AS (
	SELECT
		[A].[AUDIT_SEQ],
		[A].[Id],
		[A].[SITEID],
		[A].[PATIENTNUMBER],
		[A].[MODIFIEDBY],
		[A].[MODIFIEDDATE],
		[A].[ASSETTAG], 
		[A].[AUDITACTION],
		[A].[COMPLETIONDATE],
		[A].[ENROLLEDDATE], 
		[A].[ISHANDHELDTRAININGCOMPLETE],
		[A].[LANGUAGEID], 
		[A].[PATIENTSTATUSTYPEID], 
		[A].[PIN],  
		[A].[SECURITYANSWER], 
		[A].[weight],
		[B].[AUDIT_SEQ] AUDIT_SEQ_N, 
		[B].[Id] ID_N,
		[B].[MODIFIEDBY] MODIFIEDBY_N, 
		[B].[MODIFIEDDATE] MODIFIEDDATE_N,
		[B].[ASSETTAG] ASSETTAG_N, 
		[B].[AUDITACTION] AUDITACTION_N,
		[B].[COMPLETIONDATE] COMPLETIONDATE_N, 
		[B].[ENROLLEDDATE] ENROLLEDDATE_N, 
		[B].[ISHANDHELDTRAININGCOMPLETE] ISHANDHELDTRAININGCOMPLETE_n, 
		[B].[LANGUAGEID] LANGUAGEID_N,
		[B].[PATIENTNUMBER] PATIENTNUMBER_N, 
		[B].[PATIENTSTATUSTYPEID] PATIENTSTATUSTYPEID_N, 
		[B].[PIN] PIN_N,
		[B].[SECURITYANSWER] SECURITYANSWER_N, 
		[B].[AuditCorrectionId],
		[B].[weight] WEIGHT_N,
		[B].[AuditSource],
		[B].AuditDeviceId
	FROM [AUDIT_DATA] [A]
	LEFT JOIN [AUDIT_DATA] [B] ON 
		CAST([A].[SITEID] AS varchar(36)) = CAST([B].[SITEID] AS varchar(36)) 
		AND CAST([A].[Id] AS varchar(36)) = CAST([B].[Id] AS varchar(36))
		AND [A].[AUDIT_SEQ] + 1 = [B].[AUDIT_SEQ]
),
AUD_PAT_NUM AS (
	SELECT DISTINCT 
		aud_pat_A.SiteId, 
		aud_pat_A.AuditCorrectionId, 
		aud_pat_A.AuditSource, 
		aud_pat_A.AuditDeviceId, 
		aud_pat_A.PatientNumber, 
		aud_pat_A.ModifiedDate, 
		aud_pat_A.Id, 
		aud_pat_B.PatientNumber PatientNumber_N, 
		aud_pat_B.AuditCorrectionId AuditCorrectionId_N, 
		aud_pat_B.ModifiedBy ModifiedBy_N, 
		aud_pat_B.ModifiedDate ModifiedDate_N, 
		aud_pat_B.Id Id_N
	FROM [ypaudit].[Patient] aud_pat_A
	join AUDIT_DATA ad on aud_pat_A.PatientNumber = ad.PatientNumber
	join [ypaudit].[Patient] aud_pat_B on aud_pat_A.Id = aud_pat_B.Id 
	where aud_pat_B.PatientNumber != aud_pat_A.PatientNumber 
		and ad.SiteId = aud_pat_A.SiteId
),
AUD_PAT_NUM_COMPARE AS  ( 
	SELECT DISTINCT 
		PatientNumber_N, 
		AUD_PAT_NUM.PatientNumber, 
		ModifiedBy_N, 
		AuditCorrectionId_N, 
		MIN(AuditDeviceId) 
		AuditDeviceId, 
		MIN(AuditSource) AuditSource, 
		SiteId, 
		ModifiedDate_N 
	FROM AUD_PAT_NUM 
	WHERE ModifiedDate_N > ModifiedDate
	GROUP BY 
		PatientNumber_N, 
		PatientNumber, 
		ModifiedBy_N, 
		AuditCorrectionId_N, 
		SiteId, 
		ModifiedDate_N
),
AUD_DATA_ATTR_COMPARE AS (
	SELECT 
		[A].[AUDIT_SEQ],
		[A].[SITEID], 
		[A].[PATIENTNUMBER], 
		[A].[MODIFIEDBY],
		[A].[MODIFIEDDATE],
		[A].[weight],
		[B].[AUDIT_SEQ] [AUDIT_SEQ_N],
		[B].[MODIFIEDBY] [MODIFIEDBY_N],
		[B].[MODIFIEDDATE] [MODIFIEDDATE_N],
		[A].[AuditCorrectionId],
		[B].[weight] [WEIGHT_N],
		[B].[AuditSource],
		[B].[AuditDeviceId],
		[A].[Name] AS [OldName],
		[B].[Name] AS [NewName],
		[A].[DataTypeName] AS [DataTypeName],
		a.DateFormat
	FROM [PATIENT_ATTRIBUTE_AUDIT] [A]
    LEFT JOIN [PATIENT_ATTRIBUTE_AUDIT] [B] ON [A].[SITEID] = [B].[SITEID] 
		AND [A].[PATIENTNUMBER] = [B].[PATIENTNUMBER]
		AND [A].[Name] = [B].[Name]
		AND [A].[AUDIT_SEQ] + 1 = [B].[AUDIT_SEQ]
	WHERE [B].[AUDIT_SEQ] IS NOT NULL 
		OR [A].[AuditCorrectionId] IS NOT NULL
)

-- select final results
SELECT DISTINCT 
       @StudyName		       AS [Protocol],
       B.SITENUMBER            AS [SiteNumber],
       A.PATIENTNUMBER         AS [SubjectNumber],
	   A.SUBJECT_ATTRIBUTE     AS [SubjectAttribute],
       CASE WHEN ISDATE(A.OLD_VALUE) = 1 THEN 
                 CAST(FORMAT(PARSE(A.OLD_VALUE AS DATETIME), @dtFormat) AS VARCHAR)
	        ELSE A.OLD_VALUE
       END                     AS [OldValue],
       CASE WHEN ISDATE(A.NEW_VALUE) = 1 THEN 
                 CAST(FORMAT(PARSE(A.NEW_VALUE AS DATETIME), @dtFormat) AS VARCHAR)
	        ELSE A.NEW_VALUE
       END                     AS [NewValue],
       A.CHANGE_REASON_TYPE    AS [ChangeReasonType],
       A.CHANGE_REASON_COMMENT AS [ChangeReasonComment],	  
	  case when  (A.CHANGED_BY = '' OR A.CHANGED_BY IS null) THEN 'SYSTEM' ELSE (CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER,  A.CHANGED_BY) IS NOT NULL THEN su.username ELSE  A.CHANGED_BY END ) end AS [ChangedBy],
	  FORMAT(A.DATE_TIME , @dtWithTimeZoneFormat) AS [ChangedDate],
	   case when AuditCorrectionId is null then '' else CR.ReasonForCorrection end AS [CorrectionReason],
	   case when AuditCorrectionId is null then '' else RIGHT('0000' + CONVERT(VARCHAR(4), CR.DataCorrectionNumber),4) end AS [DCFNumber],
       case 
		 when AuditSource = 'W' then 'Web Backup' 
		 when AuditSource = 'A' AND AuditCorrectionId IS NOT NULL then ''
		 else IsNull(dv.assetTag,'') 
	   end AS [AssetTag],
	   SWITCHOFFSET(A.DATE_TIME, '+00:00')  AS dtTime   
FROM (
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		'Asset Tag' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		ASSETTAG AS  NEW_VALUE,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,AuditSource,
		AuditDeviceId,
		'Asset Tag' AS FromQ
	FROM AUDIT_DATA
	WHERE AUDITACTION = 'A' 
		AND ASSETTAG IS NOT NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		@subjectNumTranslation + ' Number' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		PATIENTNUMBER AS NEW_VALUE,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Patient Number' AS FromQ
	FROM AUDIT_DATA
	WHERE AUDITACTION = 'A' 
		AND PATIENTNUMBER IS NOT NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		'Completion Date' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		CASE 
			WHEN COMPLETIONDATE IS NULL THEN NULL
			WHEN COMPLETIONDATE < CONVERT(DATETIME, '1901-01-01') THEN NULL
			ELSE FORMAT(COMPLETIONDATE, @dtFormat)
		END  NEW_VALUE,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Completion Date' AS FromQ
	FROM AUDIT_DATA
	WHERE AUDITACTION = 'A' 
		AND COMPLETIONDATE IS NOT NULL 
		AND COMPLETIONDATE > CONVERT(DATETIME, '1901-01-01')

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		'Enrolled Date' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		CASE 
			WHEN ENROLLEDDATE IS NULL THEN NULL
			WHEN ENROLLEDDATE < CONVERT(DATETIME, '1901-01-01') THEN NULL
			ELSE FORMAT(ENROLLEDDATE,@dtFormat)
		END NEW_VALUE,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Enrolled' AS FromQ
	FROM AUDIT_DATA
	WHERE AUDITACTION = 'A' 
		AND ENROLLEDDATE IS NOT NULL 
		AND ENROLLEDDATE > CONVERT(DATETIME, '1901-01-01')

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		'Is Handheld Training Completed' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		CASE 
			WHEN CAST(ISHANDHELDTRAININGCOMPLETE AS VARCHAR)='0' THEN 'No'
			ELSE 'Yes' 
		END AS New_Value,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Handheld Training' AS FromQ
	FROM AUDIT_DATA
	WHERE AUDITACTION = 'A' 
		AND ISHANDHELDTRAININGCOMPLETE IS NOT NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		'Language' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		l.Name NEW_VALUE,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Language' AS FromQ
	FROM AUDIT_DATA
	JOIN @languageConfig l on l.id=LanguageId
	WHERE AUDITACTION = 'A' 
		AND LANGUAGEID IS NOT NULL

	UNION
	SELECT 
		T1.SITEID,
		T1.PATIENTNUMBER,
		T1.AUDIT_SEQ,
		'Current Status' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		T2.Name NEW_VALUE,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		T1.MODIFIEDBY CHANGED_BY,
		T1.MODIFIEDDATE DATE_TIME,
		T1.AuditCorrectionId,
		T1.AuditSource,
		AuditDeviceId,
		'Current Status' AS FromQ
	FROM AUDIT_DATA T1
	LEFT JOIN @patientStatusTypeConfig T2 ON T1.PATIENTSTATUSTYPEID  = T2.ID
	WHERE T1.AUDITACTION = 'A' 
		AND T1.PATIENTSTATUSTYPEID IS NOT NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		'PIN'         AS SUBJECT_ATTRIBUTE,
		''            AS OLD_VALUE,
		@PinMask      AS NEW_VALUE,
		'New'         AS CHANGE_REASON_TYPE,
		''            AS CHANGE_REASON_COMMENT,
		MODIFIEDBY    AS CHANGED_BY,
		MODIFIEDDATE  AS DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Pin' AS FromQ
	FROM AUDIT_DATA
	WHERE AUDITACTION = 'A' 
		AND PIN IS NOT NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ,
		'Security Answer' SUBJECT_ATTRIBUTE,
		'' OLD_VALUE,
		@PinMask AS  NEW_VALUE, 
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Security Answer' AS FromQ
	FROM AUDIT_DATA
	WHERE AUDITACTION = 'A' 
		AND SECURITYANSWER IS NOT NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ_N,
		'Completion Date' SUBJECT_ATTRIBUTE,
		CASE WHEN COMPLETIONDATE IS NULL THEN NULL
			WHEN COMPLETIONDATE < CONVERT(DATETIME, '1901-01-01') THEN NULL
			ELSE FORMAT(COMPLETIONDATE,@dtFormat)
		END OLD_VALUE,
		CASE WHEN COMPLETIONDATE_N IS NULL THEN NULL
			WHEN COMPLETIONDATE_N < CONVERT(DATETIME, '1901-01-01') THEN NULL
			ELSE FORMAT(COMPLETIONDATE,@dtFormat)
		END NEW_VALUE,
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY_N CHANGED_BY,
		MODIFIEDDATE_N DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Completion Date-Update' AS FromQ
	FROM AUD_DATA_COMPARE
	WHERE AUDIT_SEQ_N IS NOT NULL 
		AND ISNULL(CAST(COMPLETIONDATE AS VARCHAR), 'XYZ') <> ISNULL(CAST(COMPLETIONDATE_N AS VARCHAR), 'XYZ')

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER_N,
		AUDIT_SEQ_N,
		'Enrolled Date' SUBJECT_ATTRIBUTE,
		CASE WHEN ENROLLEDDATE IS NULL THEN NULL
			WHEN ENROLLEDDATE < CONVERT(DATETIME, '1901-01-01') THEN NULL
			ELSE FORMAT(ENROLLEDDATE,@dtFormat)
		END OLD_VALUE,
		CASE WHEN ENROLLEDDATE_N IS NULL THEN NULL
			WHEN ENROLLEDDATE_N < CONVERT(DATETIME, '1901-01-01') THEN NULL
			ELSE FORMAT(ENROLLEDDATE_N,@dtFormat)
		END NEW_VALUE,
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY_N CHANGED_BY,
		MODIFIEDDATE_N DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Enrolled Date-Update' AS FromQ
	FROM AUD_DATA_COMPARE
	WHERE AUDIT_SEQ_N IS NOT NULL 
		AND ENROLLEDDATE <> ENROLLEDDATE_N 

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ_N,
		'Is Handheld Training Completed' SUBJECT_ATTRIBUTE,
		CASE
			WHEN CAST(ISHANDHELDTRAININGCOMPLETE AS VARCHAR)='0' THEN 'No'
			ELSE 'Yes' 
		END AS Old_Value,
		CASE
			WHEN CAST(ISHANDHELDTRAININGCOMPLETE_N AS VARCHAR)='0' THEN 'No'
			ELSE 'Yes' 
		END AS New_Value,
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY_N CHANGED_BY,
		MODIFIEDDATE_N DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Handheld training update' AS FromQ
	FROM AUD_DATA_COMPARE
	WHERE AUDIT_SEQ_N IS NOT NULL 
		AND ISNULL(CAST(ISHANDHELDTRAININGCOMPLETE AS VARCHAR), 'XYZ') <> ISNULL(CAST(ISHANDHELDTRAININGCOMPLETE_N AS VARCHAR), 'XYZ')

	UNION
	SELECT
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ_N,
		'Language' SUBJECT_ATTRIBUTE,
		oldLanguage.Name OLD_VALUE,
		newLanguage.Name NEW_VALUE,
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY_N CHANGED_BY,
		MODIFIEDDATE_N DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Language update' AS FromQ
	FROM AUD_DATA_COMPARE
		JOIN @languageConfig oldLanguage on oldLanguage.id=LANGUAGEID
		JOIN @languageConfig newLanguage on newLanguage.id=LANGUAGEID_N
	WHERE AUDIT_SEQ_N IS NOT NULL 
		AND ISNULL(CAST(LANGUAGEID AS VARCHAR(36)), 'XYZ') <> ISNULL(CAST(LANGUAGEID_N AS VARCHAR(36)), 'XYZ')

	UNION
	SELECT 
		T1.SITEID,
		T1.PATIENTNUMBER,
		T1.AUDIT_SEQ_N,
		'Current Status' SUBJECT_ATTRIBUTE,
		T2.Name OLD_VALUE,
		T3.Name NEW_VALUE,
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		T1.MODIFIEDBY_N CHANGED_BY,
		T1.MODIFIEDDATE_N DATE_TIME,
		T1.AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Status Change' AS FromQ
	FROM AUD_DATA_COMPARE T1
	LEFT JOIN @patientStatusTypeConfig T2 ON T1.PATIENTSTATUSTYPEID  = T2.ID
	LEFT JOIN @patientStatusTypeConfig T3 ON T1.PATIENTSTATUSTYPEID_N  = T3.ID
	WHERE  T1.AUDIT_SEQ_N IS NOT NULL 
		AND ISNULL(CAST(T1.PATIENTSTATUSTYPEID AS VARCHAR(36)), 'XYZ') <> ISNULL(CAST(T1.PATIENTSTATUSTYPEID_N AS VARCHAR(36)), 'XYZ')

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ_N,
		'PIN' SUBJECT_ATTRIBUTE,
		@PinMask AS OLD_VALUE,
		@PinMask AS NEW_VALUE,
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY_N CHANGED_BY,
		MODIFIEDDATE_N DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Pin update' AS FromQ
	FROM AUD_DATA_COMPARE
	WHERE AUDIT_SEQ_N IS NOT NULL 
		AND ISNULL(CAST(PIN AS VARCHAR), 'XYZ') <> ISNULL(CAST(PIN_N AS VARCHAR), 'XYZ') 
		AND AuditCorrectionId IS NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER,
		AUDIT_SEQ_N,
		'Security Answer' SUBJECT_ATTRIBUTE,
		@PinMask AS OLD_VALUE,
		@PinMask AS NEW_VALUE, 
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY_N CHANGED_BY,
		MODIFIEDDATE_N DATE_TIME,
		AuditCorrectionId,
		AuditSource,
		AuditDeviceId,
		'Security Answer' AS FromQ
	FROM AUD_DATA_COMPARE
	WHERE AUDIT_SEQ_N IS NOT NULL 
		AND ISNULL(CAST(SECURITYANSWER AS VARCHAR), 'XYZ') <> ISNULL(CAST(SECURITYANSWER_N AS VARCHAR), 'XYZ') 
		AND AuditCorrectionId IS NULL

	UNION
	SELECT 
		SITEID,
		PATIENTNUMBER_N,
		0 AS AUDIT_SEQ_N,
		@subjectNumTranslation + ' Number' SUBJECT_ATTRIBUTE,
		PATIENTNUMBER AS OLD_VALUE,
		PATIENTNUMBER_N AS NEW_VALUE, 
		'Update' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY_N CHANGED_BY,
		MODIFIEDDATE_N DATE_TIME,
		AuditCorrectionId_N,
		AuditSource,
		AuditDeviceId,'Patient Number' AS FromQ
	FROM AUD_PAT_NUM_COMPARE
) A
JOIN [SITE] [B] ON [B].[ID] = [A].[SITEID]
LEFT JOIN [dbo].[Device] [dv] on [dv].[id]=[AuditDeviceId]
LEFT OUTER JOIN [dbo].[StudyUser] [su] on TRY_CAST(CHANGED_BY as uniqueidentifier) is not null AND [su].[Id] = [CHANGED_BY]
LEFT JOIN [dbo].[Correction] [CR] on [CR].[Id] = [A].[AuditCorrectionId]
WHERE (
	auditsource <> @auditSrcToExclude OR 
	(
		AuditSource='A' 
		AND AuditCorrectionId IS NOT NULL
	) OR
	(
		AuditSource='A' 
		AND AuditCorrectionId IS NULL 
		AND CHANGE_REASON_TYPE='Update' 
		AND 
		(
			SUBJECT_ATTRIBUTE='Current Status' OR
			SUBJECT_ATTRIBUTE='PIN'
		)
		AND su.UserName IS NOT NULL
	)
)

union
SELECT DISTINCT
       @StudyName			     [Protocol], 
       B.SITENUMBER				 [SiteNumber],
       jdc.PATIENTNUMBER         [SubjectNumber],
	   jdc.SUBJECT_ATTRIBUTE     [SubjectAttribute],
	   jdc.OLD_VALUE             [OldValue],
	   jdc.NEW_VALUE             [NewValue],
       jdc.CHANGE_REASON_TYPE    [ChangeReasonType],
       jdc.CHANGE_REASON_COMMENT [ChangeReasonComment],	   
	   case when  (jdc.CHANGED_BY = '' OR jdc.CHANGED_BY IS null) THEN 'SYSTEM' ELSE (CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER,  jdc.CHANGED_BY) IS NOT NULL THEN su.username ELSE  jdc.CHANGED_BY END) end AS [ChangedBy],
	   FORMAT(jdc.DATE_TIME, @dtWithTimeZoneFormat) AS [ChangedDate],
	   case when AuditCorrectionId is null then '' else jdcCR.ReasonForCorrection end AS [CorrectionReason],
	   case when AuditCorrectionId is null then '' else RIGHT('0000' + CONVERT(VARCHAR(4), jdcCR.DataCorrectionNumber),4) end AS [DCFNumber],
	   case when AuditSource = 'W' then 'Web Backup' else IsNull(dv.assetTag,'') end AS [AssetTag],
	   SWITCHOFFSET(jdc.DATE_TIME, '+00:00')  AS dtTime
FROM (
	SELECT 
		apt.SITEID,
		IsNull(apnc.PATIENTNUMBER, apt.PatientNumber) AS PATIENTNUMBER,
		AUDIT_SEQ,
		apt.Name AS SUBJECT_ATTRIBUTE,
		'' AS OLD_VALUE,
		CASE
			WHEN IsNull(apt.DataTypeName,'') IN ('TextAttribute','LettersOnlyAttribute') THEN apt.[Weight]
			WHEN LEN(IsNull(choice.[Name],'')) = 0 THEN 
				CASE 
					WHEN IsDate([Weight]) = 1 THEN FORMAT(CAST(Weight AS DATE),apt.DateFormat)
					ELSE CAST([WEIGHT] AS VARCHAR(max))
				END
			ELSE choice.[Name]
		END AS NEW_VALUE,
		'New' CHANGE_REASON_TYPE,
		'' CHANGE_REASON_COMMENT,
		MODIFIEDBY CHANGED_BY,
		MODIFIEDDATE DATE_TIME,
		AuditCorrectionId,
		apt.AuditSource,
		apt.AuditDeviceId,
		'Attributes-Add' AS FromQ
	FROM PATIENT_ATTRIBUTE_AUDIT apt
	LEFT JOIN AUD_PAT_NUM_COMPARE apnc on apt.PatientNumber = apnc.PatientNumber_N
	LEFT JOIN @choicesConfig choice on choice.id = TRY_CAST([weight] AS uniqueidentifier) 
	WHERE  AUDITACTION = 'A' 
		AND [WEIGHT] IS NOT NULL

	UNION
	SELECT 
		adc.SITEID,
		apnc.PATIENTNUMBER,
		AUDIT_SEQ_N,
		oldName AS  SUBJECT_ATTRIBUTE,
		CASE
			WHEN IsNull(adc.DataTypeName,'') IN ('TextAttribute','LettersOnlyAttribute') THEN CAST([WEIGHT] AS VARCHAR(max))
			WHEN LEN(IsNull(choicesOld.[Name],'')) = 0 THEN 
				CASE 
					WHEN IsDate(Weight) = 1 THEN FORMAT(CAST(Weight AS DATE),adc.DateFormat)
					ELSE CAST(WEIGHT AS VARCHAR(max))
				END
			ELSE choicesOld.[Name]
		END AS OLD_VALUE,
		CASE
			WHEN IsNull(adc.DataTypeName,'') IN ('TextAttribute','LettersOnlyAttribute') THEN CAST([WEIGHT_N] AS VARCHAR(max))
			WHEN LEN(IsNull(choicesNew.[Name],''))=0 THEN 
				CASE 
					WHEN IsDate([WEIGHT_N]) = 1 THEN FORMAT(CAST([WEIGHT_N] AS DATE),adc.DateFormat)
					ELSE CAST([WEIGHT_N] AS VARCHAR(max))
				END
			ELSE choicesNew.[Name]
		END AS NEW_VALUE,
		'Update' AS CHANGE_REASON_TYPE,
		'' AS CHANGE_REASON_COMMENT,
		adc.MODIFIEDBY_N AS CHANGED_BY,
		adc.MODIFIEDDATE_N AS DATE_TIME,
		AuditCorrectionId,
		adc.AuditSource,
		adc.AuditDeviceId,
		'Attributes-update' AS FromQ
	FROM AUD_DATA_ATTR_COMPARE adc
	LEFT JOIN AUD_PAT_NUM_COMPARE apnc ON adc.PatientNumber = apnc.PatientNumber_N
	LEFT JOIN @choicesConfig choicesOld ON choicesOld.Id = TRY_CAST(adc.weight AS uniqueidentifier) 
	LEFT JOIN @choicesConfig choicesNew on choicesNew.Id = TRY_CAST(adc.weight_n AS uniqueidentifier)
	WHERE  AUDIT_SEQ_N IS NOT NULL AND ISNULL(CAST(WEIGHT AS VARCHAR), 'XYZ') <> ISNULL(CAST(WEIGHT_N AS VARCHAR), 'XYZ') 
		AND adc.AuditCorrectionId is null

	UNION
	SELECT 
		adc.SITEID,
	   ISNULL(apnc.PatientNumber, adc.PatientNumber) AS PATIENTNUMBER,
		0 AS AUDIT_SEQ_N,
	   cd.TranslationKey AS  SUBJECT_ATTRIBUTE,
		cd.OldDisplayValue AS Old_Value,
	   cd.NewDisplayValue AS NEW_VALUE,
	   'Update' AS CHANGE_REASON_TYPE,
		'' AS CHANGE_REASON_COMMENT,
		MODIFIEDBY AS CHANGED_BY,
		max(MODIFIEDDATE) AS DATE_TIME,
		AuditCorrectionId,
		'D' AS AuditSource,
		adc.AuditDeviceId,
   		'DCFs' AS FromQ
	FROM AUD_DATA_ATTR_COMPARE adc
	join CorrectionApprovalData cd on cd.CorrectionId=Adc.AuditCorrectionId
	LEFT JOIN AUD_PAT_NUM_COMPARE apnc on adc.PatientNumber = apnc.PatientNumber_N 
		AND adc.ModifiedDate<apnc.ModifiedDate_N 
	WHERE adc.AuditCorrectionId IS NOT NULL AND cd.TableName != 'Patient'
	GROUP BY 
		adc.SITEID,
		adc.PatientNumber,
		apnc.PatientNumber,
		cd.translationkey,
		cd.OldDisplayValue,
		cd.NewDisplayValue,
		MODIFIEDBY,
		AuditCorrectionId,
		adc.AuditDeviceId
) jdc
JOIN SITE B ON B.ID = jdc.SITEID
LEFT JOIN [dbo].device dv on dv.id=AuditDeviceId
LEFT OUTER JOIN StudyUser su on TRY_CAST(CHANGED_BY as uniqueidentifier) is not null AND su.Id = CHANGED_BY
LEFT JOIN Correction jdcCR on CAST(jdcCR.Id AS varchar(36)) = CAST(jdc.AuditCorrectionId AS varchar(36))
where auditsource<>@auditSrcToExclude 
	OR (AuditSource='A' AND AuditCorrectionId IS NOT NULL)
ORDER BY dtTime

DROP TABLE #allowed

END
GO

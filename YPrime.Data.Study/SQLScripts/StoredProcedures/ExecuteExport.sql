
if exists(select * from information_schema.routines where specific_name = 'ExecuteExport')
	DROP PROCEDURE [dbo].[ExecuteExport]
GO

/****** Object:  StoredProcedure [dbo].[ExecuteExport]    Script Date: 1/17/2020 10:06:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Jon O / BS
-- Create date: 9/26/16
-- updated: 1/27/17 by AP
-- Updated: 2/20/17 by AP, update for new SSO User tables
-- Description:	Export SPROC
-- =============================================

CREATE PROCEDURE [dbo].[ExecuteExport]
@userId UNIQUEIDENTIFIER,
@siteId UNIQUEIDENTIFIER, -- updated from varchar(10) to uniqueidentifier by AP.
@patientId UNIQUEIDENTIFIER,
@questionnaireId UNIQUEIDENTIFIER,
@diaryStartDate DATETIMEOFFSET(7),
@diaryEndDate DATETIMEOFFSET(7),
@QuestionnaireJson varchar(max) = '',
@QuestionJson varchar(max) = '',
@ChoiceJson varchar(max) = '',
@VisitJson varchar(max) = ''
AS
BEGIN

-- Init
SET NOCOUNT ON;
DECLARE @cols AS NVARCHAR(MAX);
DECLARE @query AS NVARCHAR(MAX);  
DECLARE @questionnaireSql AS NVARCHAR(MAX);
DECLARE @paramDefinition AS NVARCHAR(500);

-- Questionnaire SQL
SELECT @questionnaireSql = N'
DECLARE @question TABLE 
(
	Id uniqueidentifier, 
	Name varchar(max),
	QuestionnaireId uniqueidentifier,
	Sequence int, 
	InputFieldTypeId int,
	ExportDisplayName varchar(max),
	ExportDisplayOrder int
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
	InputFieldTypeId int,
	ExportDisplayName varchar(max),
	ExportDisplayOrder int
);

SELECT
	@colsPar = STUFF(
	(SELECT
		CASE 
			WHEN max(q.ExportDisplayName) IS NOT NULL THEN '','' + QUOTENAME(max(q.ExportDisplayName)) 
			ELSE 
				CASE 
					WHEN max(q.Name) IS NOT NULL AND max(dbo.StripHTML(q.Name)) <> '''' THEN '','' + QUOTENAME(max(dbo.StripHTML(q.Name))) 
					ELSE ''NOQUESTIONNAME''
				END
		END
	FROM
		@question q
	WHERE 
		q.QuestionnaireId IN 
			(SELECT DISTINCT 
				QuestionnaireId 
			FROM 
				DiaryEntry de
			JOIN 
				Patient p ON p.Id = de.PatientId
			WHERE 
					((@siteId IS NULL OR p.SiteId IN
						(SELECT 
							u.SiteId 
						FROM   
							StudyUserRole u        
						WHERE 
							 u.StudyUserId = @userId)		
					) OR p.SiteId = @siteId) AND
				(@siteId IS NULL OR p.SiteId = @siteId) AND
				(@patientId IS NULL OR de.PatientId = @patientId) AND
				(@questionnaireId IS NULL OR de.QuestionnaireId = @questionnaireId) AND
				(@diaryStartDate IS NULL OR de.StartedTime > @diaryStartDate) AND
				(@diaryEndDate IS NULL OR de.CompletedTime < @diaryEndDate)
	)
GROUP BY 
	q.Id
ORDER BY 
	MAX(q.QuestionnaireId), 
	MAX(q.ExportDisplayOrder) FOR XML PATH(''''),TYPE).value(''.'', ''NVARCHAR(MAX)''),1,1,'''');';  

-- Set params & execute
SET @paramDefinition = N'
	@userId UNIQUEIDENTIFIER,
	@siteId UNIQUEIDENTIFIER,
	@patientId UNIQUEIDENTIFIER,
	@questionnaireId UNIQUEIDENTIFIER,
	@diaryStartDate DATETIMEOFFSET(7),
	@diaryEndDate DATETIMEOFFSET(7),
	@QuestionJson varchar(max),
	@colsPar NVARCHAR(MAX) OUTPUT';
EXEC sp_executesql 
	@questionnaireSql, 
	@paramDefinition, 
	@userId,
	@siteId,
	@patientId,
	@questionnaireId,
	@diaryStartDate,
	@diaryEndDate,
	@QuestionJson,
	@colsPar = @cols OUTPUT;

-- Append Query
SET @query = N'
DECLARE @question TABLE 
(
	Id uniqueidentifier, 
	Name varchar(max),
	QuestionnaireId uniqueidentifier,
	Sequence int, 
	InputFieldTypeId int,
	ExportDisplayName varchar(max),
	ExportDisplayOrder int
);

DECLARE @questionnaire TABLE 
(
	Id uniqueidentifier, 
	Name varchar(max),
	InternalName varchar(max)
);

DECLARE @choice TABLE 
(
	Id uniqueidentifier, 
	Name varchar(max),
	QuestionId uniqueidentifier
);

DECLARE @visit TABLE 
(
	Id uniqueidentifier, 
	Name varchar(max)
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
	InputFieldTypeId int,
	ExportDisplayName varchar(max),
	ExportDisplayOrder int
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

INSERT INTO @choice
SELECT * 
FROM
OPENJSON(@ChoiceJson)
WITH
(
	Id uniqueidentifier, 
	Name varchar(max),
	QuestionId uniqueidentifier
);

INSERT INTO @visit
SELECT * 
FROM
OPENJSON(@VisitJson)
WITH
(
	Id uniqueidentifier, 
	Name varchar(max)
);

SELECT 
	s.Name AS [SiteName],
	p.PatientNumber AS [PatientNumber],
	v.Name AS [VisitName],
	[UserName] = 
	CASE 
		WHEN su.UserName IS NOT NULL THEN su.UserName
		ELSE ''NULL''
	END,
	de.StartedTime AS [StartedTime],
	de.CompletedTime AS [CompletedTime],
	de.TransmittedTime as [TransmittedTime],
	de.Id AS [DiaryEntryId],
	de.QuestionnaireId AS [QuestionnaireId],
	q.InternalName AS [QuestionnaireName],
	FinalAnswer = 
	CASE 
		WHEN qq.InputFieldTypeId = 29 then -- Temperature Control
			CASE
				WHEN a.FreeTextAnswer LIKE ''%C%'' OR a.FreeTextAnswer LIKE ''%F%'' 
					THEN a.FreeTextAnswer
				ELSE CONCAT(a.FreeTextAnswer, CHAR(176), ''C'') 
			END
		WHEN a.FreeTextAnswer IS NOT NULL THEN dbo.StripHTML(a.FreeTextAnswer)
		ELSE replace(replace(dbo.StripHTML(c.Name), ''&lt;'',''<''), ''&gt;'',''>'')
	END, 
	questionForExport = 
	CASE 
		WHEN qq.ExportDisplayName IS NOT NULL THEN qq.ExportDisplayName
		ELSE 
			CASE 
				WHEN qq.Name IS NOT NULL THEN dbo.StripHTML(qq.Name)
				ELSE ''NOQUESTIONNAME''
			END
	END
INTO 
	#temp
FROM 
	DiaryEntry de
LEFT JOIN Answer a on a.DiaryEntryId = de.Id
LEFT JOIN @question qq on a.QuestionId = qq.Id
INNER JOIN Patient p ON de.PatientId = p.Id 
LEFT JOIN @visit v on de.VisitId = v.Id
LEFT JOIN StudyUser su on su.Id = de.UserId
LEFT JOIN @questionnaire q ON de.QuestionnaireId = q.Id
LEFT JOIN [Site] s on p.SiteId = s.Id
LEFT JOIN @choice c on c.Id = a.ChoiceId
WHERE
	(@siteId IS NULL OR p.SiteId = @siteId) AND
	(@patientId IS NULL OR de.PatientId = @patientId) AND
	(@questionnaireId IS NULL OR de.QuestionnaireId = @questionnaireId) AND
	(@diaryStartDate IS NULL OR de.StartedTime > @diaryStartDate) AND
	(@diaryEndDate IS NULL OR de.CompletedTime < @diaryEndDate)';

-- Append Query
SET @query = @query + N'
SELECT 
	[SiteName], 
	[PatientNumber], 
	[VisitName],
	[UserName],
	[StartedTime], 
	[CompletedTime], 
	[TransmittedTime], 
	[DiaryEntryId], 
	[QuestionnaireId], 
	[QuestionnaireName], ' + @cols + N'
FROM 
	(SELECT 
		[SiteName], 
		[PatientNumber], 
		[VisitName],
		[UserName],
		[StartedTime], 
		[CompletedTime], 
		[TransmittedTime], 
		[DiaryEntryId], 
		[QuestionnaireId], 
		[QuestionnaireName], 
		FinalAnswer = STUFF(
			(SELECT 
				'', '' + FinalAnswer 
			FROM 
				#temp b 
			WHERE 
				b.DiaryEntryId = #temp.DiaryEntryId AND
				b.QuestionnaireId = #temp.QuestionnaireId AND
				b.questionForExport = #temp.questionForExport
			FOR XML PATH, TYPE
			).value(''.[1]'', ''nvarchar(max)''), 1, 2, ''''),
		questionForExport
	FROM 
		#temp 
	GROUP BY
		[SiteName], 
		[PatientNumber], 
		[VisitName],
		[UserName],
		[StartedTime], 
		[CompletedTime], 
		[TransmittedTime], 
		[DiaryEntryId], 
		[QuestionnaireId], 
		[QuestionnaireName], 
		questionForExport
	) answers
	PIVOT
	(
		max(FinalAnswer)
		FOR questionForExport IN (' + @cols + N')
	) AS PivotTable'

SET @query = @query + N'
 DROP TABLE #temp;
 ';

SET @paramDefinition = N'
	@siteId UNIQUEIDENTIFIER,
	@patientId UNIQUEIDENTIFIER,
	@questionnaireId UNIQUEIDENTIFIER,
	@diaryStartDate DATETIMEOFFSET(7),
	@diaryEndDate DATETIMEOFFSET(7),
	@QuestionnaireJson varchar(max),
	@QuestionJson varchar(max),
	@ChoiceJson varchar(max),
	@VisitJson varchar(max)';
EXEC sp_executesql 
	@query, 
	@paramDefinition, 
	@siteId,
	@patientId,
	@questionnaireId,
	@diaryStartDate,
	@diaryEndDate,
	@QuestionnaireJson,
	@QuestionJson,
	@ChoiceJson,
	@VisitJson;


--EXEC PrintLargeString @query

END
GO


if exists(select * from information_schema.routines where specific_name = 'ExecuteExportRequest')
	DROP PROCEDURE [dbo].[ExecuteExportRequest]
GO

/****** Object:  StoredProcedure [dbo].[ExecuteExportRequest]    Script Date: 12/8/2016 11:50:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Jon O / BS
-- Create date: 9/26/16
-- Updated: 1/27/17 by AP, Change VARCHAR siteId to GUID
-- Updated: 2/20/17 by AP, update for new SSO User tables 
-- Description:	Execute ExportRequsts
-- =============================================

CREATE PROCEDURE [dbo].[ExecuteExportRequest]
@ExportId UNIQUEIDENTIFIER,
@QuestionnaireJson varchar(max) = '',
@QuestionJson varchar(max) = '',
@ChoiceJson varchar(max) = '',
@VisitJson varchar(max) = ''
AS
BEGIN

-- Init
SET NOCOUNT ON;
DECLARE @cursor CURSOR;
DECLARE @userId UNIQUEIDENTIFIER;
DECLARE @siteId UNIQUEIDENTIFIER;
DECLARE @patientId UNIQUEIDENTIFIER;
DECLARE @questionnaireId UNIQUEIDENTIFIER;
DECLARE @diaryStartDate DATETIMEOFFSET(7);
DECLARE @diaryEndDate DATETIMEOFFSET(7);

-- Set status
UPDATE 
	[Export] 
SET
	ExportStatusId = 2, -- Processing
	StartedTime = GETUTCDATE()
WHERE 
	Id = @ExportId;

-- Set filter params for Export
SELECT 
	@userId = e.UserId,
	@siteId = s.Id, --Updated to pass GUID Id to Export SPROC by AP. (OLD - We need to pass the Export SPROC the VARCHAR(10) SiteID..)
	@patientId = e.PatientId,
	@diaryStartDate = e.DiaryStartDate,
	@diaryEndDate = e.DiaryEndDate
FROM [Export] e
LEFT JOIN [Site] s on s.Id = e.SiteId
WHERE 
	e.Id = @ExportId

-- Execute export
BEGIN 
	SET @cursor = CURSOR FOR
	SELECT DISTINCT
		QuestionnaireId
	FROM
		DiaryEntry d
	JOIN 
		Patient p on p.Id = d.PatientId
	WHERE 
		((@siteId IS NULL OR p.SiteId IN --12/8/16 If SiteID is null include all
			(SELECT 
				u.SiteId 
			FROM   
				StudyUserRole u          
			WHERE 
				 u.StudyUserId = @userId)
		) OR p.SiteId = @siteId) AND
		(@patientId IS NULL OR p.Id = @patientId) AND
		(@diaryStartDate IS NULL OR d.StartedTime > @diaryStartDate) AND
		(@diaryEndDate IS NULL OR d.CompletedTime < @diaryEndDate)

	OPEN @cursor   
	FETCH NEXT FROM @cursor 
	INTO @questionnaireId   

	WHILE @@FETCH_STATUS = 0   
	BEGIN   
		EXEC ExecuteExport @userId, @siteId, @patientId, @questionnaireId, @diaryStartDate, @diaryEndDate, @QuestionnaireJson, @QuestionJson, @ChoiceJson, @VisitJson
		FETCH NEXT FROM @cursor 
		INTO @questionnaireId   
	END   

CLOSE @cursor   
DEALLOCATE @cursor
END

-- Set status
UPDATE 
	[Export] 
SET
	ExportStatusId = 3, -- Complete
	CompletedTime = GETUTCDATE()
WHERE 
	Id = @ExportId;

END


GO

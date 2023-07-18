
IF EXISTS(select * from information_schema.routines where specific_Name = 'VisitComplianceReport')
	Drop procedure VisitComplianceReport
GO


CREATE Procedure [dbo].[VisitComplianceReport]
	@QuestionnaireJson varchar(max) = '',
	@VisitJson varchar(max) = '',
	@VisitQuestionnaireJson varchar(max) = '',
	@PatientVisitStatusTypeJson varchar(max) = '', 
	@PatientStatusTypeJson varchar(max) = '',
	@VisitComplianceReportQuestionnaireTypeJson varchar(max) = ''
AS
BEGIN

	DECLARE @questionnaireNames nvarchar(max);

	CREATE TABLE #questionnaire  
	(
		Id uniqueidentifier, 
		Name varchar(max),
		InternalName varchar(max),
		QuestionnaireTypeId int
	);

	CREATE TABLE #visit  
	(
		Id uniqueidentifier, 
		Name varchar(max),
		IsScheduled bit, 
		VisitOrder int
	);

	CREATE TABLE #visitQuestionnaire  
	(
		VisitId uniqueidentifier, 
		QuestionnaireId uniqueidentifier
	);

	CREATE TABLE #visitComplianceReportQuestionnaireType
	(
		QuestionnaireTypeId int, 
		VisitId uniqueidentifier
	);

	CREATE TABLE #patientVisitStatusType  
	(
		Id int, 
		Name nvarchar(max)
	);

	CREATE TABLE #patientStatusType  
	(
		Id int, 
		Name nvarchar(max), 
		IsRemoved bit
	);

	INSERT INTO #questionnaire
	SELECT * 
	FROM
	OPENJSON(@QuestionnaireJson)
	WITH
	(
		Id uniqueidentifier, 
		Name varchar(max),
		InternalName varchar(max),
		QuestionnaireTypeId int
	);

	INSERT INTO #visit
	SELECT * 
	FROM
	OPENJSON(@VisitJson)
	WITH
	(
		Id uniqueidentifier,
		Name varchar(max),
		IsScheduled bit, 
		VisitOrder int
	);

	INSERT INTO #visitQuestionnaire
	SELECT * 
	FROM
	OPENJSON(@VisitQuestionnaireJson)
	WITH
	(
		VisitId uniqueidentifier, 
		QuestionnaireId uniqueidentifier
	);

	INSERT INTO #visitComplianceReportQuestionnaireType
	SELECT * 
	FROM
	OPENJSON(@VisitComplianceReportQuestionnaireTypeJson)
	WITH
	(
		QuestionnaireTypeId int, 
		VisitId uniqueidentifier
	);

	INSERT INTO #patientVisitStatusType
	SELECT * 
	FROM
	OPENJSON(@PatientVisitStatusTypeJson)
	WITH
	(
		Id int, 
		Name nvarchar(max)
	);

	INSERT INTO #patientStatusType
	SELECT * 
	FROM
	OPENJSON(@PatientStatusTypeJson)
	WITH
	(
		Id int, 
		Name nvarchar(max),
		IsRemoved bit
	);

	IF OBJECT_ID('tempdb..#ComplianceQuestionnaires') IS NOT NULL DROP TABLE #ComplianceQuestionnaires;

	select q.id, vq.VisitId
	into #ComplianceQuestionnaires
	from #visitQuestionnaire vq
	inner join #questionnaire q on vq.QuestionnaireID = q.Id
	inner join #visitComplianceReportQuestionnaireType vcqt on vcqt.VisitId = vq.VisitId and vcqt.QuestionnaireTypeId = q.QuestionnaireTypeId;

	IF OBJECT_ID('tempdb..#MissedDiaries') IS NOT NULL DROP TABLE #MissedDiaries;

	with OrderedVisits as
	(
			select v.Id, row_number() over (order by visitOrder) as VisitOrder
			from #visit v
			where v.IsScheduled = 1
	)
	, NextVisitLookup as
  (
      select ov1.Id as VisitId, ov2.Id as NextVisitId
      from OrderedVisits ov1
             left join OrderedVisits ov2 on ov1.VisitOrder + 1 = ov2.VisitOrder
  )
, NextVisitStatus as
(
  select p.Id as PatientId, pv.VisitId , pvst.Id as NextVisitStatusTypeId, pvst.Name as NextVisitStatusName
  from Patient p
  inner join PatientVisit pv on p.Id = pv.PatientId
  inner join NextVisitLookup nvl on nvl.VisitId = pv.VisitId and pv.PatientId = p.Id
  left join PatientVisit ppv on ppv.VisitId = nvl.NextVisitId and ppv.PatientId = p.Id
  left join #patientVisitStatusType pvst on pvst.Id = ppv.PatientVisitStatusTypeId
)

	select p.Id as PatientId, vq.QuestionnaireId as QuestionnaireId, vq.VisitId
	into #MissedDiaries
	from Patient p
	inner join PatientVisit pv on p.Id = pv.PatientId
	left join NextVisitStatus nv on nv.VisitId = pv.VisitId and nv.PatientId = p.Id
	inner join #visitQuestionnaire vq on vq.VisitId = pv.VisitId
	left join DiaryEntry de on de.PatientId = p.Id and de.VisitId = pv.VisitId and vq.QuestionnaireId = de.QuestionnaireId
	where de.Id is null and nv.NextVisitStatusTypeId <> 1;

	with questionnaireNames
	as
	(
		select distinct(q.InternalName)
		from #visitQuestionnaire vq
		inner join #questionnaire q on vq.QuestionnaireID = q.Id
		inner join #visitComplianceReportQuestionnaireType vcqt on vcqt.VisitId = vq.VisitId and vcqt.QuestionnaireTypeId = q.QuestionnaireTypeId
	)

	select @questionnaireNames = stuff((select ',' + '[' + InternalName + ']'
	from questionnaireNames
	for XML Path('')), 1, 1, '');

	IF OBJECT_ID('tempdb..#PatientVisitCount') IS NOT NULL DROP TABLE #PatientVisitCount

	select PatientId, VisitId, count(*) as Cnt
	into #PatientVisitCount
	from(
	select
				  distinct p.Id as PatientId
				, s.Id as SiteId
				,q.id as Id
        , vcr.VisitId
			from
				Patient p
				inner join Site s on p.SiteId = s.Id
				left join PatientVisit pv on p.Id = pv.PatientId
				inner join #visitQuestionnaire vq on pv.VisitId = vq.Visitid
				inner join #visit v on vq.VisitId = v.Id
				inner join #questionnaire q on vq.QuestionnaireId = q.Id
				inner join(select q.id, vq.VisitId
							from #visitQuestionnaire vq
							inner join #questionnaire q on vq.QuestionnaireID = q.Id
							inner join #visitComplianceReportQuestionnaireType vcqt on vcqt.VisitId = vq.VisitId and vcqt.QuestionnaireTypeId = q.QuestionnaireTypeId
						   ) vcr on vcr.Id = q.Id and v.Id = vcr.VisitId
				left join DiaryEntry de on de.QuestionnaireId = q.Id and de.PatientId = p.Id and de.VisitId = v.Id
	group by s.id, p.id , q.Id, vcr.VisitId
	) as visits
	group by patientid, VisitId


	IF OBJECT_ID('tempdb..#PatientVisitCompleted') IS NOT NULL DROP TABLE #PatientVisitCompleted

	select patientid,visitid,count(*) as Total
	into #PatientVisitCompleted
	from (
	select

				p.Id as PatientId
				, s.Id as SiteId
				, s.SiteNumber
				, p.PatientNumber
				, v.Name as VisitName
				, v.id as VisitId
				, pv.VisitDate
				, q.InternalName as QuestionnaireName
				, q.id
				, de.CompletedTime
				, case when de.CompletedTime is not null then 'Completed' else 'N/A' end as VisitCompliance
			from
				Patient p
				inner join Site s on p.SiteId = s.Id
				left join PatientVisit pv on p.Id = pv.PatientId
				inner join #visitQuestionnaire vq on pv.VisitId = vq.Visitid
				inner join #visit v on vq.VisitId = v.Id
				inner join #questionnaire q on vq.QuestionnaireId = q.Id
				inner join (select q.id, vq.VisitId
							from #visitQuestionnaire vq
							inner join #questionnaire q on vq.QuestionnaireID = q.Id	
						   ) vcr on vcr.Id = q.Id and vcr.visitId = v.Id
				left join DiaryEntry de on de.QuestionnaireId = q.Id and de.PatientId = p.Id and de.VisitId = v.Id
	) as r
	where VisitCompliance = 'Completed'
	group by patientid,visitid

	declare @sql nvarchar(max)='

	declare @siteCompliance as table
	(
	Id uniqueidentifier,
	Name varchar(200),
	TotalCompleted int,
	TotalExpected int,
	SiteComplianceRate float
	)

	insert into @siteCompliance
		select  s.Id as SiteId
		,s.Name

			, count(d.CompletedTime) as TotalCompleted
			, count(v.Name) as TotalExpected
			, cast(cast(count(d.CompletedTime) as float) / cast(count(v.Name) as float) * 100 as decimal(5,2)) as CompletionRate
	from Patient p
	join Site s on p.SiteId = s.Id
	join PatientVisit pv on p.Id = pv.PatientId
	join #visit v on pv.VisitId = v.Id
	join #visitQuestionnaire vq on v.Id = vq.VisitId
	join #questionnaire q on vq.QuestionnaireId = q.Id
	join #patientStatusType pst on pst.Id = p.PatientStatusTypeId
	inner join #ComplianceQuestionnaires cq on cq.Id = q.Id and vq.VisitId = cq.VisitId
	left join DiaryEntry d on q.Id = d.QuestionnaireId
		and q.Id = d.QuestionnaireId
		and v.Id = d.VisitId
		and p.Id = d.PatientId
	where
		pst.IsRemoved = 0
	group by s.Id, s.name
	order by s.Id

	select
	*
	from
	(
		select
			p.Id as PatientId
			, s.Id as SiteId
			, s.SiteNumber
			, p.PatientNumber
			, v.Name as VisitName
			, pv.VisitDate
			, q.InternalName as QuestionnaireName
			, case when md.PatientId is not null then ''Missed'' when de.CompletedTime is not null then ''Completed'' else ''N/A'' end as VisitComplianceAnswer
			, VisitCompliance = case when pcomp.Total > 0 then convert(varchar(10),cast(cast(pcomp.Total as float) / cast(pvc.cnt as float) * 100 as decimal(5,2))) + ''%''   else ''0%''	end
			, SiteComplianceRate =  case when sc.SiteComplianceRate > 0 then convert(varchar(10),convert(decimal(5,2), sc.SiteComplianceRate)) + ''%'' else ''0%'' end
			, DateOfDeactivation = (	select top 1 ChangeDate
										from SiteActiveHistory sa
										where sa.SiteId = s.Id and
											(Previous = 1 and [Current] = 0)
										order by ChangeDate desc
									)
			, DateOfReactivation = (	select top 1 ChangeDate
										from SiteActiveHistory sa
										where sa.SiteId = s.Id and
											(Previous = 0 and [Current] = 1)
										and ChangeDate > (select min(ChangeDate)  from SiteActiveHistory sa1 where s.Id = sa1.SiteId and Previous = 0 and [Current] = 1)
										order by ChangeDate desc
									)
		from
			Patient p
			inner join Site s on p.SiteId = s.Id
			left join PatientVisit pv on p.Id = pv.PatientId
			inner join #visitQuestionnaire vq on pv.VisitId = vq.Visitid
			inner join #visit v on vq.VisitId = v.Id
			inner join #questionnaire q on vq.QuestionnaireId = q.Id
			inner join #patientStatusType statusType on p.PatientStatusTypeId=statusType.Id
			left join DiaryEntry de on de.QuestionnaireId = q.Id and de.PatientId = p.Id and de.VisitId = v.Id
			left join @siteCompliance sc on s.Id = sc.Id
			left join #PatientVisitCount pvc on p.Id = pvc.PatientId and v.Id = pvc.VisitId
			left join #PatientVisitCompleted pcomp on p.Id = pcomp.PatientId and v.Id = pcomp.VisitId
			left join #MissedDiaries md on md.PatientId = p.Id and md.QuestionnaireId = q.Id and md.VisitId = v.Id
		where 
			statusType.IsRemoved <> 1
														 ) x
	pivot(
		Max(VisitComplianceAnswer)
		for QuestionnaireName in (' + @questionnaireNames + ')
	) piv
	order by patientId, VisitName
	'
	exec sp_executesql @sql

	IF OBJECT_ID('tempdb..#PatientVisitCount') IS NOT NULL DROP TABLE #PatientVisitCount
	IF OBJECT_ID('tempdb..#PatientVisitCompleted') IS NOT NULL DROP TABLE #PatientVisitCompleted
	IF OBJECT_ID('tempdb..#MissedDiaries') IS NOT NULL DROP TABLE #MissedDiaries
	IF OBJECT_ID('tempdb..#ComplianceQuestionnaires') IS NOT NULL DROP TABLE #ComplianceQuestionnaires
	IF OBJECT_ID('tempdb..#questionnaire') IS NOT NULL DROP TABLE #questionnaire
	IF OBJECT_ID('tempdb..#visitQuestionnaire') IS NOT NULL DROP TABLE #visitQuestionnaire
	IF OBJECT_ID('tempdb..#patientVisitStatusType') IS NOT NULL DROP TABLE #patientVisitStatusType
	IF OBJECT_ID('tempdb..#patientStatusType') IS NOT NULL DROP TABLE #patientStatusType
	IF OBJECT_ID('tempdb..#visit') IS NOT NULL DROP TABLE #visit
	IF OBJECT_ID('tempdb..#visitComplianceReportQuestionnaireType') IS NOT NULL DROP TABLE #visitComplianceReportQuestionnaireType
END
GO

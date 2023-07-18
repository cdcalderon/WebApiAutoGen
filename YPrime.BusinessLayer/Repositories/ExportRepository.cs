using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Reports.Models;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Repositories
{
    public class ExportRepository : BaseRepository, IExportRepository
    {
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IVisitService _visitService;
        private readonly ISoftwareReleaseRepository _softwareReleaseRepository;

        public ExportRepository(IStudyDbContext db, IQuestionnaireService questionnaireService, IVisitService visitService, ISoftwareReleaseRepository softwareReleaseRepository) : base(db)
        {
            _questionnaireService = questionnaireService;
            _visitService = visitService;
            _softwareReleaseRepository = softwareReleaseRepository;
        }

        public void CreateExport(ExportDto exportDto)
        {
            var exportEntity = new Export
            {
                Id = Guid.NewGuid(),
                Name = exportDto.Name,
                UserId = exportDto.UserId,
                SiteId = exportDto.SiteId,
                PatientId = exportDto.PatientId,
                DiaryStartDate = exportDto.DiaryStartDate.HasValue
                    ? new DateTimeOffset(exportDto.DiaryStartDate.Value.Date, TimeSpan.Zero)
                    : (DateTimeOffset?) null,
                DiaryEndDate = exportDto.DiaryEndDate.HasValue
                    ? new DateTimeOffset(exportDto.DiaryEndDate.Value.Date.AddDays(1).AddTicks(-1), TimeSpan.Zero)
                    : (DateTimeOffset?) null,
                CreatedTime = DateTimeOffset.Now,
                ExportStatusId = 1 // In Queue
            };
            exportDto.Id = exportEntity.Id;
            _db.Exports.Add(exportEntity);
            _db.SaveChanges(exportDto.UserId.ToString());
        }

        public async Task<List<ExportStream>> ExecuteExport(Guid exportId)
        {
            var configId = _softwareReleaseRepository.GetLatestGlobalConfigurationVersionId();

            //Set startedTime
            var export = _db.Exports.SingleOrDefault(e => e.Id == exportId);
            export.StartedTime = DateTimeOffset.Now;

            // Get config mongo data
            var questionnaires = await _questionnaireService.GetAllInflatedQuestionnaires(null, configId);
            var questionnaireStubs = questionnaires.Select(q => new QuestionnaireStub
            {
                Id = q.Id,
                Name = q.DisplayName,
                InternalName = q.InternalName
            });
            var questionnaireJson = JsonConvert.SerializeObject(questionnaireStubs);

            var questions = questionnaires.SelectMany(q => q.Pages.SelectMany(p => p.Questions)).ToList();
            var questionStubs = questions.Select(q => new QuestionStub
            {
                Id = q.Id,
                Name = q.QuestionText,
                QuestionnaireId = q.QuestionnaireId,
                Sequence = q.Sequence,
                InputFieldTypeId = q.InputFieldTypeId,
                ExportDisplayName = q.DisplayName,
                ExportDisplayOrder = q.QuestionSettings.ExportDisplayOrder
            });
            var questionJson = JsonConvert.SerializeObject(questionStubs);

            var choiceStubs = questions.SelectMany(q => q.Choices.Select(c => new ChoiceStub
            {
                Id = c.Id,
                Name = c.DisplayText,
                QuestionId = q.Id
            })).ToList();
            var choiceJson = JsonConvert.SerializeObject(choiceStubs);

            var visits = await _visitService.GetAll(configId);
            var visitStubs = visits.Select(v => new VisitStub
            {
                Id = v.Id,
                Name = v.Name,
                IsScheduled = v.IsScheduled,
                VisitOrder = v.VisitOrder
            });
            var visitJson = JsonConvert.SerializeObject(visitStubs);

            //Execute export
            var exports = new List<ExportStream>();
            using (var command = _db.Database.Connection.CreateCommand())
            {
                if (_db.Database.Connection.State != ConnectionState.Open)
                {
                    _db.Database.Connection.Open();
                }

                command.CommandText = "ExecuteExportRequest";
                command.Parameters.Add(new SqlParameter("ExportId", exportId));
                command.Parameters.Add(new SqlParameter("QuestionnaireJson", questionnaireJson));
                command.Parameters.Add(new SqlParameter("QuestionJson", questionJson));
                command.Parameters.Add(new SqlParameter("ChoiceJson", choiceJson));
                command.Parameters.Add(new SqlParameter("VisitJson", visitJson));
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                var reader = await command.ExecuteReaderAsync();
                exports = ToExportFiles(reader, true, Encoding.UTF8, "|", "csv");
                //_db.Database.Connection.Close();
            }

            //Set completedTime & return exports
            export.CompletedTime = DateTimeOffset.Now;
            _db.Exports.AddOrUpdate(export);
            _db.SaveChanges(export.UserId.ToString());
            return exports;
        }

        public async Task<ExportDto> GetExport(Guid Id)
        {
            var export = _db.Exports.Find(Id);
            var exportDto = new ExportDto
            {
                Id = export.Id,
                Name = export.Name,
                UserId = export.UserId,
                SiteId = export.SiteId,
                SiteName = export.Site?.Name,
                PatientId = export.PatientId,
                PatientNumber = export.Patient?.PatientNumber,
                ExportStatusId = export.ExportStatusId,
                ExportStatus = export.ExportStatus?.Name,
                DiaryStartDate = export.DiaryStartDate,
                DiaryEndDate = export.DiaryEndDate,
                ScheduledStartTime = export.ScheduledStartTime,
                CreatedTime = export.CreatedTime,
                StartedTime = export.StartedTime,
                CompletedTime = export.CompletedTime
            };
            return exportDto;
        }

        public async Task<IEnumerable<ExportDto>> GetExports(Guid userId)
        {
            var results = _db.Exports
                .Where(e => e.UserId == userId)
                .Select(e => new ExportDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    UserId = e.UserId,
                    SiteId = e.SiteId,
                    SiteName = e.Site.Name ?? string.Empty,
                    PatientId = e.PatientId,
                    PatientNumber = e.Patient.PatientNumber ?? string.Empty,
                    ExportStatusId = e.ExportStatusId,
                    ExportStatus = e.ExportStatus.Name,
                    DiaryStartDate = e.DiaryStartDate,
                    DiaryStartDateGridData = e.DiaryStartDate != null
                        ? SqlFunctions.DatePart("DAY", e.DiaryStartDate) + "-" +
                          SqlFunctions.DateName("MONTH", e.DiaryStartDate).ToString() + "-" +
                          SqlFunctions.DatePart("YEAR", e.DiaryStartDate)
                        : string.Empty,
                    DiaryEndDate = e.DiaryEndDate,
                    DiaryEndDateGridData = e.DiaryEndDate != null
                        ? SqlFunctions.DatePart("DAY", e.DiaryEndDate) + "-" +
                          SqlFunctions.DateName("MONTH", e.DiaryEndDate).ToString() + "-" +
                          SqlFunctions.DatePart("YEAR", e.DiaryEndDate)
                        : string.Empty,
                    ScheduledStartTime = e.ScheduledStartTime,
                    CreatedTime = e.CreatedTime,
                    CreatedTimeGridData = e.CreatedTime != null
                        ? SqlFunctions.DatePart("DAY", e.CreatedTime) + "-" +
                          SqlFunctions.DateName("MONTH", e.CreatedTime).ToString() + "-" +
                          SqlFunctions.DatePart("YEAR", e.CreatedTime)
                        : null,
                    StartedTime = e.StartedTime,
                    StartedTimeGridData = e.StartedTime != null
                        ? SqlFunctions.DatePart("DAY", e.StartedTime) + "-" +
                          SqlFunctions.DateName("MONTH", e.StartedTime).ToString() + "-" +
                          SqlFunctions.DatePart("YEAR", e.StartedTime)
                        : null,
                    CompletedTime = e.CompletedTime,
                    CompletedTimeGridData = e.CompletedTime != null
                        ? SqlFunctions.DatePart("DAY", e.CompletedTime) + "-" +
                          SqlFunctions.DateName("MONTH", e.CompletedTime).ToString() + "-" +
                          SqlFunctions.DatePart("YEAR", e.CompletedTime)
                        : null
                })
                .OrderBy(e => e.CompletedTime)
                .ToList();

            return results;
        }

        public List<ExportStream> ToExportFiles(DbDataReader dataReader, bool includeHeaderAsFirstRow,
            Encoding encoding, string delimeter, string extension)
        {
            List<ExportStream> exportFiles = new List<ExportStream>();
            StringBuilder sb = null;
            while (dataReader.HasRows)
            {
                string fileName = null;
                int fileNameIdx = dataReader.GetOrdinal("QuestionnaireName");
                List<string> rows = new List<string>();
                if (includeHeaderAsFirstRow)
                {
                    sb = new StringBuilder();
                    for (int index = 0; index < dataReader.FieldCount; index++)
                    {
                        if (dataReader.GetName(index) != null)
                        {
                            sb.Append(dataReader.GetName(index));
                        }

                        if (index < dataReader.FieldCount - 1)
                        {
                            sb.Append(delimeter);
                        }
                    }

                    rows.Add(sb.ToString());
                }

                while (dataReader.Read())
                {
                    if (fileName == null)
                    {
                        fileName = dataReader.GetValue(fileNameIdx).ToString();
                    }

                    sb = new StringBuilder();
                    for (int index = 0; index < dataReader.FieldCount - 1; index++)
                    {
                        if (!dataReader.IsDBNull(index))
                        {
                            string value = dataReader.GetValue(index).ToString();
                            if (dataReader.GetFieldType(index) == typeof(string))
                            {
                                //If double quotes are used in value, ensure each are replaced but 2.
                                if (value.IndexOf("\"") >= 0)
                                    value = value.Replace("\"", "\"\"");

                                //If separtor are is in value, ensure it is put in double quotes.
                                if (value.IndexOf(delimeter) >= 0)
                                    value = "\"" + value + "\"";
                            }

                            sb.Append(value);
                        }

                        if (index < dataReader.FieldCount - 1)
                            sb.Append(delimeter);
                    }

                    if (!dataReader.IsDBNull(dataReader.FieldCount - 1))
                        sb.Append(dataReader.GetValue(dataReader.FieldCount - 1).ToString().Replace(delimeter, " "));

                    rows.Add(sb.ToString());
                }

                exportFiles.Add(new ExportStream
                {
                    Name = fileName,
                    MemoryStream =
                        new MemoryStream(rows.SelectMany(c => encoding.GetBytes(c + Environment.NewLine)).ToArray()),
                    Extension = extension
                });
                dataReader.NextResult();
            }

            dataReader.Close();
            sb = null;
            return exportFiles;
        }

        public bool ExportExists(string name)
        {
            return _db.Exports.Any(e => e.Name.ToLower() == name.ToLower());
        }
    }
}
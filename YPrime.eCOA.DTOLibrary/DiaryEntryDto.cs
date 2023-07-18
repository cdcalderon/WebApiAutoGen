using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class DiaryEntryDto : DtoBase
    {
        public DiaryEntryDto()
        {
            CurrentPageNumber = 1;
            DiaryPages = new List<DiaryPageModel>();
            Answers = new List<AnswerDto>();
            QuestionAnswers = new List<QuestionAnswerDto>();
            DiaryPageHistory = new List<Guid?>();
            CorrectionApprovalDataDtos = new List<CorrectionApprovalDataDto>();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid ConfigurationId { get; set; }
        public int DiaryStatusId { get; set; }

        [DisplayName("Diary Status")] public string DiaryStatusName { get; set; }

        public int DataSourceId { get; set; }
        [DisplayName("Data Source Name")] public string DataSourceName { get; set; }
        public Guid QuestionnaireId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientNumber { get; set; }
        public Guid SiteId { get; set; }
        public string QuestionnaireName { get; set; }
        [DisplayName("Questionnaire Display Name")] public string QuestionnaireDisplayName { get; set; }
        public Guid? VisitId { get; set; }
        [DisplayName("Visit Name")] public string VisitName { get; set; }
        public Guid? DeviceId { get; set; }
        [DisplayName("Diary Date")] public DateTime DiaryDate { get; set; }
        [DisplayName("Started Time")] public DateTimeOffset StartedTime { get; set; }
        public string Version { get; set; }
        [DisplayName("Completed Time")] public DateTimeOffset CompletedTime { get; set; }
        [DisplayName("Transmitted Time")] public DateTimeOffset TransmittedTime { get; set; }

        public List<DiaryPageModel> DiaryPages { get; set; }

        public List<AnswerDto> Answers { get; set; }
        public List<QuestionAnswerDto> QuestionAnswers { get; set; }

        public SiteDto Site { get; set; }

        public string DiaryEntryDisplay { get; set; }

        public string DiaryEntryDateDisplay { get; set; }

        public Guid? ReviewedByUserid { get; set; }
        public string ReviewedByUserName { get; set; }
        public DateTimeOffset? ReviewedDate { get; set; }
        public int CurrentPageNumber { get; set; }
        public Guid? CurrentDiaryPageId { get; set; }

        public List<Guid?> DiaryPageHistory { get; set; }

        public string Error { get; set; }

        [DisplayName("Asset Tag")] public string AssetTag { get; set; }


        public List<CorrectionApprovalDataDto> CorrectionApprovalDataDtos { get; set; }
        
        public int? QuestionnaireTypeId { get; set; }

        public List<CustomExtensionModel> CustomExtensions { get; set; }

        public bool IsCSSRS => bool.TryParse(CustomExtensions?.FirstOrDefault(ce => ce.Name == nameof(IsCSSRS))?.Value, out var parsedBool) ? parsedBool : false;
       
    }
}
using System;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Extensions
{
    public static class DiaryEntryExtensions
    {
        private const string DefaultDiaryDateFormat = "mm/ddd/yyyy";

        public static DiaryStatus GetDiaryStatus(this DiaryEntry entity)
        {
            var enumType = DiaryStatus
                .FirstOrDefault<DiaryStatus>(et => et.Id == entity.DiaryStatusId);
            return enumType;
        }

        public static DataSource GetDataSource(this DiaryEntry entity)
        {
            var enumType = DataSource
                .FirstOrDefault<DataSource>(et => et.Id == entity.DataSourceId);
            return enumType;
        }

        public static DiaryEntryDto ToDto(
            this DiaryEntry diaryEntry,
            QuestionnaireModel questionnaire = null,
            VisitModel visit = null,
            string diaryDateFormat = null)
        {
            diaryDateFormat = diaryDateFormat ?? DefaultDiaryDateFormat;

            var questionnaireName = string.IsNullOrWhiteSpace(questionnaire?.DisplayName)
                ? questionnaire?.InternalName
                : questionnaire?.DisplayName;

            var dto = new DiaryEntryDto
            {
                Id = diaryEntry.Id,
                ConfigurationId = diaryEntry.ConfigurationId,
                DiaryStatusId = diaryEntry.DiaryStatusId,
                DiaryStatusName = diaryEntry.GetDiaryStatus().Name,
                DiaryDate = diaryEntry.DiaryDate,
                DataSourceId = diaryEntry.DataSourceId,
                DataSourceName = diaryEntry.GetDataSource().Name,
                PatientId = diaryEntry.PatientId,
                QuestionnaireTypeId = questionnaire?.QuestionnaireTypeId,
                QuestionnaireId = diaryEntry.QuestionnaireId,
                QuestionnaireName = questionnaire?.InternalName ?? string.Empty,
                QuestionnaireDisplayName = questionnaireName ?? string.Empty,
                VisitId = diaryEntry.VisitId,
                VisitName = visit?.Name,
                DeviceId = diaryEntry.DeviceId,
                StartedTime = diaryEntry.StartedTime,
                CompletedTime = diaryEntry.CompletedTime,
                TransmittedTime = diaryEntry.TransmittedTime,
                SiteId = diaryEntry.Patient?.SiteId ?? Guid.Empty,
                ReviewedByUserid = diaryEntry.ReviewedByUserid,
                ReviewedByUserName = diaryEntry.ReviewedByUser?.UserName ?? string.Empty,
                ReviewedDate = diaryEntry.ReviewedDate,
                AssetTag = diaryEntry.Device?.AssetTag ?? string.Empty,
                PatientNumber = diaryEntry.Patient?.PatientNumber,
                DiaryEntryDisplay = $"{questionnaire?.InternalName} {diaryEntry.DiaryDate.ToString(diaryDateFormat)}",
                CustomExtensions = questionnaire?.CustomExtensions
            };

            return dto;
        }
    }
}
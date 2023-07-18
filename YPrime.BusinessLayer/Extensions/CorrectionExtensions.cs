using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Utils;

namespace YPrime.BusinessLayer.Extensions
{
    public static class CorrectionExtensions
    {
        public static bool ShouldShowPreviousValues(this Correction correction)
        {
            var typeId = correction?.CorrectionTypeId;
            return typeId != CorrectionType.PaperDiaryEntry.Id;
        }

        public static bool IsMultiSelectAnswer(this Correction correction, CorrectionApprovalData correctionApproval)
        {
            var isMultiSelect = false;
            if (correctionApproval.TableName == nameof(Answer))
            {
                var questionId = correctionApproval?.CorrectionApprovalDataAdditionals.FirstOrDefault(a => a.ColumnName == nameof(Answer.QuestionId)).ColumnValue;
                var approvalDataAdditional = correction.CorrectionApprovalDatas.Where(cad => cad.TableName == nameof(Answer)
                && cad.CorrectionApprovalDataAdditionals != null).SelectMany(c => c.CorrectionApprovalDataAdditionals);

                isMultiSelect = approvalDataAdditional.Count(c => c.ColumnValue == questionId) > 1;
            }
            return isMultiSelect;
        }

        public static bool MultiSelectAnswerUpdated(this Correction correction, CorrectionApprovalData correctionApproval)
        {
            var multiSelectAnswerUpdated = false;
            if (correctionApproval.TableName == nameof(Answer))
            {
                var questionId = correctionApproval?.CorrectionApprovalDataAdditionals.FirstOrDefault(a => a.ColumnName == nameof(Answer.QuestionId)).ColumnValue;
                var approvalData = correction.CorrectionApprovalDatas.Where(cad => cad.TableName == nameof(Answer)
                && cad.CorrectionApprovalDataAdditionals != null && cad.CorrectionApprovalDataAdditionals.Any(a => a.ColumnValue == questionId));

                foreach (var data in approvalData)
                {
                    if ((data.NewDataPoint == null && data.RemoveItem) || data.NewDataPoint != null)
                    {
                        multiSelectAnswerUpdated = true;
                        break;
                    }
                }

            }
            return multiSelectAnswerUpdated;
        }

        public static string ShowMultiSelectValues(this Correction correction, CorrectionApprovalData correctionApproval, bool oldValues = false)
        {
            var questionId = correctionApproval?.CorrectionApprovalDataAdditionals.FirstOrDefault(a => a.ColumnName == nameof(Answer.QuestionId)).ColumnValue;
            var displayValues = correction.CorrectionApprovalDatas.Where(cad => cad.CorrectionApprovalDataAdditionals != null &&
                cad.CorrectionApprovalDataAdditionals.Any(da => da.ColumnValue == questionId))
                .Select(d => oldValues ? d.OldDisplayValue : d.NewDisplayValue);

            return displayValues.Any() ? string.Join(", ", displayValues.Where(d => !String.IsNullOrEmpty(d))) : "";
        }

        public static DiaryEntry ToDiaryEntry(this Correction correction)
        {
            var diaryEntry = new DiaryEntry
            {
                Id = Guid.NewGuid(),
                PatientId = correction.PatientId.Value,
                DiaryStatusId = DiaryStatus.Modified.Id,
                DataSourceId = DataSource.Paper.Id,
                TransmittedTime = DateTimeOffset.Now,
                StartedTime = DateTimeOffset.Now.Date,
                CompletedTime = DateTimeOffset.Now.Date,
                UserId = correction.StartedByUserId,
                SyncVersion = 1,
                ConfigurationId = correction.ConfigurationId
            };

            InflateDiaryEntryCorrectionApprovalData(correction, diaryEntry);

            return diaryEntry;
        }

        public static List<PatientAttribute> ToPatientAttribute(
            this Correction correction,
            List<PatientAttribute> existingAttributes)
        {
            var patientAttributes = new List<PatientAttribute>();

            InflatePatientAttributeCorrectionApprovalData(
                correction, 
                patientAttributes,
                existingAttributes);

            return patientAttributes;
        }

        public static void UpdatePatientData(this Correction correction, Patient patient)
        {
            foreach (var correctionApprovalData in correction.CorrectionApprovalDatas)
            {
                if (correctionApprovalData.TableName == nameof(Patient))
                {
                    switch (correctionApprovalData.ColumnName)
                    {
                        case nameof(Patient.PatientNumber):
                            {
                                patient.PatientNumber = correctionApprovalData.NewDataPoint;
                                break;
                            }
                        case nameof(Patient.PatientStatusTypeId):
                            {
                                if (int.TryParse(correctionApprovalData.NewDataPoint, out var patientStatusId))
                                {
                                    patient.PatientStatusTypeId = patientStatusId;
                                }
                                break;
                            }
                        case nameof(Patient.EnrolledDate):
                            {
                                if (DateTimeOffset.TryParse(correctionApprovalData.NewDataPoint, out var parsedEnrollDate))
                                {
                                    patient.EnrolledDate = parsedEnrollDate;
                                }
                                break;
                            }
                        case nameof(Patient.LanguageId):
                            {
                                if (Guid.TryParse(correctionApprovalData.NewDataPoint, out var parsedLanguageId))
                                {
                                    patient.LanguageId = parsedLanguageId;
                                }
                                break;
                            }
                    }
                }
            }
        }

        private static void InflateDiaryEntryCorrectionApprovalData(Correction correction, DiaryEntry diaryEntry)
        {
            foreach (var correctionApprovalData in correction.CorrectionApprovalDatas.Where(x => x.NewDataPoint != null))
            {
                if (correctionApprovalData.TableName == nameof(DiaryEntry))
                {
                    switch (correctionApprovalData.ColumnName)
                    {
                        case nameof(DiaryEntry.DiaryDate):
                            {
                                if (DateTime.TryParse(correctionApprovalData.NewDataPoint, out var parsedDate))
                                {
                                    diaryEntry.DiaryDate = parsedDate.Date;
                                }

                                break;
                            }
                        case nameof(DiaryEntry.VisitId):
                            {
                                if (Guid.TryParse(correctionApprovalData.NewDataPoint, out var parsedVisitId))
                                {
                                    diaryEntry.VisitId = parsedVisitId;
                                }

                                break;
                            }
                        case nameof(DiaryEntry.QuestionnaireId):
                            {
                                if (Guid.TryParse(correctionApprovalData.NewDataPoint, out var parsedQuestionnaireId))
                                {
                                    diaryEntry.QuestionnaireId = parsedQuestionnaireId;
                                }

                                break;
                            }
                    }
                }
                else if (correctionApprovalData.TableName == nameof(Answer))
                {
                    switch (correctionApprovalData.ColumnName)
                    {
                        case nameof(Answer.FreeTextAnswer):
                            {
                                var answer = CreateNewAnswer(
                                    diaryEntry.Id,
                                    correctionApprovalData.RowId,
                                    correction.ConfigurationId,
                                    correctionApprovalData.NewDataPoint);

                                diaryEntry.Answers.Add(answer);
                                break;
                            }
                        case nameof(Answer.ChoiceId):
                            {
                                var choiceIds = correctionApprovalData.NewDataPoint.Split(',');

                                foreach (var choiceId in choiceIds)
                                {
                                    var answerChoiceId = Guid.Parse(choiceId);

                                    var answer = CreateNewAnswer(
                                        diaryEntry.Id,
                                        correctionApprovalData.RowId,
                                        correction.ConfigurationId,
                                        null,
                                        answerChoiceId);
                                    diaryEntry.Answers.Add(answer);
                                }

                                break;
                            }
                    }
                }
            }
        }

        private static void InflatePatientAttributeCorrectionApprovalData(
            Correction correction,
            List<PatientAttribute> patientAttributes,
            List<PatientAttribute> existingAttributes)
        {
            foreach (var correctionApprovalData in correction.CorrectionApprovalDatas)
            {
                if (correctionApprovalData.TableName == nameof(PatientAttribute))
                {
                    switch (correctionApprovalData.ColumnName)
                    {
                        case nameof(PatientAttribute.AttributeValue):
                            {
                                var isExisting = correctionApprovalData.RowId != Guid.Empty;

                                var syncVersion = isExisting
                                    ? existingAttributes.First(ea => ea.Id == correctionApprovalData.RowId).SyncVersion + 1
                                    : 1;

                                var patientAttribute = new PatientAttribute
                                {
                                    PatientId = correction.PatientId.Value,
                                    SyncVersion = syncVersion,
                                    AttributeValue = correctionApprovalData.NewDataPoint,
                                    Id = isExisting
                                        ? correctionApprovalData.RowId
                                        : Guid.NewGuid(),
                                    PatientAttributeConfigurationDetailId = Guid.Parse(correctionApprovalData.CorrectionApprovalDataAdditionals[0].ColumnValue)
                                };

                                patientAttributes.Add(patientAttribute);

                                break;
                            }
                    }
                }
            }
        }

        private static Answer CreateNewAnswer(
            Guid diaryEntryId,
            Guid questionId,
            Guid configurationId,
            string freeTextAnswer = null,
            Guid? choiceId = null)
        {
            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                DiaryEntryId = diaryEntryId,
                QuestionId = questionId,
                ConfigurationId = configurationId
            };

            if (!string.IsNullOrWhiteSpace(freeTextAnswer))
            {
                answer.FreeTextAnswer = freeTextAnswer;
            }

            if (choiceId.HasValue)
            {
                answer.ChoiceId = choiceId;
            }

            return answer;
        }


        public static CorrectionApprovalDataDto EntityToDto(this CorrectionApprovalData correctionApprovalData)
        {
            var cadDto = new CorrectionApprovalDataDto();
            cadDto.CopyPropertiesFromObject(correctionApprovalData);
            cadDto.DataCorrectionNumber = correctionApprovalData.Correction.DataCorrectionNumber;
            return cadDto;
        }
    }
}
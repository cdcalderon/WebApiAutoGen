using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Utils;
using YPrime.eCOA.DTOLibrary.ViewModel;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Filters;

namespace YPrime.BusinessLayer.Repositories
{
    public class PatientAttributeRepository : BaseRepository, IPatientAttributeRepository
    {
        private const int MaximumAllowedSubjectNumberLength = 9;

        private readonly IPatientRepository _patientRepository;
        private readonly ISubjectInformationService _subjectInformationService;
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly IStudySettingService _studySettingService;
        private readonly ITranslationService _translationService;
        private readonly ISiteRepository _siteRepository;

        public PatientAttributeRepository(
            IStudyDbContext db,
            IPatientRepository patientRepository,
            ISubjectInformationService subjectInformationService,
            IQuestionnaireService questionnaireService,
            IPatientStatusService patientStatusService,
            IStudySettingService studySettingService,
            ITranslationService translationService,
            ISiteRepository siteRepository
        ) : base(db)
        {
            _patientRepository = patientRepository;
            _subjectInformationService = subjectInformationService;
            _questionnaireService = questionnaireService;
            _studySettingService = studySettingService;
            _translationService = translationService;
            _patientStatusService = patientStatusService;
            _siteRepository = siteRepository;
        }

        public async Task<IEnumerable<PatientAttributeDto>> GetPatientAttributes(Guid patientId, string cultureCode)
        {
            var patientAttributes = _db.PatientAttributes
                .Where(x => x.PatientId == patientId);

            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var patientSite = patientFilter.Execute(_db.Patients)
                .First(p => p.Id == patientId).Site;

            var configId = await _patientRepository.GetAssignedConfiguration(patientId);
            var subjectInfoModels = await _subjectInformationService
                .GetAll(configId);

            IList<PatientAttributeDto> dtos = new List<PatientAttributeDto>();

            foreach (var subjectInfo in subjectInfoModels)
            {
                if (!subjectInfo.Countries.Any(c => c.Id == patientSite.CountryId))
                {
                    continue;
                }

                var dto = new PatientAttributeDto();

                var patientAttribute = patientAttributes.FirstOrDefault(p => p.PatientAttributeConfigurationDetailId == subjectInfo.Id);
                if (patientAttribute != null)
                {
                    dto.CopyPropertiesFromObject(patientAttribute);
                }
                else
                {
                    dto.PatientId = patientId;
                    dto.PatientAttributeConfigurationDetailId = subjectInfo.Id;
                    dto.AttributeValue = string.Empty;
                    dto.NewAttributeData = true;
                }

                dto.SubjectInformation = subjectInfo;

                if (dto.SubjectInformation != null && dto.SubjectInformation.Choices.Any())
                {
                    dto.SubjectInformation.Choices = dto.SubjectInformation
                        .Choices
                        .OrderBy(c => c.DisplayOrder)
                        .ToList();

                    ChoiceModel selectedChoice = null;

                    if (Guid.TryParse(dto.AttributeValue, out var parsedGuid))
                    {
                        selectedChoice = dto.SubjectInformation
                            .Choices
                            .FirstOrDefault(c => c.Id == parsedGuid);  
                    }
                    else if (!string.IsNullOrWhiteSpace(dto.AttributeValue))
                    {
                        selectedChoice = dto.SubjectInformation
                            .Choices
                            .FirstOrDefault(c => c.Name == dto.AttributeValue);
                    }

                    dto.DisplayValue = selectedChoice?.Name;
                }
                else
                {
                    dto.DisplayValue = dto.AttributeValue;
                }

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<PatientAttributeQuestion>> GetPatientAttributesForCorrection(
            Guid patientId, 
            Guid correctionId,
            string cultureCode, 
            IList<CorrectionApprovalData> correctionApprovalData)
        {
            var patient = await _patientRepository.GetPatient(patientId, cultureCode);
            var attributes = new List<PatientAttributeQuestion>
            {
                await PatientNumberBuild(patient, correctionApprovalData, correctionId),
                await PatientEnrollmentDateBuild(patient, correctionApprovalData, correctionId),
                await PatientLanguageBuild(patient, correctionApprovalData, correctionId),
                await PatientStatusBuild(patient, correctionApprovalData, correctionId)
            };

            var patientAttributeDtos = await GetPatientAttributes(patientId, cultureCode);
            var patientAttributeQuestions = new List<PatientAttributeQuestion>();

            foreach (var attribute in patientAttributeDtos) 
            {
                if (attribute.SubjectInformation.GetDataType().Id == DataType.DateAttribute.Id)
                { // date range is not enforced for DCF
                    attribute.SubjectInformation.Min = null;
                    attribute.SubjectInformation.Max = null;
                }

                var correction = FindMatchingCorrection(correctionApprovalData, attribute);
                var patientAttributeQuestion = new PatientAttributeQuestion
                {
                    PatientAttribute = attribute,
                    PatientId = patientId,
                    SubjectInformation = attribute.SubjectInformation,
                    CorrectionId = correctionId,
                    NewDataPoint = correction?.NewDataPoint,
                    NewDisplayValue = correction?.NewDisplayValue
                };

                patientAttributeQuestions.Add(patientAttributeQuestion);
            }

            attributes
                .AddRange(patientAttributeQuestions
                .OrderBy(x => x.PatientAttribute.SubjectInformation.Sequence));

            return attributes;
        }

        public async Task<string> ExtractPatientNumber(string patientId)
        {
            var patientNumberLength = int.Parse(await _studySettingService.GetStringValue("PatientNumberLength"));
            var patientNumberSeperator = await _studySettingService.GetStringValue("PatientNumberSiteSubjectNumberSeparator") ??
                                         string.Empty;

            patientNumberLength = patientNumberLength > MaximumAllowedSubjectNumberLength
                ? MaximumAllowedSubjectNumberLength
                : patientNumberLength;

            var beginningIndex = patientId.LastIndexOf(patientNumberSeperator);

            if (patientNumberSeperator == string.Empty || beginningIndex == -1)
            {
                beginningIndex = patientId.Length - patientNumberLength;
            }
            else
            {
                beginningIndex++;
            }

            var result = patientId.Substring(beginningIndex);

            return result;
        }

        private CorrectionApprovalData FindMatchingCorrection(IList<CorrectionApprovalData> correctionData, PatientAttributeDto attribute)
        {
            CorrectionApprovalData correction;
            if (attribute.NewAttributeData)
            {
                correction = correctionData.FirstOrDefault(c => c.CorrectionApprovalDataAdditionals
                    .Any(cad => cad.ColumnValue == attribute.PatientAttributeConfigurationDetailId.ToString()));
            }
            else
            {
                correction = correctionData.FirstOrDefault(c => c.RowId == attribute.Id);
            }

            return correction;
        }

        private async Task<PatientAttributeQuestion> PatientNumberBuild(PatientDto patient, IList<CorrectionApprovalData> correctionApprovalData, Guid correctionId)
        {           
            var matchingCorrectionData = correctionApprovalData
                .FirstOrDefault(q => q.ColumnName == CorrectionConstants.PatientNumberColumnName);

            var extractedPatientNumber = await ExtractPatientNumber(patient.PatientNumber);

            return new PatientAttributeQuestion
            {
                PatientId = patient.Id,
                SubjectInformation = null,
                CorrectionId = correctionId,
                NewDataPoint = matchingCorrectionData?.NewDataPoint,
                NewDisplayValue = matchingCorrectionData?.NewDisplayValue,
                ColumnName = nameof(Patient.PatientNumber),
                PatientAttribute = new PatientAttributeDto
                {
                    AttributeValue = extractedPatientNumber,
                    Id = Guid.Empty,
                    SubjectInformation = await BuildPatientNumberDetail(),
                    PatientAttributeConfigurationDetailId = Guid.Empty,
                    PatientId = patient.Id,
                    DisplayValue = extractedPatientNumber
                }
            };
        }

        private async Task<PatientAttributeQuestion> PatientEnrollmentDateBuild(
            PatientDto patient,
            IList<CorrectionApprovalData> correctionApprovalData,
            Guid correctionId)
        {
            var matchingCorrectionData = correctionApprovalData
                .FirstOrDefault(c => c.ColumnName == CorrectionConstants.PatientEnrolledDateColumnName);

            var enrollDateValue = patient.EnrolledDate?.ToString(CorrectionConstants.EnrolledDateFormat) ?? string.Empty;

            var attributeQuestion = new PatientAttributeQuestion
            {
                PatientId = patient.Id,
                SubjectInformation = null,
                CorrectionId = correctionId,
                NewDataPoint = matchingCorrectionData?.NewDataPoint,
                NewDisplayValue = matchingCorrectionData?.NewDisplayValue,
                ColumnName = nameof(Patient.EnrolledDate),
                PatientAttribute = new PatientAttributeDto
                {
                    AttributeValue = enrollDateValue,
                    Id = Guid.Empty,
                    SubjectInformation = await BuildPatientEnrolledDateDetail(patient),
                    PatientAttributeConfigurationDetailId = Guid.Empty,
                    PatientId = patient.Id,
                    DisplayValue = enrollDateValue
                }
            };

            return attributeQuestion;
        }

        private async Task<PatientAttributeQuestion> PatientLanguageBuild(
            PatientDto patient,
            IList<CorrectionApprovalData> correctionApprovalData,
            Guid correctionId)
        {
            var matchingCorrectionData = correctionApprovalData
                .FirstOrDefault(c => c.ColumnName == CorrectionConstants.PatientLanguageColumnName);

            var siteLanguages = await _siteRepository.GetLanguagesForSite(
                patient.SiteId,
                patient.ConfigurationId);

            var selectedLanguage = siteLanguages
                .FirstOrDefault(l => l.Id == patient.LanguageId);

            var attributeQuestion = new PatientAttributeQuestion
            {
                PatientId = patient.Id,
                SubjectInformation = null,
                CorrectionId = correctionId,
                NewDataPoint = matchingCorrectionData?.NewDataPoint,
                NewDisplayValue = matchingCorrectionData?.NewDisplayValue,
                ColumnName = nameof(Patient.LanguageId),
                PatientAttribute = new PatientAttributeDto
                {
                    AttributeValue = selectedLanguage?.Id.ToString() ?? string.Empty,
                    Id = Guid.Empty,
                    SubjectInformation = await BuildPatientLanguageDetail(selectedLanguage, siteLanguages),
                    PatientAttributeConfigurationDetailId = Guid.Empty,
                    PatientId = patient.Id,
                    DisplayValue = selectedLanguage?.Name
                }
            };

            return attributeQuestion;
        }

        private async Task<PatientAttributeQuestion> PatientStatusBuild(PatientDto patient, IList<CorrectionApprovalData> correctionApprovalData, Guid correctionId)
        {
            var matchingCorrectionData = correctionApprovalData
                .FirstOrDefault(q => q.ColumnName == CorrectionConstants.PatientStatusColumnName);

            return new PatientAttributeQuestion
            {
                PatientId = patient.Id,
                SubjectInformation = null,
                CorrectionId = correctionId,
                NewDataPoint = matchingCorrectionData?.NewDataPoint,
                NewDisplayValue = matchingCorrectionData?.NewDisplayValue,
                ColumnName = nameof(Patient.PatientStatusTypeId),
                PatientAttribute = new PatientAttributeDto
                {
                    AttributeValue = patient.PatientStatus,
                    Id = Guid.Empty,
                    PatientAttributeConfigurationDetailId = Guid.Empty,
                    PatientId = patient.Id,
                    SubjectInformation = await BuildStatusDetails(),
                    DisplayValue = patient.PatientStatus
                }
            };
        }

        private async Task<SubjectInformationModel> BuildPatientNumberDetail()
        {
            var subjectNumberTranslation = await _translationService.GetByKey(CorrectionConstants.SubjectNumberTranslationKey);

            int maxSubjectNumberLength = int.Parse(await _studySettingService.GetStringValue("PatientNumberLength"));

            maxSubjectNumberLength = maxSubjectNumberLength > MaximumAllowedSubjectNumberLength
                ? MaximumAllowedSubjectNumberLength
                : maxSubjectNumberLength;

            var model = new SubjectInformationModel
            {
                Name = subjectNumberTranslation,
                Min = "1",
                Max = "999999999".Substring(0, maxSubjectNumberLength),
                ChoiceType = DataType.NumberAttribute.DisplayName,
                Sequence = 0
            };

            return model;
        }
        
        private async Task<SubjectInformationModel> BuildPatientEnrolledDateDetail(PatientDto patient)
        {
            // note - this translation does not yet exist and will need to be added
            var translation = await _translationService.GetByKey(
                CorrectionConstants.SubjectEnrolledDateTranslationKey);

            var model = new SubjectInformationModel
            {
                Min = patient.EnrolledDate?.Date.ToString(YPrimeSession.Instance.GlobalDateFormat),
                Name = translation,
                ChoiceType = DataType.DateAttribute.DisplayName,
                Sequence = 1
            };

            return model;
        }

        private async Task<SubjectInformationModel> BuildPatientLanguageDetail(
            LanguageModel selectedLanguage,
            IEnumerable<LanguageModel> languages)
        {
            var translation = await _translationService.GetByKey(CorrectionConstants.SubjectLanguageTranslationKey);

            var choices = languages
                .OrderBy(l => l.DisplayName)
                .Select((l, i) => new ChoiceModel
                {
                    Id = l.Id,
                    Name = l.DisplayName,
                    DisplayOrder = i + 1
                })
                .ToList();

            // Ensure that the selected language is in the list
            // to cover the case of it being removed from the site
            if (selectedLanguage != null && !choices.Any(c => c.Id == selectedLanguage.Id))
            {
                choices.Insert(0, new ChoiceModel
                {
                    Id = selectedLanguage.Id,
                    Name = selectedLanguage.DisplayName,
                    DisplayOrder = 0
                });
            }

            var model = new SubjectInformationModel
            {
                Name = translation,
                ChoiceType = DataType.ChoicesAttribute.DisplayName,
                Sequence = 2,
                Choices = choices
            };

            return model;
        }

        private async Task<SubjectInformationModel> BuildStatusDetails()
        {
            var statusTranslation = await _translationService.GetByKey(CorrectionConstants.SubjectStatusTranslationKey);

            var model = new SubjectInformationModel
            {
                Name = statusTranslation,
                ChoiceType = DataType.ChoicesAttribute.DisplayName,
                Sequence = 3
            };

            var patientStatuses = await _patientStatusService.GetAll();
            var statusTypes = patientStatuses
                .Where(pst => !pst.IsRemoved)
                .OrderBy(st => st.Id);

            foreach (var statusType in statusTypes)
            {
                var choice = new ChoiceModel
                {
                    AlternateId = statusType.Id.ToString(),
                    Name = statusType.Name,
                    DisplayOrder = statusType.Id
                };

                model.Choices.Add(choice);
            }

            return model;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Filters;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Utils;
using YPrime.eCOA.DTOLibrary.ViewModel;
using YPrime.BusinessLayer.Responses;
using System.Text;
using YPrime.eCOA.Utilities.EncryptionLibrary;
using YPrime.Config.Defaults;
using Autofac.Core;
using YPrime.BusinessRule.Interfaces;

namespace YPrime.BusinessLayer.Repositories
{
    public class PatientRepository : BaseRepository, IPatientRepository
    {
        private readonly IPatientVisitRepository _patientVisitRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IPrimeInventoryAPIRepository _primeInventoryAPIRepository;
        private readonly ICountryService _countryService;
        private readonly ISubjectInformationService _subjectInformationService;
        private readonly IStudySettingService _studySettingService;
        private readonly ITranslationService _translationService;
        private readonly IVisitService _visitService;
        private readonly IPatientStatusService _patientStatusService;
        private readonly ICareGiverTypeService _careGiverTypeService;
        private readonly IQuestionnaireService _questionnaireService;
        private readonly ISoftwareReleaseRepository _softwareReleaseRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly INotificationRequestRepository _notificationRequestRepository;
        private readonly IAuthService _authService;

        public const string DailyDiaryId = "ADF2EC95-A7F7-E711-80DD-000D3A1029A9";
        private const string SubjectNumberValidationRegexPattern = @"^[0-9]*[1-9]+[0-9]*$";
        private const float ComplianceThreshold = 80f;

        public PatientRepository(
            IStudyDbContext db,
            ITranslationService translationService,
            IPatientVisitRepository patientVisitRepository,
            ISiteRepository siteRepository,
            IPrimeInventoryAPIRepository primeInventoryAPIRepository,
            ICountryService countryService,
            ISubjectInformationService subjectInformationService,
            IVisitService visitService,
            IPatientStatusService patientStatusService,
            IStudySettingService studySettingService,
            ICareGiverTypeService careGiverTypeService,
            IQuestionnaireService questionnaireService,
            ISoftwareReleaseRepository softwareReleaseRepository,
            IDeviceRepository deviceRepository,
            INotificationRequestRepository notificationRequestRepository,
            IAuthService authService)
            : base(db)
        {
            _translationService = translationService;
            _patientVisitRepository = patientVisitRepository;
            _siteRepository = siteRepository;
            _primeInventoryAPIRepository = primeInventoryAPIRepository;
            _countryService = countryService;
            _subjectInformationService = subjectInformationService;
            _visitService = visitService;
            _studySettingService = studySettingService;
            _careGiverTypeService = careGiverTypeService;
            _questionnaireService = questionnaireService;
            _softwareReleaseRepository = softwareReleaseRepository;
            _deviceRepository = deviceRepository;
            _patientStatusService = patientStatusService;
            _notificationRequestRepository = notificationRequestRepository;
            _authService = authService;
        }

        public bool AnyPatients(string patientNumber)
        {
            return _db.Patients.Any(x => x.PatientNumber == patientNumber);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatients(
            IEnumerable<Guid> allowedSites,
            bool? showActive = null)
        {

            var visits = await _visitService.GetAll().ConfigureAwait(false);
            var patientStatuses = await _patientStatusService.GetAll();

            var removedTypeIds = patientStatuses
                .Where(pst => pst.IsRemoved)
                .Select(pst => pst.Id).ToList();

            var complianceQuestionnaireId = Guid
                .Parse(DailyDiaryId);

            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var patientQueryable = patientFilter.Execute(_db.Patients)
                .Include(p => p.Site)
                .Include(p => p.DiaryEntries)
                .Include(p => p.PatientVisits);

            foreach (var removedTypeId in removedTypeIds)
            {
                patientQueryable = patientQueryable
                    .Where(p => p.PatientStatusTypeId != removedTypeId);
            }

            if (allowedSites != null && allowedSites.Any())
            {
                patientQueryable = patientQueryable
                    .Where(p => allowedSites.Contains(p.SiteId));
            }

            var anonymousResults = await patientQueryable
                .Select(p => new
                {
                    p.Id,
                    p.IsHandheldTrainingComplete,
                    p.IsTabletTrainingComplete,
                    p.IsTempPin,
                    p.SiteId,
                    p.PatientStatusTypeId,
                    p.Site.SiteNumber,
                    p.PatientNumber,
                    p.EnrolledDate,
                    PatientVisits = p
                        .PatientVisits
                        .Where(pv => pv.VisitDate.HasValue),
                    IsSiteActive = p.Site.IsActive,
                    DiaryCount = p
                        .DiaryEntries
                        .Count(de => de.QuestionnaireId == complianceQuestionnaireId),
                    LastTransmittedDiaryEntry = p
                        .DiaryEntries
                        .OrderByDescending(de => de.TransmittedTime)
                        .FirstOrDefault(),
                    LastDiaryEntry = p
                        .DiaryEntries
                        .OrderByDescending(de => de.DiaryDate)
                        .FirstOrDefault(),
                    AuthUserId = p.AuthUserId
                })
                .OrderBy(p => p.PatientNumber)
                .ToListAsync()
                .ConfigureAwait(false);

            var results = new List<PatientDto>();

            foreach (var anonymousResult in anonymousResults)
            {
                var status = patientStatuses.FirstOrDefault(pst => pst.Id == anonymousResult.PatientStatusTypeId);

                // skip patients that do not match passed in active status
                if (showActive.HasValue && status.IsActive != showActive.Value)
                {
                    continue;
                }

                var daysEnrolled = (DateTime.Now.Date - anonymousResult.EnrolledDate).Days;

                var dto = new PatientDto
                {
                    Id = anonymousResult.Id,
                    IsHandheldTrainingComplete = anonymousResult.IsHandheldTrainingComplete,
                    IsTabletTrainingComplete = anonymousResult.IsTabletTrainingComplete,
                    IsTempPin = anonymousResult.IsTempPin,
                    SiteId = anonymousResult.SiteId,
                    IsSiteActive = anonymousResult.IsSiteActive,
                    SiteNumber = anonymousResult.SiteNumber,
                    PatientNumber = anonymousResult.PatientNumber,
                    EnrolledDate = anonymousResult.EnrolledDate,
                    LastDeviceSyncDate = anonymousResult.LastTransmittedDiaryEntry?.TransmittedTime,
                    LastDiaryEntryDate = anonymousResult.LastDiaryEntry?.DiaryDate,
                    PatientStatusTypeId = anonymousResult.PatientStatusTypeId,
                    PatientStatus = status?.Name,
                    IsActive = status?.IsActive ?? false,
                    Compliance = daysEnrolled > 0
                        && (anonymousResult.DiaryCount / daysEnrolled) * 100 >= ComplianceThreshold,
                    AuthUserId = anonymousResult.AuthUserId,
                };

                var lastVisit = anonymousResult
                    .PatientVisits
                    .Select(pv => new
                    {
                        PatientVisit = pv,
                        Visit = visits.FirstOrDefault(v => v.Id == pv.VisitId)
                    })
                    .Where(av => av.Visit != null && av.Visit.IsScheduled)
                    .OrderByDescending(av => av.Visit.VisitOrder)
                    .FirstOrDefault();

                dto.LastVisit = lastVisit?.Visit.Name;
                dto.LastVisitDate = lastVisit?.PatientVisit.VisitDate;

                results.Add(dto);
            }

            return results;
        }

        public async Task<PatientDto> CreateNewPatientObject(Guid SiteId)
        {
            var currentSiteTime = _siteRepository.GetSiteLocalTime(SiteId);
            var result = new PatientDto();
            result.SiteId = SiteId;

            result.Site = await _siteRepository.GetSite(SiteId);

            var configId = await _softwareReleaseRepository.FindLatestConfigurationVersion(
                new List<Guid> { SiteId },
                new List<Guid> { result.Site.CountryId });

            result.SubjectInformations = await _subjectInformationService
                .GetAll(configId);

            result.SubjectInformations = result
                .SubjectInformations
                .Where(si => si.Countries.Any(c => c.Id == result.Site.CountryId))
                .ToList();

            //add the attribute dtos from configuration
            result.PatientAttributes = new List<PatientAttributeDto>();
            result.SubjectUsePersonalDevice = null;
            result.SubjectInformations.ForEach(si =>
            {
                result.PatientAttributes.Add(new PatientAttributeDto
                {
                    Id = Guid.NewGuid(),
                    PatientAttributeConfigurationDetailId = si.Id,
                    PatientId = result.Id,
                    AttributeValue = null,
                    SubjectInformation = si.GetDataType() == DataType.DateAttribute ? si.ParseMinAndMaxDateFormat(currentSiteTime) : si
                });
            });

            return result;
        }

        public async Task<PatientDto> BuildPatientDto(Patient entity, string cultureCode)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var ComplianceList = GetPatientComplianceListByPatientStatusCurrent(new List<Guid>() { entity.SiteId });

            var result = new PatientDto
            {
                Id = entity.Id,
                IsHandheldTrainingComplete = entity.IsHandheldTrainingComplete,
                IsTabletTrainingComplete = entity.IsTabletTrainingComplete,
                IsTempPin = entity.IsTempPin,
                SiteId = entity.SiteId,
                IsSiteActive = entity.Site.IsActive,
                SiteNumber = entity.Site.SiteNumber,
                PatientNumber = entity.PatientNumber,
                EnrolledDate = entity.EnrolledDate,
                PatientStatusTypeId = entity.PatientStatusTypeId,
                AuthUserId = entity.AuthUserId,
            };

            result.Compliance = ComplianceList.Contains(result.PatientNumber);
            result.SiteNumber = entity.Site.Name;
            result.PatientStatusType = entity.GetPatientStatusType(patientStatuses);
            result.PatientStatus = result.PatientStatusType.Name;
            result.IsActive = result.PatientStatusType.IsActive;
            result.IsSiteActive = entity.Site.IsActive;
            result.PatientVisits = await _patientVisitRepository.GetAllPatientVisit(result.Id, cultureCode);
            result.LanguageId = entity.LanguageId;

            result.ConfigurationId = entity.ConfigurationId != Guid.Empty
                ? entity.ConfigurationId
                : (Guid?)null;

            var visits = await _visitService.GetAll();

            var results = result.PatientVisits.OrderByDescending(pv => pv.VisitDate);
            var resultsId = results.Select(x => x.VisitId).ToList();

            visits = visits.OrderBy(d => resultsId.IndexOf(d.Id)).ToList();

            var isScheduled = visits.FirstOrDefault(x => x.IsScheduled);

            var lastVisit = result.PatientVisits.OrderByDescending(pv => pv.VisitDate).FirstOrDefault(pv => pv.VisitId == isScheduled.Id);
            if (lastVisit != null)
            {
                result.LastVisitDate = lastVisit.VisitDate;
                result.LastVisit = lastVisit.VisitName;
            }

            var completedQuestionnaireIds = new List<Guid>();
            completedQuestionnaireIds.AddRange(result.DiaryEntries.Select(de => de.QuestionnaireId));
            result.PatientVisits.ForEach(v =>
            {
                v.DiaryEntries.ForEach(e =>
                {
                    if (!completedQuestionnaireIds.Contains(e.QuestionnaireId))
                    {
                        completedQuestionnaireIds.Add(e.QuestionnaireId);
                    }
                });
            });

            result.QuestionnairesTaken = completedQuestionnaireIds.Any()
                ? await GetQuestionnaires(completedQuestionnaireIds)
                : new List<QuestionnaireModel>();

            result.Site = await _siteRepository.GetSite(result.SiteId);

            result.SubjectInformations = await _subjectInformationService
                    .GetForCountry(result.Site.CountryId);

            result.PatientAttributes = new List<PatientAttributeDto>();

            entity.PatientAttributes.ToList().ForEach(pa =>
            {
                var dto = new PatientAttributeDto();
                dto.CopyPropertiesFromObject(pa);
                dto.SubjectInformation = result
                        .SubjectInformations
                        .FirstOrDefault(si => si.Id == pa.PatientAttributeConfigurationDetailId);

                //include any dcfs
                _db.CorrectionApprovalDatas.Where(cad =>
                        cad.RowId == pa.Id && cad.Correction.CorrectionStatusId == CorrectionStatusEnum.Completed)
                    .OrderByDescending(cad => cad.Correction.StartedDate)
                    .ToList()
                    .ForEach(cad => { dto.CorrectionApprovalDatas.Add(cad.EntityToDto()); });

                if (dto.SubjectInformation != null)
                {
                    result.PatientAttributes.Add(dto);
                }
            });

            return result;
        }

        public async Task<PatientDto> GetPatient(Patient patient, string cultureCode)
        {
            var result = new PatientDto();

            if (patient != null)
            {
                result = await BuildPatientDto(patient, cultureCode);
            }

            return result;
        }

        public async Task<PatientDto> GetPatient(Guid id, string cultureCode)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
            patientFilter.ById(id);

            var patient = patientFilter.Execute(_db.Patients).First();

            var result = await GetPatient(patient, cultureCode);

            return result;
        }

        public async Task<Patient> GetPatientForEditAsync(Guid id)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            return patientFilter.Execute(_db.Patients)
                .Include(p => p.CareGivers)
                .Include(p => p.PatientVisits)
                .Include(p => p.Site)
                .Include(p => p.PatientAttributes)
                .FirstOrDefault(p => p.Id == id);
        }

        public async Task<IEnumerable<PatientDto>> GetPatients(IEnumerable<Guid> ids)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

            var patients = await patientFilter.Execute(_db.Patients)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            var resultList = new List<PatientDto>();

            foreach (var x in patients)
            {
                resultList.Add(await GetPatient(x, TranslationConstants.DefaultCultureCode));
            }

            return resultList;
        }

        public async Task<int> GetEnrolledPatientCount(List<Guid> AllowedSites)
        {
            int result;
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
            if (AllowedSites == null || !AllowedSites.Any())
            {
                result = patientFilter.Execute(_db.Patients).Count();
            }
            else
            {
                result = patientFilter.Execute(_db.Patients).Count(p => AllowedSites.Contains(p.SiteId));
            }

            return result;
        }

        public async Task<PatientResponse> InsertUpdatePatient(
            PatientDto patient,
            bool insert,
            ModelStateDictionary modelState)
        {
            var result = new PatientResponse { Success = false };

            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    var patientStatuses = await _patientStatusService.GetAll();
                    var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());

                    var patientEntity = insert
                                            ? new Patient()
                                            : patientFilter.Execute(_db.Patients).Single(p => p.Id == patient.Id);

                    if (insert)
                    {
                        var patientSite = await _siteRepository.GetSite(patient.SiteId);

                        var configId = await _softwareReleaseRepository.FindLatestConfigurationVersion(
                                           new List<Guid> { patient.SiteId },
                                           new List<Guid> { patientSite.CountryId });

                        patientEntity.CopyPropertiesFromObject(patient);
                        patientEntity.ConfigurationId = configId;
                    }
                    else
                    {
                        //NOTE: copyproperties is breaking some foreign key attributes, so for now the update is by hand
                        patientEntity.PatientStatusTypeId = patient.PatientStatusTypeId;
                        patient.PatientAttributes.ForEach(
                            pa =>
                                {
                                    patientEntity.PatientAttributes.Single(
                                            ea => ea.PatientAttributeConfigurationDetailId
                                                  == pa.PatientAttributeConfigurationDetailId).AttributeValue =
                                        pa.AttributeValue;
                                });
                    }

                    var currentSiteTime = _siteRepository.GetSiteLocalTime(patientEntity.SiteId);

                    if (insert && await ValidatePatientNumber(patient.PatientNumber, modelState))
                    {
                        var newPatientNumber = await GeneratePatientNumber(
                                                   patientEntity.SiteId,
                                                   patientEntity.PatientNumber,
                                                   patientEntity.ConfigurationId);

                        if (patientFilter.Execute(_db.Patients).Any(
                                p => p.PatientNumber.Equals(
                                    newPatientNumber,
                                    StringComparison.CurrentCultureIgnoreCase)))
                        {
                            var errorMessage = (await _translationService.GetByKey("keyDuplicateSubjectError"));
                            modelState.AddModelError("PatientNumber", errorMessage);
                        }

                        patientEntity.PatientNumber = newPatientNumber;
                        patientEntity.Id = Guid.NewGuid();
                        patientEntity.PatientStatusTypeId = 1;
                        patientEntity.EnrolledDate = currentSiteTime;
                        patientEntity.IsTempPin = true;
                        patientEntity.Pin = await GetDefaultPINEncrypted(patientEntity.Id.ToString());

                        _db.Patients.Add(patientEntity);
                    }

                    if (modelState.IsValid && await ValidatePatientModel(patientEntity, insert, modelState, currentSiteTime, patient.PatientAttributes))
                    {
                        result.PatientId = patientEntity.Id;
                        _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
                        await _patientVisitRepository.ProjectPatientVisitSchedule(patientEntity.Id);

                        result.Success = true;
                    }

                    if (result.Success)
                    {
                        try
                        {
                            var authUserSignupResponse = await _authService.CreateSubjectAsync(result.PatientId, await this.GenerateDefaultPin());
                            await this.AddAuthUserIdAsync(result.PatientId, authUserSignupResponse.UserId);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                            result.Success = false;
                            modelState.AddModelError("AuthIdentity", "Failed to Create Patient Identity. Please try again");

                            return result;
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();

                    result.Success = false;
                    throw;
                }
            }

            return result;
        }

        public async Task<bool> ValidatePatientNumber(string patientNumber, ModelStateDictionary modelState)
        {
            var result = true;

            var patientNumberLength = await _studySettingService.GetIntValue("PatientNumberLength");

            if (patientNumber != null && patientNumber.Length != patientNumberLength)
            {
                result = false;
                modelState.AddModelError("PatientNumber",
                    YPrimeSession.Instance.SinglePatientAlias + " number must be " + patientNumberLength +
                    " digits long.");
            }

            if (!Regex.IsMatch(patientNumber, SubjectNumberValidationRegexPattern))
            {
                result = false;
                modelState.AddModelError("PatientNumber", YPrimeSession.Instance.SinglePatientAlias + " number must be greater than 0.");
            }

            return result;
        }

        public async Task UpdatePatientStatusTypeId(Guid patientId, int patientStatusTypeId)
        {
            var patient = _db.Patients.Single(p => p.Id == patientId);
            var oldPatientStatusTypeId = patient.PatientStatusTypeId;
            patient.PatientStatusTypeId = patientStatusTypeId;
            _db.Patients.AddOrUpdate(patient);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            await UpdateNotificationScheduleForPatient(patientId, patientStatusTypeId, oldPatientStatusTypeId);

        }

        public async Task UpdateNotificationScheduleForPatient(Guid patientId, int newPatientStatusTypeId, int oldPatientStatusTypeId)
        {
            // Check is patient is updated to an inactive status
            var patientStatusTypeList = await _patientStatusService.GetAll();
            var newPatientStatusType = patientStatusTypeList.FirstOrDefault(p => p.Id == newPatientStatusTypeId);
            var oldPatientStatusType = patientStatusTypeList.FirstOrDefault(p => p.Id == oldPatientStatusTypeId);

            if (newPatientStatusType != null && oldPatientStatusType != null &&
                (newPatientStatusType.IsDisabled() || (oldPatientStatusType.IsDisabled() && !newPatientStatusType.IsDisabled())))
            {
                await _notificationRequestRepository.ProcessChangePatientStatusRequest(
                    patientId,
                    newPatientStatusType.IsActive);
            }
        }

        public async Task<string> GeneratePatientNumber(Guid siteId, string patientNumber, Guid? configurationId = null)
        {
            var siteFilter = new SiteFilter();

            var site = await siteFilter.Execute(_db.Sites)
                .FirstOrDefaultAsync(x => x.Id == siteId);

            var subjectVariables = await SetUpStudySubjectVariables(configurationId);

            var result = subjectVariables.PatientNumberPrefix.Trim() +
                         subjectVariables.PatientNumberPrefixSeparator.Trim() +
                         (subjectVariables.PatientNumberIncludeSiteId ? site.SiteNumber : "") +
                         subjectVariables.PatientNumberSiteSubjectNumberSeparator.Trim() + patientNumber;
            return result;
        }

        public List<string> GetPatientComplianceListByPatientStatusCurrent(List<Guid> SiteIds)
        {
            var patientList = _db.Patients
                .Where(p => !SiteIds.Any() || SiteIds.Contains(p.SiteId))
                .ToList();

            var result = patientList.Where(p => p.PatientStatusTypeId != 1)
                .Select(p => p.PatientNumber)
                .ToList();

            return result;
        }

        public async Task<string> GeneratePin()
        {
            var patientPinLengthSettingValue = await _studySettingService.GetIntValue("PatientPINLength");

            var patientPINLength = Math.Abs(patientPinLengthSettingValue);
            var result = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < patientPINLength; i++)
            {
                result.Append(random.Next(0, 10).ToString());
            }

            return result.ToString();
        }

        public async Task<string> GenerateDefaultPin()
        {
            var patientPinLengthSettingValue = await _studySettingService.GetIntValue("PatientPINLength");

            var patientPINLength = Math.Abs(patientPinLengthSettingValue);
            var pin = string.Concat(Enumerable.Range(1, patientPINLength).Select(c => c.ToString()));
            return pin;
        }

        public string DecryptPin(PatientDto patient)
        {
            var encryptionLibrary = new PasswordEncrypt(patient.Id.ToString());
            return encryptionLibrary.Decrypt(patient.Pin);
        }

        public string DecryptPin(Patient patient)
        {
            var encryptionLibrary = new PasswordEncrypt(patient.Id.ToString());
            return encryptionLibrary.Decrypt(patient.Pin);
        }

        public async Task<bool> ResetPatientPin(Guid Id, string NewPin)
        {
            var result = false;
            var encryptionLibrary = new PasswordEncrypt(Id.ToString());

            var patientStatuses = await _patientStatusService.GetAll();          
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
            var patient = patientFilter.Execute(_db.Patients).Single(p => p.Id == Id);
            patient.Pin = encryptionLibrary.Encrypt(NewPin);
            patient.SyncVersion = patient.SyncVersion + 1;
            patient.IsTempPin = true;
            patient.AccessFailedCount = 0;
            patient.LockoutEnabled = false;

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            result = true;

            return result;
        }

        public async Task<Patient> GetPatientEntity(Guid id)
        {
            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
            return patientFilter.Execute(_db.Patients).SingleOrDefault(p => p.Id == id);
        }

        public void UpsertPatientEntity(Patient patient)
        {
            _db.Patients.AddOrUpdate(patient);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public async Task<Device> AddBringYourOwnDeviceAssetTag(string PrimeInventoryAPIUrl, string Environment, Guid PatientId,
                    int NumberOfUses)
        {
            Device device = null;
            _primeInventoryAPIRepository.SetBaseUrl(PrimeInventoryAPIUrl);

            var studyId = await _studySettingService.GetGuidValue("StudyID");
            var deviceDto = await _primeInventoryAPIRepository.AddBringYourOwnDeviceAssetTag(studyId, Environment, NumberOfUses);
            if (deviceDto != null)
            {
                var patientStatuses = await _patientStatusService.GetAll();
                var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
                patientFilter.ById(PatientId);

                var patient = patientFilter.Execute(_db.Patients).Single();
                device = _deviceRepository.AddDevice(deviceDto.Id, patient.Id, patient.SiteId, DeviceType.BYOD.Id, deviceDto.AssetTag);
            }
            return device;
        }

        public async Task<List<MergePatientDto>> GetDuplicatePatientsById(
            Guid patientId,
            string cultureCode,
            string globalDateFormat)
        {
            var result = new List<MergePatientDto>();
            int position = 1;

            var patientStatuses = await _patientStatusService.GetAll();
            var visits = await _visitService.GetAll();

            var patient = await _db.Patients
                .FirstAsync(p => p.Id == patientId);

            var subjectInfoDictionary = new Dictionary<Guid, List<SubjectInformationModel>>();

            var subjectInfos = await _subjectInformationService
                .GetAll(patient.ConfigurationId);

            subjectInfoDictionary[patient.ConfigurationId] = subjectInfos;

            var removedStatuses = patientStatuses?.Where(ps => ps.IsRemoved).Select(ps => ps.Id).ToList();
            var dupePatients = await _db.Patients
                .Where(p =>
                    p.SiteId == patient.SiteId &&
                    p.PatientNumber == patient.PatientNumber &&
                    !removedStatuses.Contains(p.PatientStatusTypeId))
                .Include(p => p.DiaryEntries)
                .Include(p => p.PatientVisits)
                .Include(p => p.PatientAttributes)
                .ToListAsync();

            foreach (var dupePatient in dupePatients)
            {
                if (!subjectInfoDictionary.ContainsKey(dupePatient.ConfigurationId))
                {
                    var configVersionSubjectInfos = await _subjectInformationService
                        .GetAll(dupePatient.ConfigurationId);

                    subjectInfoDictionary[dupePatient.ConfigurationId] = configVersionSubjectInfos;
                }
            }

            var questionnaires = await _questionnaireService
                .GetAllInflatedQuestionnaires();

            foreach (var dupePatient in dupePatients)
            {
                var diaryEntryDtos = new List<DiaryEntryDto>();

                foreach (var dupeDiaryEntry in dupePatient.DiaryEntries)
                {
                    var matchingQuestionnaire = questionnaires
                        .FirstOrDefault(q => q.Id == dupeDiaryEntry.QuestionnaireId);

                    var dto = dupeDiaryEntry.ToDto(
                        matchingQuestionnaire,
                        diaryDateFormat: globalDateFormat);

                    diaryEntryDtos.Add(dto);
                }

                var mergePatientDto = new MergePatientDto
                {
                    PatientNumber = dupePatient.PatientNumber,
                    PatientId = dupePatient.Id,
                    PatientStatusTypeId = dupePatient.PatientStatusTypeId,
                    PatientStatus = dupePatient.GetPatientStatusType(patientStatuses)?.Name,
                    Position = position++,
                    DiaryEntries = diaryEntryDtos,
                    PatientVisits = await ConvertPatientVisitsDto(dupePatient.PatientVisits, cultureCode, visits),
                    PatientAttributes = ConvertPatientAttributes(dupePatient.PatientAttributes, subjectInfoDictionary[dupePatient.ConfigurationId])
                };

                result.Add(mergePatientDto);
            }

            return result;
        }

        public async Task<bool> InsertCareGiver(Guid patientId, Guid careGiverTypeId)
        {
            var result = false;

            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
            var currentPatient = patientFilter.Execute(_db.Patients).First(p => p.Id == patientId);

            var careGiverTypes = await _careGiverTypeService.GetAll();
            var selectedCareGiverType = careGiverTypes.FirstOrDefault(c => c.Id == careGiverTypeId);

            if (currentPatient != null && selectedCareGiverType != null)
            {
                var careGiverId = Guid.NewGuid();
                var careGiver = new CareGiver
                {
                    Id = careGiverId,
                    PatientId = patientId,
                    CareGiverTypeId = careGiverTypeId,
                    SyncVersion = 0,
                    IsTempPin = true,
                    Pin = await GetDefaultPINEncrypted(careGiverId.ToString()),
                    LoginAttempts = 0
                };

                _db.CareGivers.Add(careGiver);
                _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

                result = true;
            }

            return result;
        }

        public async Task<IOrderedQueryable<CareGiverDto>> GetCareGivers(Guid patientId)
        {
            var caregiverDtos = new List<CareGiverDto>();
            var caregivers = _db.CareGivers.Where(c => c.PatientId == patientId).ToList();
            var careGiverTypes = await _careGiverTypeService.GetAll();
            caregivers.ForEach(c =>
            {
                var dto = new CareGiverDto();
                dto.CopyPropertiesFromObject(c);
                dto.CareGiverType = careGiverTypes.FirstOrDefault(x => x.Id == dto.CareGiverTypeId);
                caregiverDtos.Add(dto);
            });
            return caregiverDtos.AsQueryable().OrderBy(c => c.CareGiverType.Name);
        }

        public bool ResetCareGiverPin(Guid Id, string NewPin)
        {
            var result = true;
            var encryptionLibrary = new PasswordEncrypt(Id.ToString());
            var caregiver = _db.CareGivers.SingleOrDefault(c => c.Id == Id);
            caregiver.Pin = encryptionLibrary.Encrypt(NewPin);
            caregiver.SyncVersion = caregiver.SyncVersion + 1;
            caregiver.IsTempPin = true;
            caregiver.AccessFailedCount = 0;
            caregiver.LockoutEnabled = false;
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            return result;
        }

        public async Task<string> GetCareGiverTypeName(Guid Id)
        {
            var careGiver = _db.CareGivers.SingleOrDefault(c => c.Id == Id);
            var careGiverTypes = await _careGiverTypeService.GetAll();
            return careGiverTypes.FirstOrDefault(x => x.Id == careGiver?.CareGiverTypeId)?.Name;
        }

        public async Task<bool> ValidatePatientAttributesFromDetail(List<PatientAttributeDto> patientAttributes,
            ModelStateDictionary modelState,
            DateTimeOffset currentSiteTime,
            Guid patientId,
            bool validationIsForCorrection = false,
            bool usePatientConfig = false)
        {
            var configId = await GetAssignedConfiguration(patientId);
            var subjectInfoModels = usePatientConfig ? await _subjectInformationService.GetAll(configId) :
                await _subjectInformationService.GetAll();

            foreach (var pa in patientAttributes)
            {
                pa.SubjectInformation = subjectInfoModels
                    .First(m => m.Id == pa.PatientAttributeConfigurationDetailId);

                if (string.IsNullOrEmpty(pa.AttributeValue))
                {
                    await AddModelError(pa, TranslationConstants.SubjectAttributeRequiredFieldErrorSuffix);
                }
                else
                {
                    var dataType = pa.SubjectInformation.GetDataType();

                    switch (dataType)
                    {
                        case var d when d == DataType.TextAttribute:
                            {
                                if (!pa.AttributeValue.TextLengthIsValid(
                                    pa.SubjectInformation.GetMininumValue(),
                                    pa.SubjectInformation.GetMaximumValue()))
                                {
                                    await AddModelError(pa, TranslationConstants.SubjectAttributeInvalidTextLengthErrorSuffix);
                                }

                                break;
                            }
                        case var d when d == DataType.LettersOnlyAttribute:
                            {
                                if (pa.AttributeValue.ContainsNumbers())
                                {
                                    await AddModelError(pa, TranslationConstants.SubjectAttributeContainsNumbersErrorSuffix);
                                }

                                if (!pa.AttributeValue.TextLengthIsValid(
                                    pa.SubjectInformation.GetMininumValue(),
                                    pa.SubjectInformation.GetMaximumValue()))
                                {
                                    await AddModelError(pa, TranslationConstants.SubjectAttributeInvalidTextLengthErrorSuffix);
                                }

                                break;
                            }
                        case var d when d == DataType.DateAttribute:
                            {
                                if (!pa.SubjectInformation.HasValidDateFormat(pa.AttributeValue))
                                {
                                    await AddModelError(pa, TranslationConstants.SubjectAttributeInvalidDateFormatErrorSuffix);
                                }
                                else if (!validationIsForCorrection &&
                                         !pa.SubjectInformation.HasDateWithinRangeOfMinMax(
                                             pa.AttributeValue, currentSiteTime))
                                {
                                    await AddModelError(pa, TranslationConstants.SubjectAttributeInvalidDateRangeErrorSuffix);
                                }

                                break;
                            }
                        case var d
                            when d == DataType.NumberAttribute || d == DataType.DecimalNumberAttribute:
                            {
                                var isValidNumericValue = pa.AttributeValue.HasValidNumericValue(pa.SubjectInformation.Decimal);
                                if (!isValidNumericValue)
                                {
                                    await AddModelError(pa, TranslationConstants.InvalidNumericValueErrorSuffix);
                                }
                                else
                                {
                                    var min = pa.SubjectInformation.GetMininumValue();
                                    var max = pa.SubjectInformation.GetMaximumValue();
                                    if (!pa.AttributeValue.ValidateValueInRange((decimal?)min, (decimal?)max))
                                    {
                                        await AddModelError(pa, TranslationConstants.InvalidRangeValueErrorSuffix, min: min.ToFormattedString(pa.SubjectInformation.Decimal), max: max.ToFormattedString(pa.SubjectInformation.Decimal));
                                    }
                                }

                                break;
                            }
                    }
                }
            };

            return modelState.IsValid;

            async Task AddModelError(PatientAttributeDto pa, string errorKey, string max = null, string min = null)
            {
                var errorTranslation = await _translationService.GetByKey(errorKey);
                var fieldName = pa.SubjectInformation.Name;

                switch (errorKey)
                {
                    case TranslationConstants.SubjectAttributeInvalidTextLengthErrorSuffix:
                        if (!string.IsNullOrEmpty(max))
                        {
                            errorTranslation = errorTranslation.Replace("{{numberLength}}", max);
                        }
                        break;
                    case TranslationConstants.InvalidRangeValueErrorSuffix:
                        errorTranslation = string.Format(errorTranslation, min, max);
                        break;
                    default:
                        break;
                }
                modelState.AddModelError($"{fieldName}-{errorKey}", $"{fieldName} {errorTranslation}");
            }
        }

        public async Task<Guid> GetAssignedConfiguration(Guid patientId)
        {
            var assignedConfiguration = Guid.Empty;

            var assignedDevice = await _db
                .Devices
                .Include(d => d.SoftwareRelease)
                .FirstOrDefaultAsync(d => d.PatientId == patientId);

            if (assignedDevice != null)
            {
                assignedConfiguration = assignedDevice.SoftwareRelease.ConfigurationId;
            }
            else
            {
                var patient = await _db
                .Patients
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.Id == patientId);

                if (patient != null)
                {
                    assignedConfiguration = await _softwareReleaseRepository.FindLatestConfigurationVersion(
                        new List<Guid> { patient.SiteId },
                        new List<Guid> { patient.Site.CountryId });
                }
            }

            return assignedConfiguration;
        }

        public async Task AddAuthUserIdAsync(Guid patientId, string authUserId)
        {
            var patient = await _db
                .Patients
                .FirstOrDefaultAsync(p => p.Id == patientId);
            patient.AuthUserId = authUserId;
            UpsertPatientEntity(patient);
        }

        private async Task<SubjectVariables> SetUpStudySubjectVariables(Guid? configurationId)
        {
            // PatientNumberLength
            // Accessing the session object from hangfire doesn't work, need to rethink this.
            if (YPrimeSession.Instance == null || YPrimeSession.Instance.StudySettingValues == null)
            {
                var studySettings = await _studySettingService.GetAllStudyCustoms(configurationId);

                return new SubjectVariables
                {
                    MaxEnteredPatientNumberLength = studySettings.First(sc => sc.Key == "PatientNumberLength").GetIntValue(),
                    PatientNumberIncludeSiteId = studySettings.First(sc => sc.Key == "PatientNumberIncludeSiteId").Value == "1",
                    PatientNumberPrefix = studySettings.First(sc => sc.Key == "PatientNumberPrefix").Value,
                    PatientNumberPrefixSeparator = studySettings.First(sc => sc.Key == "PatientNumberPrefixSiteSeparator").Value,
                    PatientNumberSiteSubjectNumberSeparator = studySettings.First(sc => sc.Key == "PatientNumberSiteSubjectNumberSeparator").Value,
                    PatientNumberIsStudyWide = string.Equals(
                        studySettings.First(sc => sc.Key == "PatientNumberIsStudyWide").Value,
                        bool.TrueString,
                        StringComparison.CurrentCultureIgnoreCase)
                };
            }

            return new SubjectVariables
            {
                MaxEnteredPatientNumberLength =
                    int.Parse(YPrimeSession.Instance.StudySettingValues["PatientNumberLength"]),
                PatientNumberIncludeSiteId =
                    YPrimeSession.Instance.StudySettingValues["PatientNumberIncludeSiteId"] == "1",
                PatientNumberPrefix = YPrimeSession.Instance.StudySettingValues["PatientNumberPrefix"],
                PatientNumberPrefixSeparator =
                    YPrimeSession.Instance.StudySettingValues["PatientNumberPrefixSiteSeparator"],
                PatientNumberSiteSubjectNumberSeparator =
                    YPrimeSession.Instance.StudySettingValues["PatientNumberSiteSubjectNumberSeparator"],
                PatientNumberIsStudyWide =
                    string.Equals(YPrimeSession.Instance.StudySettingValues["PatientNumberIsStudyWide"], bool.TrueString,
                        StringComparison.CurrentCultureIgnoreCase)
            };
        }

        private async Task<bool> ValidatePatientModel(Patient patient, bool inserting, ModelStateDictionary modelState, DateTimeOffset currentSiteTime, List<PatientAttributeDto> patientAttributeDtos)
        {
            var result = true;

            var patientStatuses = await _patientStatusService.GetAll();
            var patientFilter = new PatientFilter(patientStatuses?.Where(p => p.IsRemoved).ToList());
            if (inserting && patientFilter.Execute(_db.Patients).Any(p => p.PatientNumber.Equals(patient.PatientNumber, StringComparison.CurrentCultureIgnoreCase)))
            {
                modelState.AddModelError("PatientNumber", YPrimeSession.Instance.SinglePatientAlias + " already exists.");
                result = false;
            }

            var attributeValidationResult = await ValidatePatientAttributesFromDetail(
                patientAttributeDtos,
                modelState,
                currentSiteTime,
                patient.Id);

            result = !attributeValidationResult
                ? false
                : result;

            return result;
        }

        private async Task<List<QuestionnaireModel>> GetQuestionnaires(List<Guid> questionnaireIds)
        {
            if (!questionnaireIds.Any())
            {
                return new List<QuestionnaireModel>();
            }

            var questionnaires = await _questionnaireService
                .GetAll();

            var result = questionnaires
                .Where(q => questionnaireIds.Contains(q.Id))
                .ToList();

            return result;
        }

        private async Task<string> GetDefaultPINEncrypted(string patientId)
        {
            var encryptionLib = new PasswordEncrypt(patientId);
            var pin = encryptionLib.Encrypt(await GenerateDefaultPin());
            return pin;
        }

        private List<PatientAttributeDto> ConvertPatientAttributes(
            ICollection<PatientAttribute> patientAttributes,
            List<SubjectInformationModel> subjectInformationModels)
        {
            var results = new List<PatientAttributeDto>();

            foreach (var attribute in patientAttributes)
            {
                var matchingSubjectInfo = subjectInformationModels
                    .FirstOrDefault(si => si.Id == attribute.PatientAttributeConfigurationDetailId);

                var dto = attribute.ToDto(matchingSubjectInfo);

                if (matchingSubjectInfo.GetDataType().IsMultipleChoice && Guid.TryParse(dto.AttributeValue, out var parsedChoiceId))
                {
                    var matchingChoice = matchingSubjectInfo
                        .Choices
                        .FirstOrDefault(c => c.Id == parsedChoiceId);

                    dto.AttributeValue = matchingChoice?.Name ?? dto.AttributeValue;
                }

                results.Add(dto);
            }

            results = results
                .OrderBy(r => r.SubjectInformation?.Sequence)
                .ToList();

            return results;
        }

        private async Task<List<PatientVisitDto>> ConvertPatientVisitsDto(
            ICollection<PatientVisit> patientVisits,
            string cultureCode,
            List<VisitModel> visits)
        {
            var result = new List<PatientVisitDto>();

            foreach (var patientVisit in patientVisits)
            {
                var matchingVisit = visits.FirstOrDefault(v => v.Id == patientVisit.VisitId);

                var dto = new PatientVisitDto();
                dto.CopyPropertiesFromObject(patientVisit);
                dto.VisitName = matchingVisit?.Name;
                dto.VisitOrder = matchingVisit?.VisitOrder;
                dto.PatientVisitDescription = await _patientVisitRepository.GetPatientVisitDescription(patientVisit, cultureCode);
                result.Add(dto);
            }

            result = result
                .OrderBy(r => r.VisitOrder)
                .ThenBy(r => r.VisitDate)
                .ToList();

            return result;
        }

        public async Task RemovePatient(Guid patientId)
        {
            if (await _db.Patients.AnyAsync(q => q.Id == patientId))
            {
                var matchingPatient = await _db.Patients.FirstAsync(q => q.Id == patientId);

                // Only remove patients without saved data
                if (matchingPatient.PatientStatusTypeId == PatientStatusTypes.Enrolled.Id
                    && !matchingPatient.DiaryEntries.Any())
                {
                    _db.Patients.Remove(matchingPatient);

                    await _db.SaveChangesAsync(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
                }
            }
        }

        internal class SubjectVariables
        {
            public int MaxEnteredPatientNumberLength { get; set; }
            public bool PatientNumberIncludeSiteId { get; set; }
            public string PatientNumberPrefix { get; set; }
            public string PatientNumberPrefixSeparator { get; set; }
            public string PatientNumberSiteSubjectNumberSeparator { get; set; }
            public bool PatientNumberIsStudyWide { get; set; }
        }
    }
}
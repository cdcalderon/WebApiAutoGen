using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;

namespace YPrime.BusinessLayer.Repositories
{
    public class AlarmRepository : IAlarmRepository
    {
        private readonly IRuleService _ruleService;
        private readonly IAlarmService _alarmService;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ISiteRepository _siteRepository;

        public AlarmRepository(IRuleService ruleService,
            IAlarmService alarmService,
            IDeviceRepository deviceRepository,
            IPatientRepository patientRepository,
            ISiteRepository siteRepository)
        {
            _ruleService = ruleService;
            _alarmService = alarmService;
            _deviceRepository = deviceRepository;
            _patientRepository = patientRepository;
            _siteRepository = siteRepository;
        }

        public async Task<ApiRequestResultViewModel> ExecuteAlarmBusinessRule(AlarmRuleDto alarmDto)
        {
            var resultViewModel = new ApiRequestResultViewModel();

            var configId = await _deviceRepository.GetLastReportedConfigurationForDevice(alarmDto.DeviceId);

            if (configId == null)
            {
                throw new DeviceNotFoundException();
            }

            var patient = await _patientRepository.GetPatientEntity(alarmDto.PatientId);

            if (patient == null)
            {
                throw new PatientNotFoundException();
            }

            var alarmModel = await _alarmService.GetTranslatedAlarmModel(
                alarmDto.AlarmId, 
                null, 
                configId);

            if (alarmModel == null)
            {
                throw new AlarmNotFoundException();
            }

            if (alarmModel.NotifyBusinessRuleId.HasValue)
            {
                var currentSiteTime = _siteRepository.GetSiteLocalTime(patient.SiteId);

                var result = _ruleService.ExecuteBusinessRule(
                    alarmModel.NotifyBusinessRuleId.Value,
                    patient.Id,
                    patient.SiteId,
                    alarmModel.BusinessRuleTrueFalseIndicator,
                    currentSiteTime.DateTime);

                if (result == null)
                {
                    throw new BusinessRuleException();
                }

                resultViewModel.WasSuccessful = result.ExecutionResult;
            }
            else
            {
                resultViewModel.WasSuccessful = true;
            }

            return resultViewModel;
        }
    }
}

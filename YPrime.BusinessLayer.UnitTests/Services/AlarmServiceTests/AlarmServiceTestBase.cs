using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Services;

namespace YPrime.BusinessLayer.UnitTests.Services.AlarmServiceTests
{
    public abstract class AlarmServiceTestBase : ConfigServiceTestBase<AlarmModel>
    {
        protected string ExpectedBaseEndpointAddress = $"{BaseTestHttpAddress}/Alarms";

        protected const string AllTranslatedAlarmResponse = "[{\"id\": \"c7cb77fb-a52d-49e2-be5d-cf2a9ea3cd5c\",\"name\": \"Alarm 1\",\"title\": \"Alarm 1\",\"message\": \"Please complete the daily dairy\",\"alarmHour\": null,\"alarmMinute\": null,\"minimumAlarmHour\": 8,\"minimumAlarmMinute\": 0,\"maximumAlarmHour\": 20,\"maximumAlarmMinute\": 0,\"seriesRetries\": null,\"seriesInterval\": null,\"seriesStopTime\": \"22:00:00\",\"ringInterval\": 10,\"notifyBusinessRuleId\": \"d4f2bebf-d185-4505-81e9-f115e221d3f4\",\"businessRuleTrueFalseIndicator\": true }]";

        protected const string TranslatedAlarmResponse = "{\"id\": \"c7cb77fb-a52d-49e2-be5d-cf2a9ea3cd5c\",\"name\": \"Alarm 1\",\"title\": \"Alarm 1\",\"message\": \"Please complete the daily dairy\",\"alarmHour\": null,\"alarmMinute\": null,\"minimumAlarmHour\": 8,\"minimumAlarmMinute\": 0,\"maximumAlarmHour\": 20,\"maximumAlarmMinute\": 0,\"seriesRetries\": null,\"seriesInterval\": null,\"seriesStopTime\": \"22:00:00\",\"ringInterval\": 10,\"notifyBusinessRuleId\": \"d4f2bebf-d185-4505-81e9-f115e221d3f4\",\"businessRuleTrueFalseIndicator\": true }";

        protected AlarmServiceTestBase()
            : base(AllTranslatedAlarmResponse)
        { }

        protected IAlarmService GetService()
        {
            var service = new AlarmService(
                MockHttpFactory.Object,
                MemoryCache,
                MockSessionService.Object,
                TestServiceSettings,
                _authSettings,
                MockAuthService.Object);

            return service;
        }
    }
}

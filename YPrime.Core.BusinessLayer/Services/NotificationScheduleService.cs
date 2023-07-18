using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Extensions;


namespace YPrime.Core.BusinessLayer.Services
{
    public class NotificationScheduleService : INotificationScheduleService
    {
        private readonly string _httpClientName = "notificationHttpClient";
        private readonly string _cancelSchedulePath = "notificationScheduler/cancel";
        private readonly string _updatePatientStatusAlarmPath = "notificationScheduler/updatepatientalarmstatus";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceSettings _serviceSettings;

        public NotificationScheduleService(IHttpClientFactory httpClientFactory, IServiceSettings serviceSettings)
        {
            _httpClientFactory = httpClientFactory;
            _serviceSettings = serviceSettings;
        }


        public async Task<HttpResponseMessage> CancelScheduleAsync(Guid patientId)
        {
            if (patientId == Guid.Empty) return null;

            var request = new
            {
                PatientId = patientId.ToString()
            };

            var json = JsonConvert.SerializeObject(request);
            var client = _httpClientFactory.CreateClient(_httpClientName);
            client.SetHmacAuthorizationHeader(patientId.ToString(), json, _serviceSettings.HMACAuthSharedKey);
            var response = await client.PostAsync($"{_cancelSchedulePath}", new StringContent(json));
            return response;
        }
        public async Task<HttpResponseMessage> UpdatePatientAlarmStatusAsync(Guid patientId, bool active)
        {
            if (patientId == Guid.Empty) return null;

            var request = new
            {
                PatientId = patientId.ToString(),
                PatientStatus = active.ToString()
            };

            var json = JsonConvert.SerializeObject(request);
            var client = _httpClientFactory.CreateClient(_httpClientName);
            client.SetHmacAuthorizationHeader(patientId.ToString(), json, _serviceSettings.HMACAuthSharedKey);
            var response = await client.PostAsync($"{_updatePatientStatusAlarmPath}", new StringContent(json));
            return response;
        }
    }
}

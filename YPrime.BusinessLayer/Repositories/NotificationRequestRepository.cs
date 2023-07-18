using System;
using System.Net.Http;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.Repositories
{
    public class NotificationRequestRepository : BaseRepository, INotificationRequestRepository
    {
        private readonly INotificationScheduleService _notificationScheduleService;

        public NotificationRequestRepository(IStudyDbContext db,
            INotificationScheduleService notificationScheduleService) : base(db)
        {
            _notificationScheduleService = notificationScheduleService;
        }

        public async Task<bool> ProcessCancelationRequest(Guid patientId)
        {
            var response = await _notificationScheduleService.CancelScheduleAsync(patientId);
            SaveNotificationRequest(response, patientId);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ProcessChangePatientStatusRequest(Guid patientId, bool active)
        {
            var response = await _notificationScheduleService.UpdatePatientAlarmStatusAsync(patientId, active);
            SaveNotificationRequest(response, patientId);
            return response.IsSuccessStatusCode;
        }    

        private void SaveNotificationRequest(HttpResponseMessage httpResponseMessage, Guid patientId)
        {
            var entity = new NotificationRequest
            {
               PatientId = patientId,
               AuthenticationHeader = httpResponseMessage.Headers?.ToString(),
               ReponseCode = (int)httpResponseMessage.StatusCode,
               RequestBody = httpResponseMessage.RequestMessage?.ToString()
            };

            _db.NotificationRequests.Add(entity);
            _db.SaveChanges(null);
        }
    }
}

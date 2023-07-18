using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface INotificationScheduleService
    {
        Task<HttpResponseMessage> CancelScheduleAsync(Guid patientId);
        Task<HttpResponseMessage> UpdatePatientAlarmStatusAsync(Guid patientId, bool active);
    }
}
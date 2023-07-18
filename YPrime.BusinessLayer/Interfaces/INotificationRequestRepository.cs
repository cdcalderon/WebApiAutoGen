using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface INotificationRequestRepository
    {
        Task<bool> ProcessCancelationRequest(Guid patientId);
        Task<bool> ProcessChangePatientStatusRequest(Guid patientId, bool IsActive);
    }
}

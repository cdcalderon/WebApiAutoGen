using System.Collections.Generic;
using YPrime.Data.Study.Models;
using YPrime.Core.BusinessLayer.Models;
using System.Linq;

namespace YPrime.BusinessLayer.Extensions
{
    public static class PatientExtensions
    {
        public static PatientStatusModel GetPatientStatusType(this Patient patient, List<PatientStatusModel> patientStatusTypes)
        {
            var statusType = patientStatusTypes.FirstOrDefault(pst => pst.Id == patient.PatientStatusTypeId);

            return statusType;
        }
    }
}

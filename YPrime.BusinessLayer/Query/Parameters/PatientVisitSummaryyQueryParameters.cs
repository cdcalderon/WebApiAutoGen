using System;

namespace YPrime.BusinessLayer.Query.Parameters
{
    public class PatientVisitSummaryQueryParameters
    {
        public Guid PatientId { get; }
        public PatientVisitSummaryQueryParameters(Guid patientId)
        {
            this.PatientId = patientId;
        }
    }
}

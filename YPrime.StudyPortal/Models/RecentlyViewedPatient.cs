using System;

namespace YPrime.StudyPortal.Models
{
    [Serializable]
    public class RecentlyViewedPatient
    {
        public Guid Id { get; set; }
        public string PatientNumber { get; set; }
    }
}
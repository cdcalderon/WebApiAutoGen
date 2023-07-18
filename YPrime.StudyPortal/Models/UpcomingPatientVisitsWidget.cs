using System.Collections.Generic;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Models
{
    public class UpcomingPatientVisitsWidget
    {
        public int Count { get; set; }
        public IEnumerable<PatientDto> Patients { get; set; }
    }
}
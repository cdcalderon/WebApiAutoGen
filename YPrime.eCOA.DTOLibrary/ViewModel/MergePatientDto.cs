using System;
using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary.ViewModel
{
    [Serializable]
    public class MergePatientDto
    {
        public Guid PatientId { get; set; }
        public string PatientNumber { get; set; }
        public int PatientStatusTypeId { get; set; }
        public string PatientStatus { get; set; }
        public int Position { get; set; }
        public List<PatientAttributeDto> PatientAttributes { get; set; }
        public List<DiaryEntryDto> DiaryEntries { get; set; }
        public List<PatientVisitDto> PatientVisits { get; set; }
    }
}
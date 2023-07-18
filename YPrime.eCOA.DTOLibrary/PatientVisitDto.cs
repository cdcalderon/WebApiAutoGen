using System;
using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class PatientVisitDto : DtoBase
    {
        public PatientVisitDto()
        {
            DiaryEntries = new List<DiaryEntryDto>();
        }

        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid VisitId { get; set; }

        public int? VisitReasonId { get; set; }

        public int PatientVisitStatusTypeId { get; set; }

        public int IRTPatientVisitStatusTypeId { get; set; }

        public string PatientVisitStatus { get; set; }

        public string VisitName { get; set; }

        public DateTimeOffset? VisitDate { get; set; }

        public DateTimeOffset ProjectedDate { get; set; }

        public DateTimeOffset SystemDate { get; set; }

        public bool OutsideVisitWindow { get; set; }

        public int? UnscheduledVisitOrder { get; set; }

        public Guid? DosageLevelId { get; set; }

        public string Notes { get; set; }

        public string PatientVisitDescription { get; set; }

        public string PatientNumber { get; set; }

        public List<DiaryEntryDto> DiaryEntries { get; set; }

        public int SyncVersion { get; set; }

        public string VisitDateDisplay { get; set; }

        public DateTimeOffset? ActivationDate { get; set; }

        public int? VisitOrder { get; set; }

        public string VisitActivationDateDisplay { get; set; }
    }
}
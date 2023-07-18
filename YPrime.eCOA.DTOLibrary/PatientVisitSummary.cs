using System;
using System.Collections.Generic;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.eCOA.DTOLibrary
{
    public class PatientVisitSummary
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid VisitId { get; set; }
        public int PatientVisitStatusTypeId { get; set; }
        public string PatientVisitStatus { get; set; }
        public DateTimeOffset? VisitDate { get; set; }
        public DateTimeOffset? ActivationDate { get; set; }
        public DateTimeOffset ProjectedDate { get; set; }
        public bool ShowTabletPatientWebBackup { get; set; }
        public bool ShowTabletCaregiverWebBackup { get; set; }
        public string ValidTo { get; set; }
        public bool IsNextScheduled { get; set; }
        public bool VisitInWindow { get; set; }
        public int DiaryEntryCount => DiaryEntries.Count;
        public PatientVisitHardStop PatientVisitHardStop { get; set; }
        public WebBackupEmailModel PatientWebBackUpEmailModel { get; set; }
        public WebBackupEmailModel CaregiverWebBackUpEmailModel { get; set; }
        public List<DiaryEntryDto> DiaryEntries { get; set; }
        public List<CareGiver> Caregivers { get; set; }
        public int VisitOrder { get; set; }
        public string VisitStop_HSN { get; set; }
        public bool AlwaysAvailable { get; set; }
        public bool IsScheduled { get; set; }
        public int WindowBefore { get; set; }
        public int WindowAfter { get; set; }
        public Guid? VisitAvailableBusinessRuleId { get; set; }
        public bool? VisitAvailableBusinessRuleTrueFalseIndicator { get; set; }
        public string VisitName { get; set; }
        public bool CanActivateVisit { get; set; }
        public bool ShowActivateVisit { get; set; }

    }

    public class PatientVisitHardStop
    {
        public string HardStop { get; set; }
        public string HardStopMessage { get; set; }
        public bool ShowYesNo { get; set; }
        public bool ShowOk { get; set; }
    }
}
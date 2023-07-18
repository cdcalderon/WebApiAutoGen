using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.BusinessRule.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models.Attributes;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class PatientVisit : DataSyncBase, IDataSyncObject, IPatientVisit
    {
        public int? VisitReasonId { get; set; }

        public DateTimeOffset ProjectedDate { get; set; }

        public DateTimeOffset? SystemDate { get; set; }

        public bool OutsideVisitWindow { get; set; }

        public int? UnscheduledVisitOrder { get; set; }

        [StringLength(150)] public string Notes { get; set; }

        public int IRTPatientVisitStatusTypeId { get; set; }

        public virtual Patient Patient { get; set; }

        [ForeignKey("MissedVisitReason")] public Guid? MissedVisitReasonId { get; set; }

        public virtual MissedVisitReason MissedVisitReason { get; set; }

        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        [Required] public Guid VisitId { get; set; }

        [SyncDeviceColumnAlways] public int PatientVisitStatusTypeId { get; set; }

        public DateTimeOffset? VisitDate { get; set; }

        public DateTimeOffset? ActivationDate { get; set; }

        public Guid ConfigurationId { get; set; }
    }
}
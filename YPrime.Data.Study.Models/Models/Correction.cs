using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public class Correction : ModelBase, IValidatableObject
    {
        public Correction()
        {
            CorrectionWorkflows = new List<CorrectionWorkflow>();
            CorrectionApprovalDatas = new List<CorrectionApprovalData>();
            CorrectionHistory = new List<CorrectionHistory>();
        }

        public DateTimeOffset StartedDate { get; set; }

        public DateTimeOffset? CompletedDate { get; set; }

        [ForeignKey("StartedByUser")] public Guid StartedByUserId { get; set; }

        public virtual StudyUser StartedByUser { get; set; }


        [ForeignKey("CorrectionStatus")] public Guid CorrectionStatusId { get; set; }

        public virtual CorrectionStatus CorrectionStatus { get; set; }

        public virtual List<CorrectionHistory> CorrectionHistory { get; set; }

        public Guid CorrectionTypeId { get; set; }

        [NotMapped]
        public CorrectionWorkflowSettingsModel CorrectionWorkflowSettings { get; set; }

        public virtual List<CorrectionApprovalData> CorrectionApprovalDatas { get; set; }

        public virtual ICollection<CorrectionDiscussion> CorrectionDiscussions { get; set; }

        public virtual IList<CorrectionWorkflow> CorrectionWorkflows { get; set; }

        public int? CurrentWorkflowOrder { get; set; }

        [ForeignKey("Patient")] public Guid? PatientId { get; set; }

        //remove virtual to lazy load
        public Patient Patient { get; set; }

        [ForeignKey("Site")] public Guid? SiteId { get; set; }

        public virtual Site Site { get; set; }

        [ForeignKey("DiaryEntry")] public Guid? DiaryEntryId { get; set; }

        public virtual DiaryEntry DiaryEntry { get; set; }

        public string ReasonForCorrection { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DataCorrectionNumber { get; private set; }

        public Guid? QuestionnaireId { get; set; }

        public Guid ConfigurationId { get; set; }

        [NotMapped] public bool AllowEdit { get; set; }

        [NotMapped] public bool PatientPreLoaded { get; set; }

        [NotMapped] public string PatientNumber { get; set; }

        [NotMapped] public string SiteName { get; set; }

        [NotMapped] public bool UseMetricMeasurements { get; set; }

        [DefaultValue(0)] public bool NoApprovalNeeded { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CorrectionApprovalDatas != null && CorrectionApprovalDatas.Any() &&
                CorrectionApprovalDatas.All(cad => cad.NewDataPoint == null) &&
                CorrectionApprovalDatas.All(cad => cad.OldDataPoint == null))
            {
                yield return new ValidationResult("Please select a correction.", new[] {"CorrectionApprovalDatas"});
            }
        }       
    }
}
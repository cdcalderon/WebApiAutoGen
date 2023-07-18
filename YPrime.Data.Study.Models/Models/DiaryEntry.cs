using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using YPrime.BusinessRule.Interfaces;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("DiaryEntry")]
    public class DiaryEntry : DataSyncBase, IDataSyncObject, IDiaryEntry
    {
        public DiaryEntry()
        {
            Answers = new HashSet<Answer>();
        }

        public int RowId { get; set; }

        public int DiaryStatusId { get; set; }

        public int DataSourceId { get; set; }

        public Guid? DeviceId { get; set; }

        [NotMapped]
        private DateTime _diaryDate;

        [Column(TypeName = "Date")]
        public DateTime DiaryDate
        {
            get => DaisyChainDate ?? _diaryDate;
            set => _diaryDate = value;
        }

        public DateTimeOffset StartedTime { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("getutcdate()")]
        public DateTimeOffset TransmittedTime { get; set; }

        public Guid? UserId { get; set; }

        public Guid? CareGiverId { get; set; }

        public Guid? VisitId { get; set; }

        public bool Ongoing { get; set; }

        [ForeignKey("ReviewedByUser")] public Guid? ReviewedByUserid { get; set; }

        public DateTimeOffset? ReviewedDate { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual Device Device { get; set; }

        public virtual CareGiver CareGiver { get; set; }

        public virtual StudyUser ReviewedByUser { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public DateTimeOffset CompletedTime { get; set; }

        public Guid QuestionnaireId { get; set; }

        public Guid ConfigurationId { get; set; }

        public string SoftwareVersionNumber { get; set; }

        [NotMapped]
        IList<IAnswer> IDiaryEntry.Answers
        {
            get => Answers.Select(a => a as IAnswer).ToList();
            set => _ = value;
        }

        [NotMapped]
        public DateTime? DaisyChainDate { get; set; }
    }
}

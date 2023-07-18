using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.BusinessRule.Interfaces;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Table("Answer")]
    public class Answer : DataSyncBase, IDataSyncObject, IAnswer, IArchivable
    {
        public Guid? AnswerScoreId { get; set; }

        public virtual AnswerScore AnswerScore { get; set; }

        public virtual DiaryEntry DiaryEntry { get; set; }

        public Guid DiaryEntryId { get; set; }

        public Guid QuestionId { get; set; }

        public Guid? ChoiceId { get; set; }

        public string FreeTextAnswer { get; set; }

        public int? AnswerOrder { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ConfigurationId { get; set; }

        public bool IsArchived { get; set; }
    }
}
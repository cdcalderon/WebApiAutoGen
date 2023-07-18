using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    public class AnswerScore : DataSyncBase, IDataSyncObject
    {
        public float EAP { get; set; }
        public float SEM { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
    }
}
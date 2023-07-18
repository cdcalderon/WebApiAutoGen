using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.BusinessRule.Interfaces;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("PatientAttribute")]
    public class PatientAttribute : DataSyncBase, IDataSyncObject, IPatientAttribute
    {
        public Patient Patient { get; set; }

        [Key] public Guid Id { get; set; }

        [ForeignKey("Patient")] public Guid PatientId { get; set; }

        public Guid PatientAttributeConfigurationDetailId { get; set; }

        public string AttributeValue { get; set; }
    }
}
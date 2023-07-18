using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    public abstract class ModelBase : AuditModel
    {
        protected ModelBase()
        {
            Id = Guid.NewGuid(); // So everytime someone starts a new model they don't have to do this in the repo.
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public Guid Id { get; set; }
    }
}

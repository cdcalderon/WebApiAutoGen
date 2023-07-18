using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YPrime.Data.Study.Models.BaseClasses
{
    public class HistoryModelBase<T, U>
    {
        public HistoryModelBase()
        {
            ChangeDate = DateTimeOffset.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public T Previous { get; set; }
        public T Current { get; set; }
        public DateTimeOffset ChangeDate { get; private set; }

        public virtual void SetRelationshipIDField(U ID)
        {
        }
    }
}
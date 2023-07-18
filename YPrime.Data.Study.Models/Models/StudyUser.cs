using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("StudyUser")]
    public class StudyUser : DataSyncBase, IDataSyncObject
    {
        public StudyUser()
        {
            StudyUserRoles = new HashSet<StudyUserRole>();
            StudyUserWidgets = new HashSet<StudyUserWidget>();
            ReferenceMaterials = new HashSet<ReferenceMaterial>();
        }

        [Key] public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        [StringLength(256)] public string PinHash { get; set; }

        public string LandingPageUrl { get; set; }

        public virtual ICollection<StudyUserRole> StudyUserRoles { get; set; }

        public virtual ICollection<StudyUserWidget> StudyUserWidgets { get; set; }

        public virtual ICollection<ReferenceMaterial> ReferenceMaterials { get; set; }
    }
}
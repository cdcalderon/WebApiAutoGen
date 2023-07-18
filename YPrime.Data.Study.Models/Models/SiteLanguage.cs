using System;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    [Serializable]
    [Table("SiteLanguage")]
    public class SiteLanguage : DataSyncBase, IDataSyncObject
    {
        public Guid SiteId { get; set; }
        public Guid LanguageId { get; set; }

        public virtual Site Site { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.Data.Study.Models
{
    public class AuditModel : IAuditModel
    {
        public string AuditUserID { get; set; }

        [Column(TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("GETUTCDATE()")]
        public DateTime LastModified { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("SUSER_NAME()")]
        public string LastModifiedByDatabaseUser { get; set; }
    }
}

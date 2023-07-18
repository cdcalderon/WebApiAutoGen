using System;

namespace YPrime.Data.Study.Models.Interfaces
{
    public interface IAuditModel
    {
        string AuditUserID { get; set; }
        DateTime LastModified { get; set; }
        string LastModifiedByDatabaseUser { get; set; }
    }
}

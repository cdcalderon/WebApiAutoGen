using System.Collections.Generic;

namespace YPrime.Data.Study.Models
{
    public class ExportStatus
    {
        public ExportStatus()
        {
            Exports = new HashSet<Export>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Export> Exports { get; set; }
    }
}
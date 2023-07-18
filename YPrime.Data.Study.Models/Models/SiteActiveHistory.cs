using System;
using YPrime.Data.Study.Models.BaseClasses;

namespace YPrime.Data.Study.Models
{
    public class SiteActiveHistory : HistoryModelBase<bool, Guid>
    {
        public Guid SiteId { get; set; }
        public virtual Site Site { get; set; }

        public override void SetRelationshipIDField(Guid ID)
        {
            SiteId = ID;
        }
    }
}
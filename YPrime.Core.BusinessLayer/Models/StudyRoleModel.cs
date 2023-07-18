using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class StudyRoleModel : IConfigModel
    {
        public static readonly string LandingPageDelimiter = "~";

        public Guid Id { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }

        public bool IsBlinded { get; set; }

        public string Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        public bool AutoAssignNewSites { get; set; }
        public int LandingPageId { get; set; }
        public string LandingPageName { get; set; }
        public ICollection<SystemActionModel> SystemActions { get; set; } = new HashSet<SystemActionModel>();

    }
}

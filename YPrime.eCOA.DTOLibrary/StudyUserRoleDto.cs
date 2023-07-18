using System;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class StudyUserRoleDto : DtoBase
    {
        public Guid Id { get; set; }

        public Guid StudyUserId { get; set; }

        public Guid StudyRoleId { get; set; }

        public Guid SiteId { get; set; }

        public bool SiteIsActive { get; set; }

        public string Notes { get; set; }
    }
}
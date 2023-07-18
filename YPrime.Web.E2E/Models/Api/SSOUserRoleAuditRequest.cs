using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models.Api
{
    public class SSOUserRoleAuditRequest
    {
        public string ModifiedBy { get; set; }

        public string StudyUserId { get; set; }

        public string StudyRoleId { get; set; }

        public string SiteId { get; set; }

        public string AuditAction { get; set; }
    }
}

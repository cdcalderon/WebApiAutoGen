using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models.Api
{
    public class SSORequestStudy
    {
        public Guid StudyUserId { get; set; }

        public string StudyUserName { get; set; }

        public string StudyUserEmail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class StudyUserDto : ClaimsPrincipal
    {
        public const string CultureCode = "en-US";
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public List<SiteDto> Sites { get; set; }

        public List<StudyRoleModel> Roles { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string LandingPageUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YPrime.eCOA.DTOLibrary.BaseClasses;
using YPrime.Shared.Helpers.Data.Attributes;

//generated this with dto-generator vs extension
namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class SiteDto : EmailDto
    {
        public SiteDto()
        {
            Languages = new List<LanguageDto>();
        }

        public Guid Id { get; set; }

        [Required]
        [Import(Order = 1)]
        [Display(Name = "Site Number")]
        public string SiteNumber { get; set; }

        [Required] [Import(Order = 2)] public string Name { get; set; }

        [Required] [Import(Order = 3)] public string Address1 { get; set; }

        [Import(Order = 4)] public string Address2 { get; set; }

        [Import(Order = 5)] public string Address3 { get; set; }

        [Required] [Display(Name = "Country")] public Guid CountryId { get; set; }

        [Import(Order = 6)] public string CountryName { get; set; }

        [Required] [Import(Order = 7)] public string State { get; set; }

        [Required] [Import(Order = 8)] public string City { get; set; }

        [Required] [Import(Order = 9)] public string Zip { get; set; }

        [Required] [Import(Order = 10)] public string TimeZone { get; set; }

        [Required]
        [Import(Order = 11)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Import(Order = 12)]
        [Display(Name = "Fax Number")]
        public string FaxNumber { get; set; }

        [Required]
        [Import(Order = 13)]
        [Display(Name = "Primary Contact")]
        public string PrimaryContact { get; set; }

        [Display(Name = "Date Of Birth Format")]
        public int? PatientDOBFormatId { get; set; }

        [Required]
        [Import(Order = 17)]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Import(Order = 18)] public string Notes { get; set; }

        [Import(Order = 19)]
        [Display(Name = "Investigator")]
        [Required]
        public string Investigator { get; set; }


        [Import(Order = 20)] public string AllowedLanguages { get; set; }


        public DateTime? LastUpdate { get; set; }

        public List<LanguageDto> Languages { get; set; }
        public List<Guid> SelectedLanguageIds { get; set; } = new List<Guid>();

        public DateTime? WebBackupExpireDate { get; set; }

        [Display(Name = "Web Backup Enabled")] public bool IsWebBackupEnabled { get; set; }

        public string DisplayAddress
        {
            get
            {
                var addressArray = new List<string> {Address1, Address2, Address3};
                return string.Join(", ", addressArray.Where(x => !string.IsNullOrEmpty(x)));
            }
        }

        public string EditLinkHTML { get; set; }
        public string WebBackupAssetTag { get; set; }
        public string WebBackupComputedURL { get; set; }

        [Display(Name = "Site-Facing Text Display Language")]
        public Guid? SiteDisplayLanguageId { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ConfirmationTypeDto : DtoBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string TranslationKey { get; set; }

        [Display(Name = "Blinded")] public bool IsBlinded { get; set; }

        [Display(Name = "Site Specific")] public bool IsSiteSpecific { get; set; }

        public string Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        [Display(Name = "Email Body")] public string BodyTemplate { get; set; }

        [Display(Name = "Email Subject")] public string SubjectLineTemplate { get; set; }

        [Display(Name = "Send to Creator")] public bool IsEmailSentToPerformingUser { get; set; }

        [Display(Name = "Type")] public Guid EmailContentTypeId { get; set; }

        public int? PatientStatusTypeId { get; set; }

        public bool DisplayOnScreen { get; set; }

        public bool AssociatedToUser { get; set; }

        public override bool Equals(object obj)
        {
            ConfirmationTypeDto other = obj as ConfirmationTypeDto;
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
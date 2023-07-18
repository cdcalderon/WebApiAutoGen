using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ReferenceMaterialDto : DtoBase
    {
        public Guid Id { get; set; }

        [DisplayName("User")] [Required] public Guid UserId { get; set; }

        [DisplayName("Type")] [Required] public Guid ReferenceMaterialTypeId { get; set; }

        [DisplayName("Name")] [Required] public string Name { get; set; }

        [Required] public HttpPostedFileBase File { get; set; }

        //[Required]
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset? UpdatedTime { get; set; }

        public string ReferenceMaterialType { get; set; }

        public string Username { get; set; }
    }
}
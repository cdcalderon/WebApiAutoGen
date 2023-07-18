using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class DCFRequestDto : DtoBase
    {
        public int ID { get; set; }

        [DisplayName("User")] [Required] public string UserID { get; set; }

        public string UserFirstLast { get; set; }

        public Guid SiteId { get; set; }

        public string SiteNumber { get; set; }

        public Guid? PatientId { get; set; }

        [Required] public string PatientNumber { get; set; }

        public string Username { get; set; }

        [DisplayName("Type Of Data Change")]
        [Required]
        public string TypeOfDataChange { get; set; }

        [DisplayName("Old Value")] [Required] public string OldValue { get; set; }

        [DisplayName("New Value")] [Required] public string NewValue { get; set; }

        public string Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        public string TicketNumber { get; set; }
    }
}
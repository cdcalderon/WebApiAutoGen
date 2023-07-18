using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class SendingEmailModel : YPrime.Auth.Data.Models.JSON.EmailModel
    {
        public Guid SponsorId { get; set; }
        public string Environment { get; set; }
    }
}

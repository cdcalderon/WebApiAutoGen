using System;
using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ConfirmationDto
    {
        public ConfirmationDto()
        {
            ConfirmationData = new Dictionary<string, string>();
        }

        public IDictionary<string, string> ConfirmationData { get; set; }

        public Guid ConfirmationTypeId { get; set; }
        public Guid? SiteId { get; set; }
        public Guid? UserId { get; set; }
    }
}
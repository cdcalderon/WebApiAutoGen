using System;

namespace YPrime.Core.BusinessLayer.Models
{
    public class ConfigurationVersion : IConfigModel
    {
        public Guid Id { get; set; }

        public Guid StudyId { get; set; }

        public string SrdVersion { get; set; }

        public string ConfigurationVersionNumber { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public bool ApprovedForProd { get; set; }

        public string Description { get; set; }

        public string DisplayVersion => $"{ConfigurationVersionNumber}-{SrdVersion}";
    }
}

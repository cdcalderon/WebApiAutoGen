using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.PowerBi
{
    [Serializable]
    public class PowerBiContext : IPowerBiContext
    {
        private const char TermSeparator = '|';

        private string[] excludedTermSet;

        public PowerBiContext(
            string apiUrl,
            string applicationId,
            string applicationSecret,
            string authorityUrl,
            string resourceUrl,
            string tenantId,
            Guid? workspaceId,
            string excludedPrefix = null,
            string excludedTerms = null)
        {
            ApiUrl = apiUrl;
            ApplicationId = applicationId;
            ApplicationSecret = applicationSecret;
            AuthorityUrl = authorityUrl;
            ResourceUrl = resourceUrl;
            TenantId = tenantId;
            WorkspaceId = workspaceId;
            ExcludedPrefix = excludedPrefix;
            ExcludedTerms = excludedTerms;
        }

        [Required]
        public string ApiUrl { get; }

        [Required]
        public string ApplicationId { get; }

        [Required]
        public string ApplicationSecret { get; }

        [Required]
        public string AuthorityUrl { get; }

        public string ExcludedPrefix { get; }

        [JsonIgnore]
        public string[] ExcludedTermSet
        {
            get
            {
                if (excludedTermSet == null)
                {
                    excludedTermSet = ExcludedTerms?.Split(TermSeparator);
                }

                return excludedTermSet;
            }
        }

        public string ExcludedTerms { get; }

        [Required]
        public string ResourceUrl { get; }

        [Required]
        public string TenantId { get; }

        [Required]
        public Guid? WorkspaceId { get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
using System;
using System.Collections.Generic;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class CorrectionWorkflowSettingsModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public bool NoApprovalNeeded { get; set; }

        public List<CorrectionTypeConfigurationModel> Configurations { get; set; } = new List<CorrectionTypeConfigurationModel>();
    }
}

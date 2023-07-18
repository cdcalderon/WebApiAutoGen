using System;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class VisitModel : IConfigModel, IHasCustomExtensions
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int DaysExpected { get; set; }

        public int WindowBefore { get; set; }

        public int WindowAfter { get; set; }

        public string Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        public bool WindowOverride { get; set; }

        public string OverrideReason { get; set; }

        public bool IsScheduled { get; set; }

        public int VisitOrder { get; set; }

        public Guid? VisitAnchor { get; set; }

        public string VisitStop_HSN { get; set; }

        public bool CloseoutFormAvailable { get; set; }

        public bool AlwaysAvailable { get; set; }

        public bool CircularVisit { get; set; }

        public int? CircularVisitRepeatCount { get; set; }

        public string MaxLength { get; set; }

        public bool DosageModuleEnabled { get; set; }

        public bool? VisitAvailableBusinessRuleTrueFalseIndicator { get; set; }

        public Guid? VisitAvailableBusinessRuleId { get; set; }

        public ICollection<VisitQuestionnaireModel> Questionnaires { get; set; }

        public List<CustomExtensionModel> CustomExtensions { get; set; } = new List<CustomExtensionModel>();
    }
}

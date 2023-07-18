using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class QuestionnaireTakerModel
    {
        public string DeviceType { get; set; }
        public string QuestionnaireTypeName { get; set; }
        public int QuestionnaireTypeId { get; set; }
        public bool IsTraining { get; set; }
    }
}

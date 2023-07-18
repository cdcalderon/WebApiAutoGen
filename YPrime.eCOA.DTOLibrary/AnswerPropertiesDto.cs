using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class AnswerPropertiesDto
    {
        public AnswerPropertiesDto()
        {         
        }

        public Guid DiaryEntryId { get; set; }

        //-- since we are saving temperature answers in Celsius we want to default to true for use of metric. 
        public bool UseMetricForAnswers { get; set; } = true;

    }
}

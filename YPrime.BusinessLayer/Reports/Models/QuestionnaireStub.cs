using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Reports.Models
{
    public class QuestionnaireStub : ConfigStub<Guid>
    {
        public string InternalName { get; set; }
        public int? QuestionnaireTypeId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Reports.Models
{
    public class ChoiceStub : ConfigStub<Guid>
    {
        public Guid QuestionId { get; set; }
    }
}

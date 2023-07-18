using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Reports.Models
{
    public class VisitStub : ConfigStub<Guid>
    {
        public bool IsScheduled { get; set; }
        public int VisitOrder { get; set; }
    }
}

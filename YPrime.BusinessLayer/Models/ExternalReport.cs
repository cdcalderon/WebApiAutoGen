using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Models
{
    public class ExternalReport
    {
        public Report Report { get; set; }

        public bool IsSponsorReport { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.Responses
{
    public class PatientResponse : IPatientResponse
    {
        public bool Success { get; set; }
        public Guid PatientId { get; set; }
    }
}

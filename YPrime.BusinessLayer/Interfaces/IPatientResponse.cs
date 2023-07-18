using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IPatientResponse
    {
        bool Success { get; set; }
        Guid PatientId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Exceptions
{
    public class PatientNotFoundException : BusinessException
    {
        private const string ExceptionMessage = "Patient Id not found";
        public override string Message => ExceptionMessage;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Exceptions
{
    public class DeviceTypeNotFoundException : BusinessException
    {
        private const string ExceptionMessage = "Device type not found";

        public override string Message => ExceptionMessage;
    }
}

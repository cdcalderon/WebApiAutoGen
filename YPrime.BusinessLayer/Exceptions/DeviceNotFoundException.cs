using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Exceptions
{
    public class DeviceNotFoundException : BusinessException
    {
        private const string ExceptionMessage = "Device not found";

        public override string Message => ExceptionMessage;
    }
}

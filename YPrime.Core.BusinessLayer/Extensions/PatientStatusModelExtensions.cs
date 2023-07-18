using System;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Extensions
{
    public static class PatientStatusModelExtensions
    {
        public static bool IsDisabled(this PatientStatusModel statusModel)
        {
            if (statusModel == null) throw new ArgumentNullException($"{nameof(statusModel)} can not be null.");

            return !statusModel.IsActive || statusModel.IsRemoved;
        }
    }
}

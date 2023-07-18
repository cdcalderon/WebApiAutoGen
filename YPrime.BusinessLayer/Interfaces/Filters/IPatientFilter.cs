using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Interfaces.Filters
{
    public interface IPatientFilter
    {
        IPatientFilter ByPatientStatusTypeId(int? patientStatusTypeId);
        IPatientFilter BySiteId(Guid? siteId);
        IPatientFilter ById(Guid id);
        IPatientFilter ByPatientNumber(string patientNumber);
    }
}
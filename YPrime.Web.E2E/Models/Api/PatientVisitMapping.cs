using System;
using YPrime.Data.Study.Models;

namespace YPrime.Web.E2E.Models.Api
{
    public class PatientVisitMapping
    {
        public string MappingName { get; set; }
        public Guid PatientId { get; set; }
        public Guid VisitId { get; set; }
        public PatientVisit PatientVisit { get; set; }
    }
}

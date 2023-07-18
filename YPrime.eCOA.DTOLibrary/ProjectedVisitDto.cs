using System;

namespace YPrime.eCOA.DTOLibrary
{
    public class ProjectedVisitDto
    {
        public Guid PatientId { get; set; }
        public int VisitId { get; set; }
        public DateTimeOffset VisitDate { get; set; }
        public Guid? PatientVisitId { get; set; }
    }
}
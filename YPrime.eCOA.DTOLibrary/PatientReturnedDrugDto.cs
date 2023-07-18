using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.eCOA.DTOLibrary
{
    public class PatientWithReturnedDrugDto
    {
        public string PatientNumber { get; set; }

        public string SiteNumber { get; set; }

        public DateTimeOffset EnrolledDate { get; set; }

        public Guid Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models.Api
{
    public class SubjectsSet
    {
        public string SubjectNumber { get; set; }
        public string SubjectStatus { get; set; }
        public string EnrollmentDate { get; set; }
        public bool HandheldTrainingComplete { get; set; }
        public bool TabletTrainingComplete { get; set; }
        public string LastDiaryDate { get; set; }
        public string LastSyncDate { get; set; }

    }
}

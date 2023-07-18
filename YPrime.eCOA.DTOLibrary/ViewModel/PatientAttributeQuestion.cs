using System;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary.ViewModel
{
    public class PatientAttributeQuestion
    {
        public Guid CorrectionId { get; set; }
        public Guid PatientId { get; set; }
        public SubjectInformationModel SubjectInformation { get; set; }
        public PatientAttributeDto PatientAttribute { get; set; }
        public string NewDataPoint { get; set; }
        public string NewDisplayValue { get; set; }
        public string ColumnName { get; set; }
    }
}
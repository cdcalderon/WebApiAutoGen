using System;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class PatientAttributeDto : DtoBase
    {
        public PatientAttributeDto()
        {
            CorrectionApprovalDatas = new List<CorrectionApprovalDataDto>();
        }

        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid PatientAttributeConfigurationDetailId { get; set; }
        public string AttributeValue { get; set; }
        public SubjectInformationModel SubjectInformation { get; set; }
        public List<CorrectionApprovalDataDto> CorrectionApprovalDatas { get; set; }
        public string DisplayValue { get; set; }
        [SkipPropertyCopy]
        public bool NewAttributeData { get; set; }
    }
}
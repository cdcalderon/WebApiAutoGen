using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Extensions
{
    public static class PatientAttributeExtensions
    {
        public static PatientAttributeDto ToDto(
            this PatientAttribute entity,
            SubjectInformationModel subjectInformationModel)
        {
            var model = new PatientAttributeDto
            {
                Id = entity.Id,
                PatientId = entity.PatientId,
                PatientAttributeConfigurationDetailId = entity.PatientAttributeConfigurationDetailId,
                AttributeValue = entity.AttributeValue,
                SubjectInformation = subjectInformationModel,
                DisplayValue = subjectInformationModel?.Name ?? string.Empty
            };

            return model;
        }
    }
}

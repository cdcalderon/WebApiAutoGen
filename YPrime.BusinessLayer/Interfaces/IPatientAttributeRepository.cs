using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IPatientAttributeRepository
    {
        Task<IEnumerable<PatientAttributeDto>> GetPatientAttributes(Guid patientId, string cultureCode);

        Task<List<PatientAttributeQuestion>> GetPatientAttributesForCorrection(Guid patientId, Guid correctionId,
            string cultureCode, IList<CorrectionApprovalData> correctionApprovalData);

        Task<string> ExtractPatientNumber(string patientID);
    }
}
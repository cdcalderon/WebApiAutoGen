using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface ISubjectInformationService : IConfigServiceBase<SubjectInformationModel, Guid>
    {
        Task<List<SubjectInformationModel>> GetForCountry(Guid countryId, Guid? configurationId = null);
    }
}

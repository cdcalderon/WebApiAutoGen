using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface ICareGiverTypeService : IConfigServiceBase<CareGiverTypeModel, Guid>
    {
        Task<List<CareGiverTypeModel>> GetAllAlphabetical(Guid? configurationId = null);
    }
}
